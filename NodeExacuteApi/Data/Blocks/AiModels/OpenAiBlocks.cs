using NodeBaseApi.Version2;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using Type = NodeBaseApi.Version2.Type;

namespace NodeExacuteApi.Data.Blocks.AiModels
{
    //Gpt
    public class GPTBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var apiKey = inputs[0].ToString();
            var prompt = inputs[1].ToString();
            var maxTokens = (int)inputs[2];

            var api = new OpenAI_API.OpenAIAPI(apiKey);
            //var result = api.Completions.(prompt).Result;

            string model = "";
            if (variableid.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                model = Model.DefaultModel;
            }
            else if (variableid.ToString() == "10000000-0000-0000-0000-000000000000")
            {
                model = Model.ChatGPTTurbo;
            }
            else if (variableid.ToString() == "20000000-0000-0000-0000-000000000000")
            {
                model = Model.DavinciText;
            }
            else if (variableid.ToString() == "30000000-0000-0000-0000-000000000000")
            {
                model = Model.DavinciCode;
            }
            else if (variableid.ToString() == "40000000-0000-0000-0000-000000000000")
            {
                model = Model.GPT4;
            }
            else if (variableid.ToString() == "50000000-0000-0000-0000-000000000000")
            {
                model = Model.GPT4_32k_Context;
            }
            else if (variableid.ToString() == "60000000-0000-0000-0000-000000000000")
            {
                model = Model.AdaText;
            }
            else if (variableid.ToString() == "70000000-0000-0000-0000-000000000000")
            {
                model = Model.AdaTextEmbedding;
            }
            else if (variableid.ToString() == "80000000-0000-0000-0000-000000000000")
            {
                model = Model.BabbageText;
            }
            else if (variableid.ToString() == "90000000-0000-0000-0000-000000000000")
            {
                model = Model.TextModerationLatest;
            }

            var result = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
            {
                Model = model,
                MaxTokens = maxTokens,
                Messages = new ChatMessage[] {
                new ChatMessage(ChatMessageRole.User, prompt)
            }
            });

            programStructure.InputValues[Outputs[0].Id] = result.Object;
        }
    }

    //public class WhisperASRBlock : Block
    //{
    //    public WhisperASRBlock()
    //    {
    //        Id = Guid.NewGuid();
    //        Name = "Whisper ASR Block";
    //        Description = "Transcribes audio using the Whisper ASR model";

    //        Inputs = new List<Input>
    //    {
    //        new Input { Name = "API Key", Type = Type.String, IsRequired = true, Description = "Your OpenAI API key" },
    //        new Input { Name = "Audio", Type = Type.Audio, IsRequired = true, Description = "The audio data to transcribe" }
    //    };

    //        Outputs = new List<Output>
    //    {
    //        new Output { Name = "Transcription", Type = Type.String, Description = "The transcribed text" }
    //    };
    //    }

    //    public async override Task<List<object>> ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
    //    {
    //        var apiKey = inputs[0].ToString();
    //        var audioData = (byte[])inputs[1];

    //        var api = new OpenAI_API.OpenAIAPI(apiKey);

    //        // Assuming the API has a method to transcribe audio
    //        var result = await api.TranscribeAudioAsync(audioData);

    //        var transcription = result.Transcription;

    //        programStructure.InputValues[Outputs[0].Id] = transcription;
    //        return new List<object> { transcription };
    //    }
    //}

    //public class CLIPBlock : Block
    //{
    //    public CLIPBlock()
    //    {
    //        Id = Guid.NewGuid();
    //        Name = "CLIP Block";
    //        Description = "Uses CLIP model to provide similarity scores between text and images";

    //        Inputs = new List<Input>
    //    {
    //        new Input { Name = "API Key", Type = Type.String, IsRequired = true, Description = "Your OpenAI API key" },
    //        new Input { Name = "Text", Type = Type.String, IsRequired = true, Description = "Text to compare" },
    //        new Input { Name = "Image", Type = Type.Picture, IsRequired = true, Description = "Image to compare" }
    //    };

    //        Outputs = new List<Output>
    //    {
    //        new Output { Name = "Similarity Score", Type = Type.Number, Description = "Similarity score between text and image" }
    //    };
    //    }

    //    public override List<object> ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
    //    {
    //        var apiKey = inputs[0].ToString();
    //        var text = inputs[1].ToString();
    //        var imageData = (byte[])inputs[2];

    //        var api = new OpenAI_API.OpenAIAPI(apiKey);
    //        var result = api(text, imageData).Result;  // This is a speculative method call, adjust according to actual API documentation

    //        var similarityScore = result.Score;

    //        return new List<object> { similarityScore };
    //    }
    //}
    //public class JukeboxBlock : Block
    //{
    //    public JukeboxBlock()
    //    {
    //        Id = Guid.NewGuid();
    //        Name = "Jukebox Block";
    //        Description = "Generates music using the Jukebox model";

    //        Inputs = new List<Input>
    //    {
    //        new Input { Name = "API Key", Type = Type.String, IsRequired = true, Description = "Your OpenAI API key" },
    //        new Input { Name = "Prompt", Type = Type.String, IsRequired = true, Description = "The text prompt to generate music from" }
    //    };

    //        Outputs = new List<Output>
    //    {
    //        new Output { Name = "Generated Music", Type = Type.Audio, Description = "The generated music" }
    //    };
    //    }

    //    public override List<object> ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
    //    {
    //        var apiKey = inputs[0].ToString();
    //        var prompt = inputs[1].ToString();

    //        var api = new OpenAI_API.OpenAIAPI(apiKey);
    //        var result = api.JukeboxAsync(prompt).Result;  // This is a speculative method call, adjust according to actual API documentation

    //        var generatedMusic = Convert.FromBase64String(result.Music);

    //        return new List<object> { generatedMusic };
    //    }
    //}
}
