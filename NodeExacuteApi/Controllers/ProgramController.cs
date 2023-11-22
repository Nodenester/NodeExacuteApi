using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NodeBaseApi.Version2;
using Org.BouncyCastle.Bcpg;

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
        public async Task<ActionResult> ExecuteProgramAsync([FromQuery] Guid programKey, [FromBody] Dictionary<Guid, object> inputValues, [FromQuery] Guid apiKey, [FromQuery] Guid? sessionId = null, bool isTest = false)
        {
            if (programKey == Guid.Empty)
            {
                return BadRequest(new { error = "Invalid program key." });
            }

            if (apiKey == Guid.Empty)
            {
                return BadRequest(new { error = "Invalid API key." });
            }

            if (inputValues == null || inputValues.Count == 0)
            {
                return BadRequest(new { error = "No input values provided." });
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
                program = await _dbConnection.LoadProgramAsyncApi(programKey.ToString());
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

            bool isFirstIteration = true;
            foreach (var inputId in program.ProgramStructure.ProgramEndConnections)
            {
                if (isFirstIteration)
                {
                    isFirstIteration = false;
                    continue;
                }

                if (program.ProgramStructure.InputValues.ContainsKey(inputId.Value))
                {
                    ProgramOutput.Add(program.ProgramStructure.InputValues[inputId.Value]);
                }
                else if (program.ProgramStructure.DirectInputValues.ContainsKey(inputId.Value))
                {
                    ProgramOutput.Add(program.ProgramStructure.DirectInputValues[inputId.Value]);
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
                Input = inputValues.Select(kvp => (object)kvp).ToList(),
                Output = ProgramOutput
            };

            try
            {
                int tokensLeft = program.ProgramStructure.MaxPrice - program.ProgramStructure.CurrentPrizing;
                await _dbConnection.InsertCallAsync(call);
                await _dbConnection.UpdateUserTokensAsync(apiKey, tokensLeft, !isTest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error Logging Call: {ex.Message}" });
            }

            // Get the output values from the program and return them as JSON;
            return Ok(ProgramOutput);
        }
    }
}
