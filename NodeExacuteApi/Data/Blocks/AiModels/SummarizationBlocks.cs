using NodeBaseApi.Version2;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Type = NodeBaseApi.Version2.Type;
using System.Net.Http.Headers;

namespace NodeExacuteApi.Data.Blocks.AiModels
{
    public class SummarizeBlock : Block
    {
        public SummarizeBlock()
        {
            Id = Guid.NewGuid();
            Name = "Summarize Block";
            Description = "Summarize text using Bart Large CNN API";
            Inputs = new List<Input>
        {
            new Input { Name = "TextToSummarize", Type = Type.String, Description = "The text input for the Bart Large CNN summarization API" }
        };
            Outputs = new List<Output>
        {
            new Output { Name = "Summary", Type = Type.String, Description = "The summarized text from Bart Large CNN API" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            var textToSummarize = inputs[0].ToString();
            var summarizedText = await CallBartLargeCnnApiAsync(textToSummarize);
            programStructure.InputValues[Outputs[0].Id] = summarizedText;
        }

        private async Task<string> CallBartLargeCnnApiAsync(string textToSummarize)
        {
            var apiUrl = "https://api-inference.huggingface.co/models/facebook/bart-large-cnn";
            var content = new StringContent(JsonConvert.SerializeObject(new { inputs = textToSummarize }), Encoding.UTF8, "application/json");
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_HF_TOKEN_HERE");

            var response = await client.PostAsync(apiUrl, content);
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
