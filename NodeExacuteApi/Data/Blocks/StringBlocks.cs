using Newtonsoft.Json;
using NodeBaseApi.Version2;
using Type = NodeBaseApi.Version2.Type;

namespace NodeExacuteApi.Data.Blocks
{
    public class Concatenation : Block
    {
        public Concatenation()
        {
            Id = Guid.NewGuid();
            Name = "Concatenation";
            Description = "A block that concatenates two strings.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "String 1", Description = "The first string.", Type = Type.String, IsRequired = true },
            new Input { Id = Guid.NewGuid(), Name = "String 2", Description = "The second string.", Type = Type.String, IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Result", Description = "The concatenated string.", Type = Type.String }
        };
        }

        public override List<object> Execute(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var result = inputs[0].ToString() + inputs[1].ToString();
            programStructure.InputValues[Outputs[0].Id] = result;
            return new List<object> { result };
        }
    }

    public class Length : Block
    {
        public Length()
        {
            Id = Guid.NewGuid();
            Name = "Length";
            Description = "A block that returns the length of a string.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Input String", Description = "The input string.", Type = Type.String, IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Length", Description = "The length of the input string.", Type = Type.Number }
        };
        }

        public override List<object> Execute(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            string inputString = inputs[0] as string;
            if (inputString != null)
            {
                var length = inputString.Length;
                programStructure.InputValues[Outputs[0].Id] = length;
                return new List<object> { length };
            }
            else
            {
                throw new InvalidOperationException("Expected a string input but received a different type or null.");
            }
;
        }
    }

    public class Substring : Block
    {
        public Substring()
        {
            Id = Guid.NewGuid();
            Name = "Substring";
            Description = "A block that returns a substring from a given string.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Input String", Description = "The input string.", Type = Type.String, IsRequired = true },
            new Input { Id = Guid.NewGuid(), Name = "Start Index", Description = "The starting position of the substring.", Type = Type.Number, IsRequired = true },
            new Input { Id = Guid.NewGuid(), Name = "Length", Description = "The length of the substring.", Type = Type.Number, IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Substring", Description = "The extracted substring.", Type = Type.String }
        };
        }

        public override List<object> Execute(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var substring = ((string)inputs[0]).Substring((int)inputs[1], (int)inputs[2]);
            programStructure.InputValues[Outputs[0].Id] = substring;
            return new List<object> { substring };
        }
    }

    public class Lowercase : Block
    {
        public Lowercase()
        {
            Id = Guid.NewGuid();
            Name = "Lowercase";
            Description = "Converts a string to lowercase.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Input String", Description = "The input string.", Type = Type.String, IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Lowercase String", Description = "The converted lowercase string.", Type = Type.String }
        };
        }

        public override List<object> Execute(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var result = ((string)inputs[0]).ToLower();
            programStructure.InputValues[Outputs[0].Id] = result;
            return new List<object> { result };
        }
    }

    public class Uppercase : Block
    {
        public Uppercase()
        {
            Id = Guid.NewGuid();
            Name = "Uppercase";
            Description = "Converts a string to uppercase.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Input String", Description = "The input string.", Type = Type.String, IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Uppercase String", Description = "The converted uppercase string.", Type = Type.String }
        };
        }

        public override List<object> Execute(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var result = ((string)inputs[0]).ToUpper();
            programStructure.InputValues[Outputs[0].Id] = result;
            return new List<object> { result };
        }
    }

    public class Contains : Block
    {
        public Contains()
        {
            Id = Guid.NewGuid();
            Name = "Contains";
            Description = "Checks if a string contains a specified substring.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Input String", Description = "The input string.", Type = Type.String, IsRequired = true },
            new Input { Id = Guid.NewGuid(), Name = "Substring", Description = "The substring to search for.", Type = Type.String, IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Contains", Description = "Whether the string contains the substring.", Type = Type.Boolean }
        };
        }

        public override List<object> Execute(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var contains = inputs[0].ToString().Contains(inputs[1].ToString());
            programStructure.InputValues[Outputs[0].Id] = contains;
            return new List<object> { contains };
        }
    }

}
