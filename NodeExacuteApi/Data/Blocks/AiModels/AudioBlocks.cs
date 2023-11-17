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
    public class Transcription : Block
    {
        public Transcription()
        {
            Id = Guid.NewGuid();
            Name = "Transcription";
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

    public class TextToSpeech : Block
    {
        public TextToSpeech()
        {
            Id = Guid.NewGuid();
            Name = "Text to Speech";
            Description = "This block converts text to speech using the Hugging Face Suno Bark API, supporting multiple languages and voice presets.";
            Inputs = new List<Input>
        {
            new Input { Name = "Text", Type = Type.String, IsList = false, Description = "Text input for speech synthesis" },
            new Input { Name = "VoicePreset", Type = Type.String, IsList = false, Description = "Optional voice preset for speech synthesis" }
        };
            Outputs = new List<Output>
        {
            new Output { Name = "AudioData", Type = Type.Audio, IsList = false, Description = "Generated speech audio data" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            string textInput = inputs[0].ToString();
            string voicePreset = inputs.Count > 1 ? inputs[1].ToString() : null;

            byte[] audioData = await CallBarkApiAsync(textInput, voicePreset);
            programStructure.InputValues[Outputs[0].Id] = audioData;
        }

        private async Task<byte[]> CallBarkApiAsync(string text, string voicePreset)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_HF_TOKEN_HERE");

            var data = new
            {
                inputs = text,
                voice_preset = voicePreset
            };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api-inference.huggingface.co/models/suno/bark", content);

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
        public MusicGeneration()
        {
            Id = Guid.NewGuid();
            Name = "Music Generation";
            Description = "This block generates music based on textual prompts using the Hugging Face Music Generation API.";
            Inputs = new List<Input>
        {
            new Input { Name = "Prompt", Type = Type.String, IsList = false, Description = "Prompt describing the style or elements of the music" }
        };
            Outputs = new List<Output>
        {
            new Output { Name = "MusicData", Type = Type.Audio, IsList = false, Description = "Generated music audio data" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
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
            var response = await client.PostAsync("https://api-inference.huggingface.co/models/facebook/musicgen-stereo-medium", content);

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
        public SpeechEnhancement()
        {
            Id = Guid.NewGuid();
            Name = "Speech Enhancement";
            Description = "This block enhances speech quality using the speechbrain/mtl-mimic-voicebank model.";
            Inputs = new List<Input>
        {
            new Input { Name = "AudioData", Type = Type.Audio, IsList = false, Description = "Audio data for enhancement" }
        };
            Outputs = new List<Output>
        {
            new Output { Name = "EnhancedAudio", Type = Type.Audio, IsList = false, Description = "Enhanced audio data" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            if (inputs[0] is byte[] audioData)
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
