using System;
using System.Collections.Generic;
using System.IO;
using NodeBaseApi.Version2;
using Type = NodeBaseApi.Version2.Type;

namespace NodeExacuteApi.Data.Blocks
{
    public class GreaterThanBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = Convert.ToDouble(inputs[0]) > Convert.ToDouble(inputs[1]);
            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class LessThanBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = Convert.ToDouble(inputs[0]) < Convert.ToDouble(inputs[1]);
            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class EqualToBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = Convert.ToDouble(inputs[0]) == Convert.ToDouble(inputs[1]);
            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class NotEqualToBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = Convert.ToDouble(inputs[0]) != Convert.ToDouble(inputs[1]);
            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class GreaterThanOrEqualToBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = Convert.ToDouble(inputs[0]) >= Convert.ToDouble(inputs[1]);
            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class LessThanOrEqualToBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = Convert.ToDouble(inputs[0]) <= Convert.ToDouble(inputs[1]);
            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }
}
