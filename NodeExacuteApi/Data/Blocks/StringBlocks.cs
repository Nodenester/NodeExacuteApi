using Newtonsoft.Json;
using NodeBaseApi.Version2;
using System.Text;
using System.Text.RegularExpressions;
using Type = NodeBaseApi.Version2.Type;

namespace NodeExacuteApi.Data.Blocks
{
    public class Concatenation : Block
    {
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
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var substring = ((string)inputs[0]).Substring((int)inputs[1], (int)inputs[2]);
            programStructure.InputValues[Outputs[0].Id] = substring;
        }
    }

    public class Lowercase : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var result = ((string)inputs[0]).ToLower();
            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class Uppercase : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var result = ((string)inputs[0]).ToUpper();
            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class Contains : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            if (inputs[0] == null)
            {
                inputs[0] = "";
            }
            var contains = inputs[0].ToString().Contains(inputs[1].ToString());
            programStructure.InputValues[Outputs[0].Id] = contains;
        }
    }

    // new ones
    public class Split : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var substrings = inputs[0].ToString().Split(new string[] { inputs[1].ToString() }, StringSplitOptions.None).ToList();
            programStructure.InputValues[Outputs[0].Id] = substrings;
        }
    }

    public class Join : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var strings = ((List<object>)inputs[0]).ConvertAll(input => input.ToString());
            var resultantString = string.Join(inputs[1].ToString(), strings);
            programStructure.InputValues[Outputs[0].Id] = resultantString;
        }
    }

    public class Replace : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            if (inputs[0] == null)
            {
                inputs[0] = "";
            }
            var resultantString = inputs[0].ToString().Replace(inputs[1].ToString(), inputs[2].ToString());
            programStructure.InputValues[Outputs[0].Id] = resultantString;
        }
    }

    public class Trim : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var trimmedString = inputs[0].ToString().Trim();
            programStructure.InputValues[Outputs[0].Id] = trimmedString;
        }
    }

    public class IndexOf : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var index = inputs[0].ToString().IndexOf(inputs[1].ToString());
            programStructure.InputValues[Outputs[0].Id] = index;
        }
    }

    public class RegularExpressionMatch : Block
    {
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
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var args = ((List<object>)inputs[1]).ToArray();
            var formattedString = string.Format(inputs[0].ToString(), args);
            programStructure.InputValues[Outputs[0].Id] = formattedString;
        }
    }

    public class StringCleaner : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            var mainString = inputs[0].ToString();
            var startDelimiter = inputs[1].ToString();
            var endDelimiter = inputs[2].ToString();

            var cleanedString = RemoveSubstringsBetweenDelimiters(mainString, startDelimiter, endDelimiter);

            programStructure.InputValues[Outputs[0].Id] = cleanedString;
        }

        private string RemoveSubstringsBetweenDelimiters(string input, string start, string end)
        {
            var regexPattern = Regex.Escape(start) + "(.*?)" + Regex.Escape(end);
            return Regex.Replace(input, regexPattern, string.Empty);
        }
    }

    public class IntParser : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            var inputString = inputs[0].ToString();
            if (int.TryParse(inputString, out int parsedInt))
            {
                programStructure.InputValues[Outputs[0].Id] = parsedInt;
            }
            else
            {
                // Handle the case where parsing fails, e.g., return a default value or log an error
            }
        }
    }

    public class BoolParser : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            var inputString = inputs[0].ToString();
            if (bool.TryParse(inputString, out bool parsedBool))
            {
                programStructure.InputValues[Outputs[0].Id] = parsedBool;
            }
            else
            {
                // Handle the case where parsing fails, e.g., return a default value or log an error
            }
        }
    }
}
