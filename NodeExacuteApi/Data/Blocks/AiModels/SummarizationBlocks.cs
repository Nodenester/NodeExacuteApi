using NodeBaseApi.Version2;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Type = NodeBaseApi.Version2.Type;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace NodeExacuteApi.Data.Blocks.AiModels
{
    public class Summarize : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            var textToSummarize = inputs[0].ToString();
            var summarizedText = await CallBartLargeCnnApiAsync(textToSummarize);

            var tokens = programStructure.CountTokens(summarizedText + textToSummarize);
            programStructure.HasTokens(tokens * 0.01);
            programStructure.CurrentPrizing += (int)(tokens * 0.01);

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

    public class TextToText : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            string textInput = inputs[0].ToString();
            string textOutput = await CallFlanT5XxlApiAsync(textInput);

            var tokens = programStructure.CountTokens(textOutput + textInput);
            programStructure.HasTokens(tokens * 0.01);
            programStructure.CurrentPrizing += (int)(tokens * 0.01);

            programStructure.InputValues[Outputs[0].Id] = textOutput;
        }

        private async Task<string> CallFlanT5XxlApiAsync(string text)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_HF_TOKEN_HERE");

            var data = new { inputs = text };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api-inference.huggingface.co/models/google/flan-t5-xxl", content);

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

    public class FactualConsistency : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            string text1 = inputs[0].ToString();
            string text2 = inputs[1].ToString();
            string concatenatedText = $"{text1} [SEP] {text2}";

            double consistencyScore = await CallFactualConsistencyApiAsync(concatenatedText);

            var tokens = programStructure.CountTokens(text1 + text2);
            programStructure.HasTokens(tokens * 0.01);
            programStructure.CurrentPrizing += (int)(tokens * 0.01);

            programStructure.InputValues[Outputs[0].Id] = consistencyScore;
        }

        private async Task<double> CallFactualConsistencyApiAsync(string text)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_HF_TOKEN_HERE");

            var data = new { inputs = text };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api-inference.huggingface.co/models/vectara/hallucination_evaluation_model", content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseString);
                return result.probability;
            }
            else
            {
                throw new Exception($"API call failed: {response.StatusCode}");
            }
        }
    }

}
