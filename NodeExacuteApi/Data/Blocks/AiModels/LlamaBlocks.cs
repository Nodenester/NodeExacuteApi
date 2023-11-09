using LLama.Common;
using LLama;
using Type = NodeBaseApi.Version2.Type;
using NodeBaseApi.Version2;
using LLama.Abstractions;
using Azure;

namespace NodeExacuteApi.Data.Blocks.AiModels
{
    public class LlamaBlock : Block
    {
        public LlamaBlock()
        {
            Id = Guid.NewGuid();
            Name = "LLaMa Block";
            Description = "Generates text using the LLaMa models";

            Inputs = new List<Input>
        {
            new Input { Name = "Prompt", Type = Type.String, IsRequired = true, Description = "The text prompt to generate from" },
                // Add other inputs as necessary
        };

            Outputs = new List<Output>
        {
            new Output { Name = "Generated Text", Type = Type.String, Description = "The generated text" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            string modelPath = "<Your model path>"; 
            var prompt = inputs[0].ToString();

            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 1024,
                Seed = 1337,
                GpuLayerCount = 5
            };

            using var model = LLamaWeights.LoadFromFile(parameters);
            using var context = model.CreateContext(parameters);
            var ex = new InteractiveExecutor(context);
            ChatSession session = new ChatSession(ex);

            // Initialize a list to hold all responses
            var allResponses = new List<string>();

            // Use await foreach to collect all responses
            await foreach (var response in session.ChatAsync(prompt, new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "User:" } }))
            {
                allResponses.Add(response);
            }

            string combinedResponse = string.Join(" ", allResponses);

            programStructure.InputValues[Outputs[0].Id] = combinedResponse;
        }
    }
}
