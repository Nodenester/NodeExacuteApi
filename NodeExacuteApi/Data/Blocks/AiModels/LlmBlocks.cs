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
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            programStructure.HasTokens(7);
            programStructure.CurrentPrizing += 7;
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

    public class Zephyr7b : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            programStructure.HasTokens(10);
            programStructure.CurrentPrizing += 10;
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

            var responseString = await CallZephyr7bApiAsync(query, stopWords, maxNewTokens, topP, temperature, returnFullText);
            programStructure.InputValues[Outputs[0].Id] = responseString;
        }

        private async Task<string> CallZephyr7bApiAsync(string query, List<string> stopWords, int maxNewTokens = 1024, double topP = 0.8, double temperature = 0.6, bool returnFullText = false)
        {
            var url = "https://api-inference.huggingface.co/models/HuggingFaceH4/zephyr-7b-beta";
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
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            programStructure.HasTokens(15);
            programStructure.CurrentPrizing += 15;
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
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            programStructure.HasTokens(25);
            programStructure.CurrentPrizing += 25;
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
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            programStructure.HasTokens(15);
            programStructure.CurrentPrizing += 15;
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
