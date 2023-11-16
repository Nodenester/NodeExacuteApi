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
    public class Llama27bBlock : Block
    {
        public Llama27bBlock()
        {
            Id = Guid.NewGuid();
            Name = "Llama2 7b Block";
            Description = "Llama 2-7b API";
            Inputs = new List<Input>
            {
                new Input { Name = "Query", Type = Type.String, Description = "The input query for the Llama 2-7b API" }
            };
                Outputs = new List<Output>
            {
                new Output { Name = "ApiResponse", Type = Type.String, Description = "The API response from Llama 2-7b" }
            };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            var query = inputs[0].ToString();
            var responseString = await CallLlama27bApiAsync(query);
            programStructure.InputValues[Outputs[0].Id] = responseString;
        }

        private async Task<string> CallLlama27bApiAsync(string query)
        {
            var url = "https://api-inference.huggingface.co/models/meta-llama/Llama-2-7b-chat-hf";
            var content = new StringContent(JsonConvert.SerializeObject(new { inputs = query }), Encoding.UTF8, "application/json");
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer YOUR_HF_TOKEN_HERE");

            var response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"API call failed: {response.StatusCode}");
            }
        }
    }

    public class Llama213bBlock : Block
    {
        public Llama213bBlock()
        {
            Id = Guid.NewGuid();
            Name = "Llama2 13b Block";
            Description = "Llama 2-13b API";
            Inputs = new List<Input>
        {
            new Input { Name = "Query", Type = Type.String, Description = "The input query for the Llama 2-13b API" }
        };
            Outputs = new List<Output>
        {
            new Output { Name = "ApiResponse", Type = Type.String, Description = "The API response from Llama 2-13b" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            var query = inputs[0].ToString();
            var responseString = await CallLlama213bApiAsync(query);
            programStructure.InputValues[Outputs[0].Id] = responseString;
        }

        private async Task<string> CallLlama213bApiAsync(string query)
        {
            var url = "https://api-inference.huggingface.co/models/meta-llama/Llama-2-13b-chat-hf";
            var content = new StringContent(JsonConvert.SerializeObject(new { inputs = query }), Encoding.UTF8, "application/json");
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer YOUR_HF_TOKEN_HERE");

            var response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"API call failed: {response.StatusCode}");
            }
        }
    }

    public class Llama270bBlock : Block
    {
        public Llama270bBlock()
        {
            Id = Guid.NewGuid();
            Name = "Llama2 70b Block";
            Description = "Llama 2-70b API";
            Inputs = new List<Input>
        {
            new Input { Name = "Query", Type = Type.String, Description = "The input query for the Llama 2-70b API" }
        };
            Outputs = new List<Output>
        {
            new Output { Name = "ApiResponse", Type = Type.String, Description = "The API response from Llama 2-70b" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            var query = inputs[0].ToString();
            var responseString = await CallLlama270bApiAsync(query);
            programStructure.InputValues[Outputs[0].Id] = responseString;
        }

        private async Task<string> CallLlama270bApiAsync(string query)
        {
            var url = "https://api-inference.huggingface.co/models/meta-llama/Llama-2-70b-chat-hf";
            var content = new StringContent(JsonConvert.SerializeObject(new { inputs = query }), Encoding.UTF8, "application/json");
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer YOUR_HF_TOKEN_HERE");

            var response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"API call failed: {response.StatusCode}");
            }
        }
    }
}
