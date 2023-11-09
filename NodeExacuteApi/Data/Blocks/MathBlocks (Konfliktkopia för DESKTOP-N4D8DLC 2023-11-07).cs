using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using NodeBaseApi.Version2;
using Type = NodeBaseApi.Version2.Type;

namespace NodeExacuteApi.Data.Blocks
{
    public class AddBlock : Block
    {
        public AddBlock()
        {
            Id = Guid.NewGuid();
            Name = "AddBlock";
            Description = "Adds two numbers together.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "First Number", Description = "The first number to add.", Type = Type.Number, IsRequired = true, IsList = false },
            new Input { Id = Guid.NewGuid(), Name = "Second Number", Description = "The second number to add.", Type = Type.Number, IsRequired = true, IsList = false }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Sum", Description = "The sum of the two numbers.", Type = Type.Number, IsList = false }
        };
        }

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
        public SubtractBlock()
        {
            Id = Guid.NewGuid();
            Name = "SubtractBlock";
            Description = "Subtracts the second number from the first number.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "First Number", Description = "The number to subtract from.", Type = Type.Number, IsRequired = true, IsList = false },
            new Input { Id = Guid.NewGuid(), Name = "Second Number", Description = "The number to subtract.", Type = Type.Number, IsRequired = true, IsList = false }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Difference", Description = "The difference of the two numbers.", Type = Type.Number, IsList = false }
        };
        }

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
        public MultiplyBlock()
        {
            Id = Guid.NewGuid();
            Name = "MultiplyBlock";
            Description = "Multiplies two numbers together.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "First Number", Description = "The first number to multiply.", Type = Type.Number, IsRequired = true, IsList = false },
            new Input { Id = Guid.NewGuid(), Name = "Second Number", Description = "The second number to multiply.", Type = Type.Number, IsRequired = true, IsList = false }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Product", Description = "The product of the two numbers.", Type = Type.Number, IsList = false }
        };
        }

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
        public DivideBlock()
        {
            Id = Guid.NewGuid();
            Name = "DivideBlock";
            Description = "Divides the first number by the second number.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "First Number", Description = "The number to divide.", Type = Type.Number, IsRequired = true, IsList = false },
            new Input { Id = Guid.NewGuid(), Name = "Second Number", Description = "The number to divide by.", Type = Type.Number, IsRequired = true, IsList = false }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Quotient", Description = "The quotient of the two numbers.", Type = Type.Number, IsList = false }
        };
        }

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
        public ModulusBlock()
        {
            Id = Guid.NewGuid();
            Name = "ModulusBlock";
            Description = "Calculates the remainder of the division of the first number by the second number.";
            Inputs = new List<Input>
    {
        new Input { Id = Guid.NewGuid(), Name = "First Number", Description = "The dividend.", Type = Type.Number, IsRequired = true },
        new Input { Id = Guid.NewGuid(), Name = "Second Number", Description = "The divisor.", Type = Type.Number, IsRequired = true }
    };
            Outputs = new List<Output>
    {
        new Output { Id = Guid.NewGuid(), Name = "Remainder", Description = "The remainder of the division.", Type = Type.Number }
    };
        }

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
        public PowerBlock()
        {
            Id = Guid.NewGuid();
            Name = "PowerBlock";
            Description = "Raises the first number to the power of the second number.";
            Inputs = new List<Input>
    {
        new Input { Id = Guid.NewGuid(), Name = "Base", Description = "The base number.", Type = Type.Number, IsRequired = true },
        new Input { Id = Guid.NewGuid(), Name = "Exponent", Description = "The exponent.", Type = Type.Number, IsRequired = true }
    };
            Outputs = new List<Output>
    {
        new Output { Id = Guid.NewGuid(), Name = "Result", Description = "The result of the operation.", Type = Type.Number }
    };
        }

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
        public SquareRootBlock()
        {
            Id = Guid.NewGuid();
            Name = "SquareRootBlock";
            Description = "Calculates the square root of a number.";
            Inputs = new List<Input>
    {
        new Input { Id = Guid.NewGuid(), Name = "Number", Description = "The number.", Type = Type.Number, IsRequired = true }
    };
            Outputs = new List<Output>
    {
        new Output { Id = Guid.NewGuid(), Name = "Result", Description = "The square root of the number.", Type = Type.Number }
    };
        }

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
        public AbsoluteValueBlock()
        {
            Id = Guid.NewGuid();
            Name = "AbsoluteValueBlock";
            Description = "Calculates the absolute value of a number.";
            Inputs = new List<Input>
    {
        new Input { Id = Guid.NewGuid(), Name = "Number", Description = "The number.", Type = Type.Number, IsRequired = true }
    };
            Outputs = new List<Output>
    {
        new Output { Id = Guid.NewGuid(), Name = "Result", Description = "The absolute value of the number.", Type = Type.Number }
    };
        }

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

        public RandomNumberBlock()
        {
            Id = Guid.NewGuid();
            Name = "RandomNumberBlock";
            Description = "Generates a random number between the specified minimum and maximum values.";

            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Min", Description = "Minimum value (inclusive).", Type = Type.Number, IsRequired = true },
            new Input { Id = Guid.NewGuid(), Name = "Max", Description = "Maximum value (exclusive).", Type = Type.Number, IsRequired = true }
        };

            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Result", Description = "The generated random number.", Type = Type.Number }
        };
        }

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

}
