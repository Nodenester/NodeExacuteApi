using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodeBaseApi.Version2;
using Org.BouncyCastle.Bcpg;
using OtpNet;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace NodeExacuteApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgramController : ControllerBase
    {
        DateTime Stime { get; set; } = DateTime.UtcNow;

        private readonly DBConnection _dbConnection;

        public ProgramController(DBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        [HttpPost("execute")]
        public async Task<ActionResult> ExecuteProgramAsync([FromQuery] Guid programKey, [FromBody] Dictionary<Guid, object> inputValues, [FromQuery] Guid apiKey, [FromQuery] Guid? sessionId = null, [FromQuery] bool isTest = false, [FromQuery] bool isCustomBlock = false, [FromQuery] string testToken = "")
        {
            if (programKey == Guid.Empty)
            {
                return BadRequest(new { error = "Invalid program key." });
            }

            if (apiKey == Guid.Empty)
            {
                return BadRequest(new { error = "No API key." });
            }

            if (inputValues == null || inputValues.Count == 0)
            {
                return BadRequest(new { error = "No input values provided." });
            }

            if(!isTest && isCustomBlock)
            {
                return BadRequest(new { error = "You can only trigger customblocks from the webapp." });
            }

            if(isTest && !ValidateTOTP(testToken))
            {
                return BadRequest(new { error = "Api calls cant be tests" });
            }

            //check if the api key exists
            if (!isTest && !await _dbConnection.ApiKeyExistsAsync(apiKey.ToString()))
            {
                return BadRequest(new { error = "Invalid API key." });
            }

            var UserId = await _dbConnection.GetUserIdByApiKeyAsync(apiKey.ToString());
            if (isTest && UserId == null)
            {
                UserId = apiKey;
                //return NotFound();
            }

            int maxPrice = await _dbConnection.GetUserTokensAsync(apiKey, !isTest);
            if(maxPrice < 50)
            {
                return BadRequest(new { error = "Not enough tokens." });
            }

            CustomProgram program = new CustomProgram();
            try
            {
                program = await _dbConnection.LoadProgramAsyncApi(programKey.ToString(), isCustomBlock);
            }
            catch (Exception ex)
            {
                return NotFound(new { error =  $"Program not found.{ex.Message}"});
            }
            if (program == null || program.ProgramStructure == null)
            {
                return NotFound(new { error = "Program not found 2." });
            }

            program.ProgramStructure.MaxPrice = maxPrice;

            Session session = null;

            if(program.SupportsSessions && sessionId.HasValue)
            {
                session = await _dbConnection.GetSessionAsync(sessionId.ToString());

                if (session == null)
                {
                    var random = new Random();
                    var randomString = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 6)
                                                  .Select(s => s[random.Next(s.Length)]).ToArray());

                    // Check if the program supports sessions
                    session = new Session
                    {
                        SessionId = sessionId.Value,
                        UserId = UserId.ToString(),
                        ProgramId = program.Id.ToString(),
                        Variables = JsonConvert.SerializeObject(new Dictionary<Guid, object>()),
                        SessionName = $"Session-{randomString}"  // "Session-" followed by the random string
                    };
                    await _dbConnection.CreateSessionAsync(session);

                    // Set the session variables in the program structure
                    var sessionVariables = JsonConvert.DeserializeObject<Dictionary<Guid, object>>(session.Variables);
                    program.ProgramStructure.SetSessionVariables(sessionVariables);
                    return BadRequest(new { error = "Session not found and the program does not support sessions." });
                }
                else
                {
                    var sessionVariables = JsonConvert.DeserializeObject<Dictionary<Guid, object>>(session.Variables);
                    program.ProgramStructure.SetSessionVariables(sessionVariables);
                }
            }


            // Set the input values for the program
            foreach (var kvp in inputValues)
            {
                program.ProgramStructure.SetInputValue(program.ProgramStructure.ProgramStart.FindIndex(output => output.Id == kvp.Key), kvp.Value);
            }

            // Execute the program
            try
            {
                await program.ProgramStructure.ExecuteProgram(null);
            }
            catch (Exception ex)
            {
                int tokensLeft = program.ProgramStructure.CurrentPrizing - 10;
                await _dbConnection.UpdateUserTokensAsync(apiKey, tokensLeft, !isTest);
                return StatusCode(500, new { error = $"Error executing program: {ex.Message}" });
            }

            // If the session is not null, update the session variables in the database
            if (session != null)
            {
                var updatedSessionVariables = program.ProgramStructure.GetSessionVariables();
                session.Variables = JsonConvert.SerializeObject(updatedSessionVariables);
                await _dbConnection.UpdateSessionAsync(session);
            }

            // Add pricing

            //Create The output
            List<object> ProgramOutput = new List<object>();

            foreach (var inputId in program.ProgramStructure.ProgramEndConnections)
            {
                if (program.ProgramStructure.InputValues.ContainsKey(inputId.Value) &&
                    program.ProgramStructure.ProgramEnd.Any(input => input.Id == inputId.Key && input.Type != NodeBaseApi.Version2.Type.Trigger))
                {
                    if (program.ProgramStructure.InputValues.ContainsKey(inputId.Value))
                    {
                        ProgramOutput.Add(program.ProgramStructure.InputValues[inputId.Value]);
                    }
                    else if (program.ProgramStructure.DirectInputValues.ContainsKey(inputId.Value))
                    {
                        ProgramOutput.Add(program.ProgramStructure.DirectInputValues[inputId.Value]);
                    }
                }
            }

            // Register the call
            var call = new Call
            {
                ProgramId = program.Id.ToString(),
                ApiUserId = apiKey.ToString(),
                Cost = program.ProgramStructure.CurrentPrizing.ToString(),
                IsTest = isTest,
                StartTime = Stime,
                EndTime = DateTime.UtcNow,
                //Input = inputValues.Select(kvp => (object)kvp).ToList(),
                //Output = ProgramOutput
            };

            try
            {
                int tokensLeft = program.ProgramStructure.CurrentPrizing;
                await _dbConnection.InsertCallAsync(call);
                await _dbConnection.UpdateUserTokensAsync(apiKey, tokensLeft, !isTest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error Logging Call: {ex.Message}" });
            }

            // Get the output values from the program and return them as JSON;
            var convertedOutput = new List<object>();
            foreach (var output in ProgramOutput)
            {
                convertedOutput.Add(ByteSwitch(output));
            }
            ProgramOutput = convertedOutput;

            return Ok(ProgramOutput);
        }

        private object ByteSwitch(object output)
        {
            switch (output)
            {
                case List<object> list:
                    var convertedArray = new List<object>();
                    foreach (var obj in list)
                    {
                        convertedArray.Add(ByteSwitch(obj));
                    }
                    return convertedArray;

                case string text when IsBase64String(text) && IsImage(text):
                    return Convert.FromBase64String(text); // Convert image base64 string to byte array and return

                case string text when IsBase64String(text) && IsAudio(text):
                    return Convert.FromBase64String(text); // Convert audio base64 string to byte array and return

                default:
                    return output; // Return the output as is for any other type
            }
        }

        private bool IsBase64String(string s)
        {
            // Check if the string is likely a base64 string
            return s.Length % 4 == 0 && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$");
        }

        private bool IsImage(string base64)
        {
            if (base64.StartsWith("iVBOR")) return true; // PNG
            if (base64.StartsWith("/9j/")) return true;  // JPEG
            if (base64.StartsWith("R0lGOD") || base64.StartsWith("Qk02U")) return true;
            return false;
        }

        private bool IsAudio(string base64)
        {
            // Implement based on known patterns for your audio format
            // Example pattern check (this is just an example and might not be accurate for your case)
            if (base64.StartsWith("UklGR")) return true;
            return false;
        }

        public bool ValidateTOTP(string totpToken)
        {
            string secretKeyBase64 = "YOUR_TOTP_SECRET_HERE";
            byte[] secretKey = Convert.FromBase64String(secretKeyBase64);
            var totp = new Totp(secretKey);
            return totp.VerifyTotp(totpToken, out long timeStepMatched, window: null);
        }
    }
}
