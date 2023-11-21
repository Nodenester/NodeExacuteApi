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
                new Input { Name = "Query", Type = Type.String, Description = "The input query for the Llama 2-13b API", IsRequired = true },
                new Input { Name = "MaxNewTokens", Type = Type.Number, Description = "Maximum new tokens to be generated (Max tokens 4096)", IsRequired = false},
                new Input { Name = "TopP", Type = Type.Number, Description = "Top P value for controlling randomness", IsRequired = false },
                new Input { Name = "Temperature", Type = Type.Number, Description = "Temperature value for controlling creativity", IsRequired = false },
                new Input { Name = "StopWords", Type = Type.String, IsList = true, Description = "List of words where the AI should stop generating text", IsRequired = false },
                new Input { Name = "ReturnFullText", Type = Type.Boolean, Description = "Whether to include the input prompt in the API response", IsRequired = false }
            };
            Outputs = new List<Output>
            {
                new Output { Name = "ApiResponse", Type = Type.String, Description = "The API response from Llama 2-7b" }
            };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            var query = inputs[0]?.ToString() ?? string.Empty;
            var maxNewTokens = inputs.Count > 1 && inputs[1] != null ? Convert.ToInt32(inputs[1]) : 1024; // default value if null
            var topP = inputs.Count > 2 && inputs[2] != null ? Convert.ToDouble(inputs[2]) : 0.8; // default value if null
            var temperature = inputs.Count > 3 && inputs[3] != null ? Convert.ToDouble(inputs[3]) : 0.6; // default value if null
            var returnFullText = inputs.Count > 5 && inputs[5] != null ? Convert.ToBoolean(inputs[5]) : false; // default value if null

            // Safely handling the stopwords input
            var stopWordsInput = inputs.Count > 4 ? inputs[4] : null;
            var stopWords = new List<string>();
            if (stopWordsInput != null)
            {
                if (stopWordsInput is List<object> stopWordsObjectList)
                {
                    stopWords = stopWordsObjectList.Select(obj => obj.ToString()).ToList();
                }
            }

            var responseString = await CallLlama27bApiAsync(query, stopWords, maxNewTokens, topP, temperature, returnFullText);
            programStructure.InputValues[Outputs[0].Id] = responseString;
        }

        private async Task<string> CallLlama27bApiAsync(string query, List<string> stopWords, int maxNewTokens = 1024, double topP = 0.8, double temperature = 0.6, bool returnFullText = false)
        {
            var url = "https://api-inference.huggingface.co/models/meta-llama/Llama-2-7b-chat-hf";
            object parameters;
            if (stopWords.Count == 0 || (stopWords.Count == 1 && stopWords[0] == ""))
            {
                parameters = new
                {
                    max_new_tokens = maxNewTokens,
                    top_p = topP,
                    temperature = temperature,
                    return_full_text = returnFullText
                };
            }
            else
            {
                parameters = new
                {
                    max_new_tokens = maxNewTokens,
                    top_p = topP,
                    temperature = temperature,
                    return_full_text = returnFullText,
                    stop = stopWords
                };
            }
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
                new Input { Name = "MaxNewTokens", Type = Type.Number, Description = "Maximum new tokens to be generated (Max tokens 4096)", IsRequired = false},
                new Input { Name = "TopP", Type = Type.Number, Description = "Top P value for controlling randomness", IsRequired = false },
                new Input { Name = "Temperature", Type = Type.Number, Description = "Temperature value for controlling creativity", IsRequired = false },
                new Input { Name = "StopWords", Type = Type.String, IsList = true, Description = "List of words where the AI should stop generating text", IsRequired = false },
                new Input { Name = "ReturnFullText", Type = Type.Boolean, Description = "Whether to include the input prompt in the API response", IsRequired = false }
            };
            Outputs = new List<Output>
            {
                new Output { Name = "ApiResponse", Type = Type.String, Description = "The API response from Llama 2-13b" }
            };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            var query = inputs[0]?.ToString() ?? string.Empty;
            var maxNewTokens = inputs.Count > 1 && inputs[1] != null ? Convert.ToInt32(inputs[1]) : 1024; // default value if null
            var topP = inputs.Count > 2 && inputs[2] != null ? Convert.ToDouble(inputs[2]) : 0.8; // default value if null
            var temperature = inputs.Count > 3 && inputs[3] != null ? Convert.ToDouble(inputs[3]) : 0.6; // default value if null
            var returnFullText = inputs.Count > 5 && inputs[5] != null ? Convert.ToBoolean(inputs[5]) : false; // default value if null

            // Safely handling the stopwords input
            var stopWordsInput = inputs.Count > 4 ? inputs[4] : null;
            var stopWords = new List<string>();
            if (stopWordsInput != null)
            {
                if (stopWordsInput is List<object> stopWordsObjectList)
                {
                    stopWords = stopWordsObjectList.Select(obj => obj.ToString()).ToList();
                }
            }

            var responseString = await CallLlama213bApiAsync(query, stopWords, maxNewTokens, topP, temperature, returnFullText);
            programStructure.InputValues[Outputs[0].Id] = responseString;
        }

        private async Task<string> CallLlama213bApiAsync(string query, List<string> stopWords, int maxNewTokens = 1024, double topP = 0.8, double temperature = 0.6, bool returnFullText = false)
        {
            var url = "https://api-inference.huggingface.co/models/meta-llama/Llama-2-13b-chat-hf";
            object parameters;
            if (stopWords.Count == 0 || (stopWords.Count == 1 && stopWords[0] == ""))
            {
                parameters = new
                {
                    max_new_tokens = maxNewTokens,
                    top_p = topP,
                    temperature = temperature,
                    return_full_text = returnFullText
                };
            }
            else
            {
                parameters = new
                {
                    max_new_tokens = maxNewTokens,
                    top_p = topP,
                    temperature = temperature,
                    return_full_text = returnFullText,
                    stop = stopWords
                };
            }
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
                new Input { Name = "Query", Type = Type.String, Description = "The input query for the Llama 2-13b API", IsRequired = true },
                new Input { Name = "MaxNewTokens", Type = Type.Number, Description = "Maximum new tokens to be generated (Max tokens 4096)", IsRequired = false},
                new Input { Name = "TopP", Type = Type.Number, Description = "Top P value for controlling randomness", IsRequired = false },
                new Input { Name = "Temperature", Type = Type.Number, Description = "Temperature value for controlling creativity", IsRequired = false },
                new Input { Name = "StopWords", Type = Type.String, IsList = true, Description = "List of words where the AI should stop generating text", IsRequired = false },
                new Input { Name = "ReturnFullText", Type = Type.Boolean, Description = "Whether to include the input prompt in the API response", IsRequired = false }
            };
            Outputs = new List<Output>
            {
                new Output { Name = "ApiResponse", Type = Type.String, Description = "The API response from Llama 2-70b" }
            };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            var query = inputs[0]?.ToString() ?? string.Empty;
            var maxNewTokens = inputs.Count > 1 && inputs[1] != null ? Convert.ToInt32(inputs[1]) : 1024; // default value if null
            var topP = inputs.Count > 2 && inputs[2] != null ? Convert.ToDouble(inputs[2]) : 0.8; // default value if null
            var temperature = inputs.Count > 3 && inputs[3] != null ? Convert.ToDouble(inputs[3]) : 0.6; // default value if null
            var returnFullText = inputs.Count > 5 && inputs[5] != null ? Convert.ToBoolean(inputs[5]) : false; // default value if null

            // Safely handling the stopwords input
            var stopWordsInput = inputs.Count > 4 ? inputs[4] : null;
            var stopWords = new List<string>();
            if (stopWordsInput != null)
            {
                if (stopWordsInput is List<object> stopWordsObjectList)
                {
                    stopWords = stopWordsObjectList.Select(obj => obj.ToString()).ToList();
                }
            }

            var responseString = await CallLlama270bApiAsync(query, stopWords, maxNewTokens, topP, temperature, returnFullText);
            programStructure.InputValues[Outputs[0].Id] = responseString;
        }

        private async Task<string> CallLlama270bApiAsync(string query, List<string> stopWords, int maxNewTokens = 1024, double topP = 0.8, double temperature = 0.6, bool returnFullText = false)
        {
            var url = "https://api-inference.huggingface.co/models/meta-llama/Llama-2-70b-chat-hf";
            object parameters;
            if (stopWords.Count == 0 || (stopWords.Count == 1 && stopWords[0] == ""))
            {
                parameters = new
                {
                    max_new_tokens = maxNewTokens,
                    top_p = topP,
                    temperature = temperature,
                    return_full_text = returnFullText
                };
            }
            else
            {
                parameters = new
                {
                    max_new_tokens = maxNewTokens,
                    top_p = topP,
                    temperature = temperature,
                    return_full_text = returnFullText,
                    stop = stopWords
                };
            }
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

    public class CodeLlama34b : Block
    {
        public CodeLlama34b()
        {
            Id = Guid.NewGuid();
            Name = "CodeLlama 34b";
            Description = "CodeLlama 34b API Integration";
            Inputs = new List<Input>
        {
            new Input { Name = "Query", Type = Type.String, Description = "The input query for the CodeLlama 34b API", IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Name = "ApiResponse", Type = Type.String, Description = "The API response from CodeLlama 34b" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            var query = inputs[0].ToString();
            var responseString = await CallCodeLlama34bApiAsync(query);
            programStructure.InputValues[Outputs[0].Id] = responseString;
        }

        private async Task<string> CallCodeLlama34bApiAsync(string query)
        {
            var url = "https://api-inference.huggingface.co/models/codellama/CodeLlama-34b-Instruct-hf";
            var payload = new
            {
                inputs = query
            };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var client = new HttpClient();

            // Replace {API_TOKEN} with your actual API token
            client.DefaultRequestHeaders.Add("Authorization", "Bearer {API_TOKEN}");

            var response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                // Assuming the response format is similar to the previous API
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
