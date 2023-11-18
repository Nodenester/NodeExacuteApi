using Newtonsoft.Json;
using NodeBaseApi.Version2;
using Type = NodeBaseApi.Version2.Type;

namespace NodeExacuteApi.Data.Blocks
{
    //Loop blocks
    public class IndexLoop : Block
    {
        public IndexLoop()
        {
            Id = Guid.NewGuid();
            Name = "Index Loop";
            Description = "A loop that iterates a specified number of times.";

            Inputs = new List<Input>
        {
            new Input { Name = "Trigger", Type = Type.Trigger, Description = "The trigger input for starting the loop" },
            new Input { Name = "LoopCount", Type = Type.Number, IsRequired = true, Description = "The number of iterations for the loop" }
        };

            Outputs = new List<Output>
        {
            new Output { Name = "Trigger", Type = Type.Trigger, Description = "The trigger output for each iteration of the loop" },
            new Output { Name = "Index", Type = Type.Number, Description = "The current index of the loop" },
            new Output { Name = "LoopCompleted", Type = Type.Trigger, Description = "The trigger output when the loop is completed" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
        }
    }
    public class ForLoop : Block
    {
        public ForLoop()
        {
            Id = Guid.NewGuid();
            Name = "For Loop";
            Description = "A loop that iterates through each item in a list.";

            Inputs = new List<Input>
        {
            new Input { Name = "Trigger", Type = Type.Trigger, Description = "The trigger input for starting the loop" },
            new Input { Name = "List", Type = Type.Object, IsRequired = true, IsList = true, Description = "The list to loop through" }
        };

            Outputs = new List<Output>
        {
            new Output { Name = "Trigger", Type = Type.Trigger, Description = "The trigger output for each iteration of the loop" },
            new Output { Name = "CurrentItem", Type = Type.Object, Description = "The current item in the list being looped through" },
            new Output { Name = "LoopCompleted", Type = Type.Trigger, Description = "The trigger output when the loop is completed" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {

        }
    }
    public class WhileLoop : Block
    {
        public WhileLoop()
        {
            Id = Guid.NewGuid();
            Name = "While Loop";
            Description = "A loop that continues to iterate while a condition is true.";

            Inputs = new List<Input>
        {
            new Input { Name = "Trigger", Type = Type.Trigger, Description = "The trigger input for starting the loop" },
            new Input { Name = "Condition", Type = Type.Boolean, IsRequired = true, Description = "The condition to be checked each iteration" }
        };

            Outputs = new List<Output>
        {
            new Output { Name = "Trigger", Type = Type.Trigger, Description = "The trigger output for each iteration of the loop" },
            new Output { Name = "LoopCompleted", Type = Type.Trigger, Description = "The trigger output when the loop is completed" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {

        }
    }

    //If block
    public class IfBlock : Block
    {
        public IfBlock()
        {
            Id = Guid.NewGuid();
            Name = "If Block";
            Description = "A conditional block that executes different outputs based on a boolean condition.";

            Inputs = new List<Input>
        {
            new Input { Name = "Trigger", Type = Type.Trigger, Description = "Triggers the Ifblock" },
            new Input { Name = "Condition", Type = Type.Boolean, IsRequired = true, Description = "The condition to be checked" }
        };

            Outputs = new List<Output>
        {
            new Output { Name = "TrueBranchOutput", Type = Type.Trigger, Description = "The trigger output when the condition is true" },
            new Output { Name = "FalseBranchOutput", Type = Type.Trigger, Description = "The trigger output when the condition is false" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {

        }
    }

    //Variable handeling
    public class SetVariable : Block
    {
        public SetVariable()
        {
            Id = Guid.NewGuid();
            Name = "Set Variable";
            Description = "Sets the value of a variable";

            Inputs = new List<Input>
        {
            new Input
            {Id = Guid.NewGuid(),Name = "Value",Description = "The value to set",Type = Type.Object,IsRequired = true,IsList = false}
        };

            Outputs = new List<Output>
        {
            new Output
            {Id = Guid.NewGuid(),Name = "Trigger",Description = "Trigger output",Type = Type.Trigger,IsList = false}
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            programStructure.Variables[variableid].Value = inputs[1];

            // Trigger output
        }
    }
    public class GetVariable : Block
    {
        public GetVariable()
        {
            Id = Guid.NewGuid();
            Name = "Get Variable";
            Description = "Gets the value of a variable";

            Inputs = new List<Input>{ };

            Outputs = new List<Output>
            {
                new Output
                {
                    Id = Guid.NewGuid(),
                    Name = "Value",
                    Description = "The value of the variable",
                    Type = Type.Object,
                    IsList = false
                }
            };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            object value = programStructure.Variables[variableid].Value;
            // Return the value as output
            programStructure.InputValues[Outputs[0].Id] = value;
        }
    }

    //Switch
    public class Select : Block
    {
        public Select()
        {
            Id = Guid.NewGuid();
            Name = "Select";
            Description = "Selects one of the inputs based on the selector value.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Selector", Description = "The index of the input to select.", Type = Type.Number, IsRequired = true },
            new Input { Id = Guid.NewGuid(), Name = "Input 1", Description = "First input.", Type = Type.Object, IsRequired = false },
            new Input { Id = Guid.NewGuid(), Name = "Input 2", Description = "Second input.", Type = Type.Object, IsRequired = false },
            // ... add more inputs as needed
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Output", Description = "The selected input.", Type = Type.Object }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            int selector = Convert.ToInt32(inputs[0]);

            if (selector < 1 || selector > inputs.Count - 1)
            {
                throw new ArgumentOutOfRangeException("Selector value is out of range.");
            }

            object selectedInput = inputs[selector];
            programStructure.InputValues[Outputs[0].Id] = selectedInput;
        }
    }
    public class Switch : Block
    {
        public Switch()
        {
            Id = Guid.NewGuid();
            Name = "Switch";
            Description = "Triggers different outputs based on a selector value.";

            Inputs = new List<Input>
    {
        new Input { Id = Guid.NewGuid(), Name = "Trigger", Description = "The trigger input for starting the switch", Type = Type.Trigger, IsRequired = true },
        new Input { Id = Guid.NewGuid(), Name = "Selector", Description = "The value to be checked.", Type = Type.Number, IsRequired = true }
    };

            Outputs = new List<Output>
    {
        new Output { Id = Guid.NewGuid(), Name = "DefaultOutput", Description = "The default output if no cases match.", Type = Type.Trigger },
        new Output { Id = Guid.NewGuid(), Name = "Case 1", Description = "Triggered for case 1.", Type = Type.Trigger },
        new Output { Id = Guid.NewGuid(), Name = "Case 2", Description = "Triggered for case 2.", Type = Type.Trigger },
    };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {

        }
    }


    //Custom block
    public class CustomBlock : Block
    {
        public ProgramStructure SubProgramStructure { get; set; }

        private readonly DBConnection db;

        public CustomBlock(DBConnection dbConnection)
        {
            var db = dbConnection;
        }

        public CustomBlock()
        {
            Id = Guid.NewGuid();
            Name = "Custom Block";
            Description = "A block that contains a sub-program";
            Inputs = new List<Input>();
            Outputs = new List<Output>();
        }

        //need to remake this because now they dont work  they need to get the right programstructure
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            Session session = null;
            Guid guidSessionId;

            CustomBlockProgram program = programStructure.CustomPrograms[variableid];

            if (!string.IsNullOrEmpty(sessionId) && Guid.TryParse(sessionId, out guidSessionId))
            {
                session = db.GetSessionAsync(guidSessionId).GetAwaiter().GetResult();

                if (session == null)
                {
                    // Check if the program supports sessions
                    if (true)
                    {
                        // Create a new session and store it in the database
                        session = new Session { SessionId = Guid.NewGuid(), Variables = JsonConvert.SerializeObject(new Dictionary<Guid, object>()) };
                        db.CreateSessionAsync(session).GetAwaiter().GetResult();

                        // Set the session variables in the program structure
                        var sessionVariables = JsonConvert.DeserializeObject<Dictionary<Guid, object>>(session.Variables);
                        programStructure.SetSessionVariables(sessionVariables);
                    }
                    else
                    {
                        throw new Exception("Session not found and the program does not support sessions.");
                    }
                }
                else
                {
                    var sessionVariables = JsonConvert.DeserializeObject<Dictionary<Guid, object>>(session.Variables);
                    programStructure.SetSessionVariables(sessionVariables);
                }
            }

            // Set input values for the program based on the inputs to this block
            for (int i = 0; i < inputs.Count; i++)
            {
                program.ProgramStructure.SetInputValue(i, inputs[i]);
            }

            // Execute the program
            await program.ProgramStructure.ExecuteProgram(sessionId);

            // Update the session variables if session is not null
            if (session != null)
            {
                var updatedSessionVariables = program.ProgramStructure.GetSessionVariables();
                session.Variables = JsonConvert.SerializeObject(updatedSessionVariables);
                db.UpdateSessionAsync(session).GetAwaiter().GetResult();
            }

            //Create The output
            List<object> ProgramOutput = new List<object>();

            bool isFirstIteration = true;
            foreach (var inputId in program.ProgramStructure.ProgramEndConnections)
            {
                if (isFirstIteration)
                {
                    isFirstIteration = false;
                    continue;
                }

                if (program.ProgramStructure.InputValues.ContainsKey(inputId.Value))
                {
                    ProgramOutput.Add(program.ProgramStructure.InputValues[inputId.Value]);
                }
                else if (program.ProgramStructure.DirectInputValues.ContainsKey(inputId.Value))
                {
                    ProgramOutput.Add(program.ProgramStructure.DirectInputValues[inputId.Value]);
                }
            }

            foreach(var output in ProgramOutput)
            {
                programStructure.InputValues[Outputs[ProgramOutput.IndexOf(output) + 1].Id] = output;
            }
        }
    }
}
