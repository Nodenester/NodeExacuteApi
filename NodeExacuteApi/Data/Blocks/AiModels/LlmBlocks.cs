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
            new Input { Name = "MaxNewTokens", Type = Type.Number, Description = "Maximum new tokens to be generated (Max tokens 4096)", IsRequired = true},
            new Input { Name = "TopP", Type = Type.Number, Description = "Top P value for controlling randomness", IsRequired = true },
            new Input { Name = "Temperature", Type = Type.Number, Description = "Temperature value for controlling creativity", IsRequired = true },
            new Input { Name = "StopWords", Type = Type.String, IsList = true, Description = "List of words where the AI should stop generating text", IsRequired = false }
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

            var responseString = await CallLlama27bApiAsync(query, stopWords, maxNewTokens, topP, temperature);
            programStructure.InputValues[Outputs[0].Id] = responseString;
        }

        private async Task<string> CallLlama27bApiAsync(string query, List<string> stopWords, int maxNewTokens = 1024, double topP = 0.8, double temperature = 0.6)
        {
            var url = "https://api-inference.huggingface.co/models/meta-llama/Llama-2-7b-chat-hf";
            var parameters = new
            {
                max_new_tokens = maxNewTokens,
                top_p = topP,
                temperature = temperature,
                return_full_text = false,  // To prevent the response from including the input prompt
                stop = stopWords
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
            new Input { Name = "MaxNewTokens", Type = Type.Number, Description = "Maximum new tokens to be generated (Max tokens 4096)", IsRequired = true},
            new Input { Name = "TopP", Type = Type.Number, Description = "Top P value for controlling randomness", IsRequired = true },
            new Input { Name = "Temperature", Type = Type.Number, Description = "Temperature value for controlling creativity", IsRequired = true },
            new Input { Name = "StopWords", Type = Type.String, IsList = true, Description = "List of words where the AI should stop generating text", IsRequired = false }
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

            var responseString = await CallLlama213bApiAsync(query, stopWords, maxNewTokens, topP, temperature);
            programStructure.InputValues[Outputs[0].Id] = responseString;
        }

        private async Task<string> CallLlama213bApiAsync(string query, List<string> stopWords, int maxNewTokens = 1024, double topP = 0.8, double temperature = 0.6 )
        {
            var url = "https://api-inference.huggingface.co/models/meta-llama/Llama-2-13b-chat-hf";
            var parameters = new
            {
                max_new_tokens = maxNewTokens,
                top_p = topP,
                temperature = temperature,
                return_full_text = false,  // To prevent the response from including the input prompt
                stop = stopWords ?? new List<string>() // Adding stop words parameter
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
            new Input { Name = "MaxNewTokens", Type = Type.Number, Description = "Maximum new tokens to be generated (Max tokens 4096)", IsRequired = true},
            new Input { Name = "TopP", Type = Type.Number, Description = "Top P value for controlling randomness", IsRequired = true },
            new Input { Name = "Temperature", Type = Type.Number, Description = "Temperature value for controlling creativity", IsRequired = true },
            new Input { Name = "StopWords", Type = Type.String, IsList = true, Description = "List of words where the AI should stop generating text", IsRequired = false }
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

            var responseString = await CallLlama270bApiAsync(query, stopWords, maxNewTokens, topP, temperature);
            programStructure.InputValues[Outputs[0].Id] = responseString;
        }


        private async Task<string> CallLlama270bApiAsync(string query, List<string> stopWords, int maxNewTokens = 1024, double topP = 0.8, double temperature = 0.6)
        {
            var url = "https://api-inference.huggingface.co/models/meta-llama/Llama-2-70b-chat-hf";
            var parameters = new
            {
                max_new_tokens = maxNewTokens,
                top_p = topP,
                temperature = temperature,
                return_full_text = false,  // To prevent the response from including the input prompt
                stop = stopWords
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

    public class CodeLlama34b : Block
    {
        public CodeLlama34b()
        {
            Id = Guid.NewGuid();
            Name = "CodeLlama 34b";
            Description = "CodeLlama 34b API Integration";
            Inputs = new List<Input>
        {
            new Input { Name = "Query", Type = Type.String, Description = "The input query for the CodeLlama 34b API", IsRequired = true },
            new Input { Name = "MaxTokens", Type = Type.Number, Description = "Maximum number of tokens to be generated (Max tokens 16384)", IsRequired = false },
            new Input { Name = "Temperature", Type = Type.Number, Description = "Temperature for controlling creativity", IsRequired = false },
            new Input { Name = "TopP", Type = Type.Number, Description = "Top P value for controlling randomness", IsRequired = false },
            new Input { Name = "StopSequence", Type = Type.String, Description = "Sequence where the AI should stop generating text", IsRequired = false }
        };
            Outputs = new List<Output>
        {
            new Output { Name = "ApiResponse", Type = Type.String, Description = "The API response from CodeLlama 34b (start \\begin{code}, end \\end{code})" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            var query = inputs[0].ToString();
            var maxTokens = inputs.Count > 1 && inputs[1] != null ? Convert.ToInt32(inputs[1]) : 1024; // Default value
            var temperature = inputs.Count > 2 && inputs[2] != null ? Convert.ToDouble(inputs[2]) : 0.7; // Default value
            var topP = inputs.Count > 3 && inputs[3] != null ? Convert.ToDouble(inputs[3]) : 0.9; // Default value
            var stopSequence = inputs.Count > 4 && inputs[4] != null ? inputs[4].ToString() : null;

            var responseString = await CallCodeLlama34bApiAsync(query, maxTokens, temperature, topP, stopSequence);
            programStructure.InputValues[Outputs[0].Id] = responseString;
        }

        private async Task<string> CallCodeLlama34bApiAsync(string query, int maxTokens, double temperature, double topP, string stopSequence)
        {
            var url = "https://api-inference.huggingface.co/models/codellama/CodeLlama-34b-Instruct-hf";
            var parameters = new
            {
                max_tokens = maxTokens,
                temperature = temperature,
                top_p = topP,
                stop_sequence = stopSequence
            };
            var payload = new
            {
                inputs = query,
                parameters
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
