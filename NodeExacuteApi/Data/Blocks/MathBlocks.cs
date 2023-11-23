using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using NodeBaseApi.Version2;
using Type = NodeBaseApi.Version2.Type;

namespace NodeExacuteApi.Data.Blocks
{
    public class AddBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            double firstNumber = Convert.ToDouble(inputs[0]);
            double secondNumber = Convert.ToDouble(inputs[1]);

            double sum = firstNumber + secondNumber;

            programStructure.InputValues[Outputs[0].Id] = sum;
        }
    }

    public class SubtractBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            double firstNumber = Convert.ToDouble(inputs[0]);
            double secondNumber = Convert.ToDouble(inputs[1]);

            double difference = firstNumber - secondNumber;

            programStructure.InputValues[Outputs[0].Id] = difference;
        }
    }

    public class MultiplyBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            double firstNumber = Convert.ToDouble(inputs[0]);
            double secondNumber = Convert.ToDouble(inputs[1]);

            double product = firstNumber * secondNumber;

            programStructure.InputValues[Outputs[0].Id] = product;
        }
    }

    public class DivideBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure , string sessionId, Guid variableid)
        {
            double firstNumber = Convert.ToDouble(inputs[0]);
            double secondNumber = Convert.ToDouble(inputs[1]);

            if (secondNumber == 0)
            {
                throw new DivideByZeroException("Cannot divide by zero.");
            }

            double quotient = firstNumber / secondNumber;

            programStructure.InputValues[Outputs[0].Id] = quotient;
        }
    }

    //new ones
    public class ModulusBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            double firstNumber = Convert.ToDouble(inputs[0]);
            double secondNumber = Convert.ToDouble(inputs[1]);

            if (secondNumber == 0)
            {
                throw new DivideByZeroException("Cannot divide by zero.");
            }

            double remainder = firstNumber % secondNumber;

            programStructure.InputValues[Outputs[0].Id] = remainder;
        }
    }

    public class PowerBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            double baseNumber = Convert.ToDouble(inputs[0]);
            double exponent = Convert.ToDouble(inputs[1]);

            double result = Math.Pow(baseNumber, exponent);

            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class SquareRootBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            double number = Convert.ToDouble(inputs[0]);

            if (number < 0)
            {
                throw new ArgumentException("Cannot calculate the square root of a negative number.");
            }

            double result = Math.Sqrt(number);

            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class AbsoluteValueBlock : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            double number = Convert.ToDouble(inputs[0]);

            double result = Math.Abs(number);

            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class RandomNumberBlock : Block
    {
        private Random _random = new Random();

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            double min = Convert.ToDouble(inputs[0]);
            double max = Convert.ToDouble(inputs[1]);

            // Ensure min is less than or equal to max
            if (min > max)
            {
                throw new ArgumentException("Min value cannot be greater than Max value.");
            }

            double result = min + _random.NextDouble() * (max - min);

            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    //math logic block
    public class Equal : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            double number1 = Convert.ToDouble(inputs[0]);
            double number2 = Convert.ToDouble(inputs[1]);

            bool result = number1 == number2;

            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class LessThan : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            double number1 = Convert.ToDouble(inputs[0]);
            double number2 = Convert.ToDouble(inputs[1]);

            bool result = number1 < number2;

            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class MoreThan : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            double number1 = Convert.ToDouble(inputs[0]);
            double number2 = Convert.ToDouble(inputs[1]);

            bool result = number1 > number2;

            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class LessThanOrEqual : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            double number1 = Convert.ToDouble(inputs[0]);
            double number2 = Convert.ToDouble(inputs[1]);

            bool result = number1 <= number2;

            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

    public class MoreThanOrEqual : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            double number1 = Convert.ToDouble(inputs[0]);
            double number2 = Convert.ToDouble(inputs[1]);

            bool result = number1 >= number2;

            programStructure.InputValues[Outputs[0].Id] = result;
        }
    }

}
