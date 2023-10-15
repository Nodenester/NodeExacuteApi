using Newtonsoft.Json;
using NodeBaseApi.Version2;
using Type = NodeBaseApi.Version2.Type;

namespace NodeExacuteApi.Data.Blocks
{
    public class AndBlock : Block
    {
        public AndBlock()
        {
            this.Id = Guid.NewGuid();
            this.Name = "AndBlock";
            this.Description = "A block that performs a logical AND operation";
            this.Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Input 1", Description = "First input", Type = Type.Boolean, IsRequired = true },
            new Input { Id = Guid.NewGuid(), Name = "Input 2", Description = "Second input", Type = Type.Boolean, IsRequired = true }
        };
            this.Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Output", Description = "Output of the AND operation", Type = Type.Boolean }
        };
        }

        public override List<object> ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = (bool)inputs[0] && (bool)inputs[1];
            programStructure.InputValues[Outputs[0].Id] = result;
            return new List<object> { result };
        }
    }

    public class OrBlock : Block
    {
        public OrBlock()
        {
            this.Id = Guid.NewGuid();
            this.Name = "OrBlock";
            this.Description = "A block that performs a logical OR operation";
            this.Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Input 1", Description = "First input", Type = Type.Boolean, IsRequired = true },
            new Input { Id = Guid.NewGuid(), Name = "Input 2", Description = "Second input", Type = Type.Boolean, IsRequired = true }
        };
            this.Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Output", Description = "Output of the OR operation", Type = Type.Boolean }
        };
        }

        public override List<object> ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = (bool)inputs[0] || (bool)inputs[1];
            programStructure.InputValues[Outputs[0].Id] = result;
            return new List<object> { result };
        }
    }

    public class NotBlock : Block
    {
        public NotBlock()
        {
            this.Id = Guid.NewGuid();
            this.Name = "NotBlock";
            this.Description = "A block that performs a logical NOT operation";
            this.Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Input", Description = "Input", Type = Type.Boolean, IsRequired = true }
        };
            this.Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Output", Description = "Output of the NOT operation", Type = Type.Boolean }
        };
        }

        public override List<object> ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = !(bool)inputs[0];
            programStructure.InputValues[Outputs[0].Id] = result;
            return new List<object> { result };
        }
    }

    public class XorBlock : Block
    {
        public XorBlock()
        {
            this.Id = Guid.NewGuid();
            this.Name = "XorBlock";
            this.Description = "A block that performs a logical XOR operation";
            this.Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Input 1", Description = "First input", Type = Type.Boolean, IsRequired = true },
            new Input { Id = Guid.NewGuid(), Name = "Input 2", Description = "Second input", Type = Type.Boolean, IsRequired = true }
        };
            this.Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Output", Description = "Output of the XOR operation", Type = Type.Boolean }
        };
        }

        public override List<object> ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = (bool)inputs[0] ^ (bool)inputs[1];
            programStructure.InputValues[Outputs[0].Id] = result;
            return new List<object> { result };
        }
    }

    //new ones
    public class NandBlock : Block
    {
        public NandBlock()
        {
            this.Id = Guid.NewGuid();
            this.Name = "NandBlock";
            this.Description = "A block that performs a logical NAND operation";
            this.Inputs = new List<Input>
    {
        new Input { Id = Guid.NewGuid(), Name = "Input 1", Description = "First input", Type = Type.Boolean, IsRequired = true },
        new Input { Id = Guid.NewGuid(), Name = "Input 2", Description = "Second input", Type = Type.Boolean, IsRequired = true }
    };
            this.Outputs = new List<Output>
    {
        new Output { Id = Guid.NewGuid(), Name = "Output", Description = "Output of the NAND operation", Type = Type.Boolean }
    };
        }

        public override List<object> ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = !((bool)inputs[0] && (bool)inputs[1]);
            programStructure.InputValues[Outputs[0].Id] = result;
            return new List<object> { result };
        }
    }

    public class NorBlock : Block
    {
        public NorBlock()
        {
            this.Id = Guid.NewGuid();
            this.Name = "NorBlock";
            this.Description = "A block that performs a logical NOR operation";
            this.Inputs = new List<Input>
    {
        new Input { Id = Guid.NewGuid(), Name = "Input 1", Description = "First input", Type = Type.Boolean, IsRequired = true },
        new Input { Id = Guid.NewGuid(), Name = "Input 2", Description = "Second input", Type = Type.Boolean, IsRequired = true }
    };
            this.Outputs = new List<Output>
    {
        new Output { Id = Guid.NewGuid(), Name = "Output", Description = "Output of the NOR operation", Type = Type.Boolean }
    };
        }

        public override List<object> ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = !((bool)inputs[0] || (bool)inputs[1]);
            programStructure.InputValues[Outputs[0].Id] = result;
            return new List<object> { result };
        }
    }

    public class BooleanEqualityBlock : Block
    {
        public BooleanEqualityBlock()
        {
            this.Id = Guid.NewGuid();
            this.Name = "BooleanEqualityBlock";
            this.Description = "A block that checks if two boolean values are equal";
            this.Inputs = new List<Input>
    {
        new Input { Id = Guid.NewGuid(), Name = "Input 1", Description = "First input", Type = Type.Boolean, IsRequired = true },
        new Input { Id = Guid.NewGuid(), Name = "Input 2", Description = "Second input", Type = Type.Boolean, IsRequired = true }
    };
            this.Outputs = new List<Output>
    {
        new Output { Id = Guid.NewGuid(), Name = "Output", Description = "Result of the equality check", Type = Type.Boolean }
    };
        }

        public override List<object> ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = (bool)inputs[0] == (bool)inputs[1];
            programStructure.InputValues[Outputs[0].Id] = result;
            return new List<object> { result };
        }
    }

    public class BooleanToggleBlock : Block
    {
        public BooleanToggleBlock()
        {
            this.Id = Guid.NewGuid();
            this.Name = "BooleanToggleBlock";
            this.Description = "A block that toggles a boolean value";
            this.Inputs = new List<Input>
    {
        new Input { Id = Guid.NewGuid(), Name = "Input", Description = "Input value", Type = Type.Boolean, IsRequired = true }
    };
            this.Outputs = new List<Output>
    {
        new Output { Id = Guid.NewGuid(), Name = "Output", Description = "Toggled value", Type = Type.Boolean }
    };
        }

        public override List<object> ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result = !(bool)inputs[0];
            programStructure.InputValues[Outputs[0].Id] = result;
            return new List<object> { result };
        }
    }

    public class BooleanToStringBlock : Block
    {
        public BooleanToStringBlock()
        {
            this.Id = Guid.NewGuid();
            this.Name = "BooleanToStringBlock";
            this.Description = "A block that converts a boolean value to a string";
            this.Inputs = new List<Input>
    {
        new Input { Id = Guid.NewGuid(), Name = "Input", Description = "Input value", Type = Type.Boolean, IsRequired = true }
    };
            this.Outputs = new List<Output>
    {
        new Output { Id = Guid.NewGuid(), Name = "Output", Description = "String representation", Type = Type.String }
    };
        }

        public override List<object> ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            string result = ((bool)inputs[0]).ToString();
            programStructure.InputValues[Outputs[0].Id] = result;
            return new List<object> { result };
        }
    }

    public class StringToBooleanBlock : Block
    {
        public StringToBooleanBlock()
        {
            this.Id = Guid.NewGuid();
            this.Name = "StringToBooleanBlock";
            this.Description = "A block that converts a string to a boolean value";
            this.Inputs = new List<Input>
    {
        new Input { Id = Guid.NewGuid(), Name = "Input", Description = "Input string", Type = Type.String, IsRequired = true }
    };
            this.Outputs = new List<Output>
    {
        new Output { Id = Guid.NewGuid(), Name = "Output", Description = "Boolean value", Type = Type.Boolean }
    };
        }

        public override List<object> ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            bool result;
            Boolean.TryParse(inputs[0].ToString(), out result);
            programStructure.InputValues[Outputs[0].Id] = result;
            return new List<object> { result };
        }
    }

    public class BooleanToIntegerBlock : Block
    {
        public BooleanToIntegerBlock()
        {
            this.Id = Guid.NewGuid();
            this.Name = "BooleanToIntegerBlock";
            this.Description = "A block that converts a boolean value to an integer";
            this.Inputs = new List<Input>
            {
        new Input { Id = Guid.NewGuid(), Name = "Input", Description = "Input value", Type = Type.Boolean, IsRequired = true }
    };
            this.Outputs = new List<Output>
            {
        new Output { Id = Guid.NewGuid(), Name = "Output", Description = "Integer representation", Type = Type.Number }
    };
        }
        public override List<object> ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            int result = (bool)inputs[0] ? 1 : 0;
            programStructure.InputValues[Outputs[0].Id] = result;
            return new List<object> { result };
        }
    }
}
