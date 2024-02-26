using NodeBaseApi.Version2;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Type = NodeBaseApi.Version2.Type;
using System.Net.Http.Headers;
using System.Collections;
using System.Text.Json;

namespace NodeExacuteApi.Data.Blocks.AiModels
{
    public class Transcription : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            byte[] audioData = null;

            if (inputs[0] is byte[] byteArray)
            {
                audioData = byteArray;
            }
            else if (inputs[0].ToString() is string base64EncodedString)
            {
                try
                {
                    audioData = Convert.FromBase64String(base64EncodedString);
                }
                catch (FormatException e)
                {
                    throw new ArgumentException("Input string is not a valid Base64 encoded byte array.", e);
                }
            }
            else
            {
                throw new ArgumentException("Invalid input type. Expected a byte array or a Base64 encoded string representing audio data.");
            }

            if (audioData != null)
            {
                var transcription = await CallWhisperLargeV3ApiAsync(audioData);

                var tokens = programStructure.CountTokens(transcription);
                programStructure.HasTokens(tokens * 0.01);
                programStructure.CurrentPrizing += (int)(tokens * 0.01);

                programStructure.InputValues[Outputs[0].Id] = transcription;
            }
            else
            {
                throw new ArgumentException("Invalid input type. Expected a byte array representing audio data.");
            }
        }

        private async Task<string> CallWhisperLargeV3ApiAsync(byte[] audioData)
        {
            var apiUrl = "https://api-inference.huggingface.co/models/openai/whisper-tiny";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_HF_TOKEN_HERE");

            using (var content = new ByteArrayContent(audioData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("audio/flac");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var parsedJson = JsonDocument.Parse(jsonResponse);
                    if (parsedJson.RootElement.TryGetProperty("text", out var textElement))
                    {
                        return textElement.GetString();
                    }
                    else
                    {
                        throw new Exception("The API response did not contain a 'text' field.");
                    }
                }
                else
                {
                    throw new Exception($"API call failed: {response.StatusCode}");
                }
            }
        }
    }

    public class TextToSpeech : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            string textInput = inputs[0].ToString();
            //string voicePreset = inputs.Count > 1 ? inputs[1].ToString() : null;

            byte[] audioData = await CallBarkApiAsync(textInput);

            var tokens = programStructure.CountTokens(textInput);
            programStructure.HasTokens(tokens * 0.02);
            programStructure.CurrentPrizing += (int)(tokens * 0.02);

            programStructure.InputValues[Outputs[0].Id] = audioData;
        }

        private async Task<byte[]> CallBarkApiAsync(string text)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_HF_TOKEN_HERE");

            var data = new
            {
                inputs = text,
            };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api-inference.huggingface.co/models/facebook/mms-tts-eng", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBytes = await response.Content.ReadAsByteArrayAsync();
                return responseBytes;
            }
            else
            {
                throw new Exception($"API call failed: {response.StatusCode}");
            }
        }
    }

    public class MusicGeneration : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            programStructure.HasTokens(20);
            programStructure.CurrentPrizing += 20;
            string prompt = inputs[0].ToString();
            byte[] musicData = await CallMusicGenApiAsync(prompt);
            programStructure.InputValues[Outputs[0].Id] = musicData;
        }

        private async Task<byte[]> CallMusicGenApiAsync(string prompt)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_HF_TOKEN_HERE");

            var data = new { inputs = prompt };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api-inference.huggingface.co/models/facebook/musicgen-small", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBytes = await response.Content.ReadAsByteArrayAsync();
                return responseBytes;
            }
            else
            {
                throw new Exception($"API call failed: {response.StatusCode}");
            }
        }
    }

    public class SpeechEnhancement : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            programStructure.HasTokens(20);
            programStructure.CurrentPrizing += 20;
            byte[] audioData = null;

            // Check if the input is a byte array and assign it directly.
            if (inputs[0] is byte[] byteArray)
            {
                audioData = byteArray;
            }
            // If the input is a string, assume it's Base64 encoded and try to convert it.
            else if (inputs[0] is string base64EncodedString)
            {
                try
                {
                    audioData = Convert.FromBase64String(base64EncodedString);
                }
                catch (FormatException e)
                {
                    // Handle the case where the string is not a valid Base64 string.
                    throw new ArgumentException("Input string is not a valid Base64 encoded byte array.", e);
                }
            }
            else
            {
                throw new ArgumentException("Invalid input type. Expected a byte array or a Base64 encoded string representing audio data.");
            }

            // Proceed with the audioData if it's not null.
            if (audioData != null)
            {
                byte[] enhancedAudio = await CallSpeechEnhancementApiAsync(audioData);
                programStructure.InputValues[Outputs[0].Id] = enhancedAudio;
            }
            else
            {
                throw new ArgumentException("Invalid input type. Expected a byte array representing audio data.");
            }
        }

        private async Task<byte[]> CallSpeechEnhancementApiAsync(byte[] audioData)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_HF_TOKEN_HERE");

            using (var content = new ByteArrayContent(audioData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
                var response = await client.PostAsync("https://api-inference.huggingface.co/models/speechbrain/mtl-mimic-voicebank", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(responseString);
                    return Convert.FromBase64String(result.blob);
                }
                else
                {
                    throw new Exception($"API call failed: {response.StatusCode}");
                }
            }
        }
    }
}
