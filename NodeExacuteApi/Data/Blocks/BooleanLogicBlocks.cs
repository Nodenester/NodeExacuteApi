using Newtonsoft.Json;
using NodeBaseApi.Version2;
using Type = NodeBaseApi.Version2.Type;

namespace NodeExacuteApi.Data.Blocks
{
    public class AndBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            // Start with the assumption that the result is true
            bool result = true;

            // Iterate through all inputs and apply the AND operation
            foreach (var input in inputs)
            {
                if (input is bool inputBool)
                {
                    result = result && inputBool;
                }
                else
                {
                    // Handle the case where the input is not a boolean
                    // This could be an error or a default behavior
                }
            }

            // Store the result in the program structure
            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class OrBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = (bool)inputs[0] || (bool)inputs[1];
            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class NotBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = !(bool)inputs[0];
            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class XorBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = (bool)inputs[0] ^ (bool)inputs[1];
            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    //new ones
    public class NandBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = !((bool)inputs[0] && (bool)inputs[1]);
            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class NorBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = !((bool)inputs[0] || (bool)inputs[1]);
            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class BooleanEquality : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = (bool)inputs[0] == (bool)inputs[1];
            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class BooleanToggle : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = !(bool)inputs[0];
            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class BooleanToString : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            string result = ((bool)inputs[0]).ToString();
            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class StringToBoolean : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result;
            Boolean.TryParse(inputs[0].ToString(), out result);
            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class BooleanToInteger : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            int result = (bool)inputs[0] ? 1 : 0;
            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }
}
