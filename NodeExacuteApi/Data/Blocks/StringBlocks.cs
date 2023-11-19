using Newtonsoft.Json;
using NodeBaseApi.Version2;
using System.Text;
using System.Text.RegularExpressions;
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

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            StringBuilder resultBuilder = new StringBuilder();

            foreach (var input in inputs)
            {
                resultBuilder.Append(input?.ToString() ?? string.Empty);
            }

            string result = resultBuilder.ToString();
            programStructure.InputValues[Outputs[0].Id] = result;
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

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            string inputString = inputs[0].ToString();
            if (inputString != null)
            {
                var length = inputString.Length;
                programStructure.InputValues[Outputs[0].Id] = length;
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

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var substring = ((string)inputs[0]).Substring((int)inputs[1], (int)inputs[2]);
            programStructure.InputValues[Outputs[0].Id] = substring;
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

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var result = ((string)inputs[0]).ToLower();
            programStructure.InputValues[Outputs[0].Id] = result;
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

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var result = ((string)inputs[0]).ToUpper();
            programStructure.InputValues[Outputs[0].Id] = result;
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

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var contains = inputs[0].ToString().Contains(inputs[1].ToString());
            programStructure.InputValues[Outputs[0].Id] = contains;
        }
    }

    // new ones
    public class Split : Block
    {
        public Split()
        {
            Id = Guid.NewGuid();
            Name = "Split";
            Description = "A block that splits a string into an array of substrings based on a specified delimiter.";
            Inputs = new List<Input>
    {
        new Input { Id = Guid.NewGuid(), Name = "Input String", Description = "The input string.", Type = Type.String, IsRequired = true },
        new Input { Id = Guid.NewGuid(), Name = "Delimiter", Description = "The delimiter.", Type = Type.String, IsRequired = true }
    };
            Outputs = new List<Output>
    {
        new Output { Id = Guid.NewGuid(), Name = "Substrings", Description = "The array of substrings.", Type = Type.String, IsList = true }
    };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var substrings = inputs[0].ToString().Split(new string[] { inputs[1].ToString() }, StringSplitOptions.None).ToList();
            programStructure.InputValues[Outputs[0].Id] = substrings;
        }
    }

    public class Join : Block
    {
        public Join()
        {
            Id = Guid.NewGuid();
            Name = "Join";
            Description = "A block that concatenates an array of strings into a single string with a specified separator.";
            Inputs = new List<Input>
    {
        new Input { Id = Guid.NewGuid(), Name = "Array of Strings", Description = "The array of strings.", Type = Type.String, IsRequired = true, IsList = true },
        new Input { Id = Guid.NewGuid(), Name = "Separator", Description = "The separator.", Type = Type.String, IsRequired = true }
    };
            Outputs = new List<Output>
    {
        new Output { Id = Guid.NewGuid(), Name = "Resultant String", Description = "The concatenated string.", Type = Type.String }
    };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var strings = ((List<object>)inputs[0]).ConvertAll(input => input.ToString());
            var resultantString = string.Join(inputs[1].ToString(), strings);
            programStructure.InputValues[Outputs[0].Id] = resultantString;
        }
    }

    public class Replace : Block
    {
        public Replace()
        {
            Id = Guid.NewGuid();
            Name = "Replace";
            Description = "A block that replaces occurrences of a specified substring with another substring.";
            Inputs = new List<Input>
    {
        new Input { Id = Guid.NewGuid(), Name = "Input String", Description = "The input string.", Type = Type.String, IsRequired = true },
        new Input { Id = Guid.NewGuid(), Name = "Search String", Description = "The substring to replace.", Type = Type.String, IsRequired = true },
        new Input { Id = Guid.NewGuid(), Name = "Replacement String", Description = "The replacement string.", Type = Type.String, IsRequired = true }
    };
            Outputs = new List<Output>
    {
        new Output { Id = Guid.NewGuid(), Name = "Resultant String", Description = "The resultant string.", Type = Type.String }
    };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var resultantString = inputs[0].ToString().Replace(inputs[1].ToString(), inputs[2].ToString());
            programStructure.InputValues[Outputs[0].Id] = resultantString;
        }
    }

    public class Trim : Block
    {
        public Trim()
        {
            Id = Guid.NewGuid();
            Name = "Trim";
            Description = "A block that removes all leading and trailing white-space characters from the current string.";
            Inputs = new List<Input>
    {
        new Input { Id = Guid.NewGuid(), Name = "Input String", Description = "The input string.", Type = Type.String, IsRequired = true }
    };
            Outputs = new List<Output>
    {
        new Output { Id = Guid.NewGuid(), Name = "Trimmed String", Description = "The trimmed string.", Type = Type.String }
    };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var trimmedString = inputs[0].ToString().Trim();
            programStructure.InputValues[Outputs[0].Id] = trimmedString;
        }
    }

    public class IndexOf : Block
    {
        public IndexOf()
        {
            Id = Guid.NewGuid();
            Name = "IndexOf";
            Description = "A block that returns the index of the first occurrence of a specified substring.";
            Inputs = new List<Input>
    {
        new Input { Id = Guid.NewGuid(), Name = "Input String", Description = "The input string.", Type = Type.String, IsRequired = true },
        new Input { Id = Guid.NewGuid(), Name = "Search String", Description = "The substring to search for.", Type = Type.String, IsRequired = true }
    };
            Outputs = new List<Output>
    {
        new Output { Id = Guid.NewGuid(), Name = "Index", Description = "The index of the first occurrence.", Type = Type.Number }
    };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var index = inputs[0].ToString().IndexOf(inputs[1].ToString());
            programStructure.InputValues[Outputs[0].Id] = index;
        }
    }

    public class RegularExpressionMatch : Block
    {
        public RegularExpressionMatch()
        {
            Id = Guid.NewGuid();
            Name = "RegularExpressionMatch";
            Description = "A block that checks if a string matches a specified regular expression pattern.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Input String", Description = "The input string.", Type = Type.String, IsRequired = true },
            new Input { Id = Guid.NewGuid(), Name = "Regular Expression", Description = "The regular expression.", Type = Type.String, IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Match Result", Description = "Whether the string matches the pattern.", Type = Type.Boolean },
            new Output { Id = Guid.NewGuid(), Name = "Matched Groups", Description = "The groups matched by the regular expression.", Type = Type.String, IsList = true }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var match = Regex.Match(inputs[0].ToString(), inputs[1].ToString());
            var matchResult = match.Success;
            var matchedGroups = match.Groups.Cast<Group>().Select(g => g.Value).ToList();
            programStructure.InputValues[Outputs[0].Id] = matchResult;
            programStructure.InputValues[Outputs[1].Id] = matchedGroups;
        }
    }

    public class Format : Block
    {
        public Format()
        {
            Id = Guid.NewGuid();
            Name = "Format";
            Description = "A block that replaces the format items in a specified string with the string representation of specified objects.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Format String", Description = "The format string.", Type = Type.String, IsRequired = true },
            new Input { Id = Guid.NewGuid(), Name = "Arguments", Description = "The arguments.", Type = Type.String, IsRequired = true, IsList = true }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Formatted String", Description = "The formatted string.", Type = Type.String }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var args = ((List<object>)inputs[1]).ToArray();
            var formattedString = string.Format(inputs[0].ToString(), args);
            programStructure.InputValues[Outputs[0].Id] = formattedString;
        }
    }
}
