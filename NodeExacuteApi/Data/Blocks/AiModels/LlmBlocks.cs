using NodeBaseApi.Version2;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Type = NodeBaseApi.Version2.Type;

namespace NodeExacuteApi.Data.Blocks.AiModels
{
    public class Llama27b : Block
    {
        public Llama27b()
        {
            Id = Guid.NewGuid();
            Name = "Llama2 7b";
            Description = "Llama 2-7b API";
            Inputs = new List<Input>
        {
            new Input { Name = "Query", Type = Type.String, Description = "The input query for the Llama 2-7b API", IsRequired = true },
            new Input { Name = "MaxNewTokens", Type = Type.Number, Description = "Maximum new tokens to be generated", IsRequired = true},
            new Input { Name = "TopP", Type = Type.Number, Description = "Top P value for controlling randomness", IsRequired = true },
            new Input { Name = "Temperature", Type = Type.Number, Description = "Temperature value for controlling creativity", IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Name = "ApiResponse", Type = Type.String, Description = "The API response from Llama 2-7b" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            var query = inputs[0].ToString();
            var maxNewTokens = Convert.ToInt32(inputs[1]);
            var topP = Convert.ToDouble(inputs[2]);
            var temperature = Convert.ToDouble(inputs[3]);

            var responseString = await CallLlama27bApiAsync(query, maxNewTokens, topP, temperature);
            programStructure.InputValues[Outputs[0].Id] = responseString;
        }

        private async Task<string> CallLlama27bApiAsync(string query, int maxNewTokens = 1024, double topP = 0.8, double temperature = 0.6)
        {
            var url = "https://api-inference.huggingface.co/models/meta-llama/Llama-2-7b-chat-hf";
            var parameters = new
            {
                max_new_tokens = maxNewTokens,
                top_p = topP,
                temperature = temperature,
                return_full_text = false  // To prevent the response from including the input prompt
            };
            var payload = new
            {
                inputs = $"[INST] <<SYS>> {query} [/INST]",
                parameters
            };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer YOUR_HF_TOKEN_HERE");

            var response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                // Extracting only the generated text
                var generatedText = jsonResponse[0].generated_text.ToString();

                return generatedText;
            }
            else
            {
                throw new Exception($"API call failed: {response.StatusCode}");
            }
        }
    }

    public class Llama213b : Block
    {
        public Llama213b()
        {
            Id = Guid.NewGuid();
            Name = "Llama2 13b";
            Description = "Llama 2-13b API";
            Inputs = new List<Input>
        {
            new Input { Name = "Query", Type = Type.String, Description = "The input query for the Llama 2-13b API", IsRequired = true },
            new Input { Name = "MaxNewTokens", Type = Type.Number, Description = "Maximum new tokens to be generated", IsRequired = true},
            new Input { Name = "TopP", Type = Type.Number, Description = "Top P value for controlling randomness", IsRequired = true },
            new Input { Name = "Temperature", Type = Type.Number, Description = "Temperature value for controlling creativity", IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Name = "ApiResponse", Type = Type.String, Description = "The API response from Llama 2-13b" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            var query = inputs[0].ToString();
            var maxNewTokens = Convert.ToInt32(inputs[1]);
            var topP = Convert.ToDouble(inputs[2]);
            var temperature = Convert.ToDouble(inputs[3]);

            var responseString = await CallLlama213bApiAsync(query, maxNewTokens, topP, temperature);
            programStructure.InputValues[Outputs[0].Id] = responseString;
        }

        private async Task<string> CallLlama213bApiAsync(string query, int maxNewTokens = 1024, double topP = 0.8, double temperature = 0.6)
        {
            var url = "https://api-inference.huggingface.co/models/meta-llama/Llama-2-13b-chat-hf";
            var parameters = new
            {
                max_new_tokens = maxNewTokens,
                top_p = topP,
                temperature = temperature,
                return_full_text = false  // To prevent the response from including the input prompt
            };
            var payload = new
            {
                inputs = $"[INST] <<SYS>> {query} [/INST]",
                parameters
            };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer YOUR_HF_TOKEN_HERE");

            var response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                // Extracting only the generated text
                var generatedText = jsonResponse[0].generated_text.ToString();

                return generatedText;
            }
            else
            {
                throw new Exception($"API call failed: {response.StatusCode}");
            }
        }
    }

    public class Llama270b : Block
    {
        public Llama270b()
        {
            Id = Guid.NewGuid();
            Name = "Llama2 70b";
            Description = "Llama 2-70b API";
            Inputs = new List<Input>
        {
            new Input { Name = "Query", Type = Type.String, Description = "The input query for the Llama 2-70b API", IsRequired = true },
            new Input { Name = "MaxNewTokens", Type = Type.Number, Description = "Maximum new tokens to be generated", IsRequired = true},
            new Input { Name = "TopP", Type = Type.Number, Description = "Top P value for controlling randomness", IsRequired = true },
            new Input { Name = "Temperature", Type = Type.Number, Description = "Temperature value for controlling creativity", IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Name = "ApiResponse", Type = Type.String, Description = "The API response from Llama 2-70b" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            var query = inputs[0].ToString();
            var maxNewTokens = Convert.ToInt32(inputs[1]);
            var topP = Convert.ToDouble(inputs[2]);
            var temperature = Convert.ToDouble(inputs[3]);

            var responseString = await CallLlama270bApiAsync(query, maxNewTokens, topP, temperature);
            programStructure.InputValues[Outputs[0].Id] = responseString;
        }

        private async Task<string> CallLlama270bApiAsync(string query, int maxNewTokens = 1024, double topP = 0.8, double temperature = 0.6)
        {
            var url = "https://api-inference.huggingface.co/models/meta-llama/Llama-2-70b-chat-hf";
            var parameters = new
            {
                max_new_tokens = maxNewTokens,
                top_p = topP,
                temperature = temperature,
                return_full_text = false  // To prevent the response from including the input prompt
            };
            var payload = new
            {
                inputs = $"[INST] <<SYS>> {query} [/INST]",
                parameters
            };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer YOUR_HF_TOKEN_HERE");

            var response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                // Extracting only the generated text
                var generatedText = jsonResponse[0].generated_text.ToString();

                return generatedText;
            }
            else
            {
                throw new Exception($"API call failed: {response.StatusCode}");
            }
        }
    }
}
