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
    public class WhisperBlock : Block
    {
        public Whisper3Block()
        {
            Id = Guid.NewGuid();
            Name = "Whisper Block";
            Description = "This block processes audio data using the Whisper Large V3 API and returns the transcription.";
            Inputs = new List<Input>
        {
            new Input { Name = "AudioData", Type = Type.Audio, IsList = false, Description = "Audio data for processing" }
        };
            Outputs = new List<Output>
        {
            new Output { Name = "Transcription", Type = Type.String, IsList = false, Description = "Transcribed audio text" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            if (inputs[0] is byte[] audioData)
            {
                var transcription = await CallWhisperLargeV3ApiAsync(audioData);
                programStructure.InputValues[Outputs[0].Id] = transcription;
            }
            else
            {
                throw new ArgumentException("Invalid input type. Expected a byte array representing audio data.");
            }
        }

        private async Task<string> CallWhisperLargeV3ApiAsync(byte[] audioData)
        {
            var apiUrl = "https://api-inference.huggingface.co/models/openai/whisper-large-v3";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_HF_TOKEN_HERE");

            using (var content = new ByteArrayContent(audioData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("audio/flac");
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
}
