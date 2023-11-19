using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using NodeExacuteApi.Data.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

// Create the base and all functions   and write what they should do in the comments
namespace NodeBaseApi.Version2
{
    public abstract class ProgramObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string AuthorName { get; set; }
        public bool IsPublic { get; set; }
        public bool SupportsSessions { get; set; }
    }
    public class CustomProgram : ProgramObject
    {
        //The Program they make should later be like an api for them to call where the call my api whit that specific key and the server checks that progaram and exacutes it
        public ProgramStructure ProgramStructure { get; set; }
        public Guid ApiKey { get; set; }
    }
    public class CustomBlockProgram : ProgramObject 
    {
        //This should be a program that can be used as a block in other programs
        public ProgramStructure ProgramStructure { get; set; }
    }

    public class ProgramStructure
    {
        public List<ProgramBlock> ProgramBlocks { get; set; } = new List<ProgramBlock>();
        public Dictionary<Guid, Variable> Variables { get; set; } = new Dictionary<Guid, Variable>();
        public List<Output> ProgramStart { get; set; } = new List<Output>();
        public Tuple<int, int> ProgramStartLocation { get; set; } = new Tuple<int, int>(0, 200);

        public List<Input> ProgramEnd { get; set; } = new List<Input>();
        public Dictionary<Guid, Guid> ProgramEndConnections { get; set; } = new Dictionary<Guid, Guid>();
        public Tuple<int, int> ProgramEndLocation { get; set; } = new Tuple<int, int>(300, 200);
        public float zoom { get; set; } = 1;

        public int MaxPrice { get; set; } = 100;
        public int CurrentPrizing { get; set; } = 50;

        public Tuple<double, double> CameraPos { get; set; } = new Tuple<double, double>(0, 0);
        public Dictionary<Guid, object> InputValues { get; set; } = new Dictionary<Guid, object>();
        public Dictionary<Guid, object> OutputValues { get; set; } = new Dictionary<Guid, object>();
        public Dictionary<Guid, CustomBlockProgram> CustomPrograms { get; set; } = new Dictionary<Guid, CustomBlockProgram>();
        public Dictionary<Guid, object> BlockOutputValues { get; set; } = new Dictionary<Guid, object>();
        public Dictionary<Guid, object> DirectInputValues { get; set; } = new Dictionary<Guid, object>();

        //Pricing functions
        public void AddCost(int cost)
        {
            CurrentPrizing = CurrentPrizing + cost;
        }

        public bool HasTokens(int cost)
        {
            return MaxPrice >= cost;
        }


        public void AddProgramInput(Output output)
        {
            ProgramStart.Add(output);
        }

        public void SetInputValue(int inputIndex, object value)
        {
            if (inputIndex < 0 || inputIndex >= ProgramStart.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(inputIndex), "Invalid input index");
            }

            InputValues[ProgramStart[inputIndex].Id] = value;
        }

        public object GetOutputValue(int outputIndex)
        {
            if (outputIndex < 0 || outputIndex >= ProgramEnd.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(outputIndex), "Invalid output index");
            }

            return OutputValues[ProgramEnd[outputIndex].Id];
        }

        public bool SetVariableValue(Guid variableId, object value)
        {
            if (!Variables.ContainsKey(variableId))
            {
                return false;
            }

            Variables[variableId].Value = value;
            return true;
        }

        public object GetVariableValue(Guid variableId)
        {
            return Variables.TryGetValue(variableId, out Variable variable) ? variable.Value : null;
        }

        public List<string> CheckErrors()
        {
            List<string> errors = new List<string>();

            // Check for required inputs without values
            foreach (var input in ProgramBlocks.SelectMany(block => block.Block.Inputs))
            {
                if (input.IsRequired && !InputValues.ContainsKey(input.Id))
                {
                    errors.Add($"Required input '{input.Name}' is missing a value.");
                    throw new MissingRequiredInputException($"Required input '{input.Name}' is missing a value.");
                }
            }

            // Check for disconnected inputs
            foreach (var input in ProgramBlocks.SelectMany(block => block.Block.Inputs))
            {
                if (!InputValues.ContainsKey(input.Id))
                {
                    errors.Add($"Input '{input.Name}' is not connected.");
                    throw new DisconnectedInputException($"Input '{input.Name}' is not connected.");
                }
            }

            // Additional error checks can be added here...

            return errors;
        }

        public void SetSessionVariables(Dictionary<Guid, object> sessionVariables)
        {
            // Iterate through the session variables and update the Variables dictionary
            foreach (var sessionVariable in sessionVariables)
            {
                if (Variables.ContainsKey(sessionVariable.Key))
                {
                    Variables[sessionVariable.Key].Value = sessionVariable.Value;
                }
                else
                {
                    // If the session variable does not exist in the Variables dictionary, you can choose to create a new Variable object or throw an exception
                    throw new InvalidOperationException($"Variable with ID '{sessionVariable.Key}' not found.");
                }
            }
        }
        public Dictionary<Guid, object> GetSessionVariables()
        {
            // Create a dictionary to store the session variable values
            Dictionary<Guid, object> sessionVariables = new Dictionary<Guid, object>();

            // Iterate through the Variables dictionary and add the values of session variables to the sessionVariables dictionary
            foreach (var variable in Variables)
            {
                if (variable.Value.IsSessionVariable)
                {
                    sessionVariables[variable.Key] = variable.Value.Value;
                }
            }

            return sessionVariables;
        }
        public void AddCustomProgram(CustomBlockProgram customProgram)
        {
            if (CustomPrograms.ContainsKey(customProgram.Id))
            {
                throw new ArgumentException("A custom program with the same ID already exists.");
            }

            CustomPrograms[customProgram.Id] = customProgram;
        }

        public async Task ExecuteProgram(string sessionId)
        {
            // Execute the program starting from the default start trigger and this list should only contain blocks with trigger inputs
            Output startTrigger = ProgramStart.FirstOrDefault(output => output.Type == Type.Trigger);
            if (startTrigger != null)
            {
                await ExecuteBlockAndConnectedAsync(startTrigger.Id, null, sessionId);
            }
        }

        private async Task ExecuteBlockAndConnectedAsync(Guid outputId, ProgramBlock BlockToExecute, string sessionId)
        {
            if (ProgramEndConnections.ContainsValue(outputId))
            {
                foreach (var inputId in ProgramEndConnections)
                {
                    foreach (ProgramBlock pb in ProgramBlocks)
                    {
                        // If this block has an output connected to the given input and doesn't have a trigger output
                        if (pb.Block.Outputs != null && pb.Block.Outputs.Any(output => output.Id == inputId.Value) && !pb.Block.Outputs.Any(o => o.Type == Type.Trigger))
                        {
                            await ExecuteBlockAndConnectedAsync(Guid.Empty, pb, sessionId);
                            if (pb.Outputs == null)
                            {
                                foreach (var output in pb.Block.Outputs)
                                {
                                    InputValues[outputId] = InputValues[output.Id];
                                }
                            }
                        }
                    }
                }
                return;
            }

            // Find the connected block for the given output
            ProgramBlock blockToExecute = new ProgramBlock();

            //triggerinputs might not be showing up in here
            if (outputId != Guid.Empty)
            {
                Guid inputId = InputValues.FirstOrDefault(kvp => kvp.Key.Equals(outputId)).Key;
                if(inputId == Guid.Empty)
                {
                    blockToExecute = ProgramBlocks.FirstOrDefault(b => b.Inputs?.Contains(outputId) ?? false);                
                }
                else
                {
                    blockToExecute = ProgramBlocks.FirstOrDefault(b => b.Inputs?.Contains(inputId) ?? false);
                }
            }
            else
            {
                blockToExecute = BlockToExecute;
            }

            if (blockToExecute != null)
            {
                // Get the input values for the block and execute it
                List<object> inputValues = await GetInputValuesForBlock(blockToExecute, sessionId);
                await blockToExecute.Block.ExecuteAsync(inputValues, this, sessionId, blockToExecute.VariableId);

                // Check for loop and conditional blocks
                if (blockToExecute.Block is IndexLoop indexLoop)
                {
                    int loopCount = (int)InputValues[indexLoop.Inputs.ToArray()[1].Id];
                    for (int i = 0; i < loopCount; i++)
                    {
                        InputValues[blockToExecute.Outputs[1]] = i;
                        await ExecuteBlockAndConnectedAsync(indexLoop.Outputs.ToArray()[0].Id, null, sessionId);
                    }
                    await ExecuteBlockAndConnectedAsync(indexLoop.Outputs.ToArray()[2].Id, null, sessionId);
                }
                else if (blockToExecute.Block is ForLoop forLoop)
                {
                    Guid Index = blockToExecute.Inputs.ToArray()[1];
                    List<object> list = new List<object>();
                    try
                    {
                        JsonElement jsonElement = (JsonElement)InputValues[Index];
                        string jsonString = jsonElement.GetRawText();
                        string[] stringArray = System.Text.Json.JsonSerializer.Deserialize<string[]>(jsonString);
                        list = stringArray.Cast<object>().ToList();
                    }
                    catch(Exception e)
                    { 
                        Console.WriteLine(e.Message);
                    }
                    foreach (var item in list)
                    {
                        InputValues[forLoop.Outputs.ToArray()[1].Id] = item;
                        await ExecuteBlockAndConnectedAsync(forLoop.Outputs.ToArray()[0].Id, null, sessionId);
                    }
                    await ExecuteBlockAndConnectedAsync(forLoop.Outputs.ToArray()[2].Id, null, sessionId);
                }
                else if (blockToExecute.Block is WhileLoop whileLoop)
                {
                    bool condition = (bool)InputValues[whileLoop.Outputs.ToArray()[0].Id];
                    while (condition)
                    {
                        await ExecuteBlockAndConnectedAsync(whileLoop.Outputs.ToArray()[2].Id, null, sessionId);
                        condition = (bool)InputValues[whileLoop.Inputs.ToArray()[1].Id];
                    }
                    await ExecuteBlockAndConnectedAsync(whileLoop.Outputs.ToArray()[2].Id, null, sessionId);
                }
                else if (blockToExecute.Block is IfBlock ifBlock)
                {
                    // Execute the appropriate branch based on the condition
                    bool condition = bool.Parse(inputValues[1].ToString());

                    if (condition)
                    {
                        await ExecuteBlockAndConnectedAsync(ifBlock.Outputs.ToArray()[0].Id, null, sessionId);
                    }
                    else
                    {
                        await ExecuteBlockAndConnectedAsync(ifBlock.Outputs.ToArray()[1].Id, null, sessionId);
                    }
                }
                else if (blockToExecute.Block is Switch switchBlock)
                {
                    // Assuming the trigger input is at index 0 and the selector input is at index 1.
                    if (InputValues[switchBlock.Inputs.ToArray()[0].Id] != null) // Check if the trigger input is not null
                    {
                        int selector = Convert.ToInt32(InputValues[switchBlock.Inputs.ToArray()[1].Id]);

                        // The default output is at index 0. 
                        if (selector > 0 && selector < switchBlock.Outputs.Count())
                        {
                            await ExecuteBlockAndConnectedAsync(switchBlock.Outputs.ToArray()[selector].Id, null, sessionId);
                        }
                        else
                        {
                            await ExecuteBlockAndConnectedAsync(switchBlock.Outputs.First().Id, null, sessionId); // Default case
                        }
                    }
                }

                //Get Next Block to exacute
                if (blockToExecute.Block.Outputs[0].Type == Type.Trigger)
                {
                    await ExecuteBlockAndConnectedAsync(blockToExecute.Block.Outputs[0].Id, null, sessionId);
                }
                else
                {
                    return;
                }
            }
        }

        // This function should collect input values for a block based on its connections
        private async Task<List<object>> GetInputValuesForBlock(ProgramBlock block, string sessionId)
        {
            List<object> inputValues = new List<object>();

            if(block.Block.Inputs.Count() > 0)
            {
                var index = 0;
                foreach (Guid inputId in block.Inputs)
                {
                    foreach (ProgramBlock pb in ProgramBlocks)
                    {
                        // If this block has an output connected to the given input and doesn't have a trigger output
                        if (pb.Block.Outputs != null && pb.Block.Outputs.Any(output => output.Id == inputId) && !pb.Block.Outputs.Any(o => o.Type == Type.Trigger))
                        {
                            await ExecuteBlockAndConnectedAsync(Guid.Empty, pb, sessionId);
                        }
                    }
                    if (InputValues.ContainsKey(inputId))
                    {
                        inputValues.Add(InputValues[inputId]);
                    }
                    else if (DirectInputValues.ContainsKey(block.Block.Inputs[block.Inputs.IndexOf(inputId)].Id) && DirectInputValues[block.Block.Inputs[block.Inputs.IndexOf(inputId)].Id].ToString() != "{}")
                    {
                        //var index = block.Inputs.IndexOf(inputId);
                        inputValues.Add(DirectInputValues[block.Block.Inputs[index].Id]);
                    }
                    else
                    {
                        inputValues.Add(null);
                    }
                    index++;
                }
            }
            return inputValues;
        }
    }

    //BlockData-------------------------------------------------------
    public abstract class Block
    {
        public Guid Id;
        public string Name;
        public string Description;
        public List<Input> Inputs;
        public List<Output> Outputs;

        public abstract Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid);
    }

    public class ProgramBlock
    {
        public Guid Id;
        public Block Block;
        public List<Guid> Inputs = new List<Guid>();
        public List<Guid> Outputs = new List<Guid>();
        public Guid VariableId{ get; set; }
        public int X;
        public int Y;
    }
    //Data------------------------------------------------------------
    public class Variable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Type Type { get; set; }
        public bool IsList { get; set; }
        public bool IsSessionVariable { get; set; }
        public object Value { get; set; }
    }
    public class Session
    {
        public Guid SessionId { get; set; }
        public string UserId { get; set; }
        public string ProgramId { get; set; }
        public string Variables { get; set; }
        public string SessionName { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastEditedTime { get; set; }
    }
    public class Input
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Type Type { get; set; }
        public bool IsRequired { get; set; }
        public bool IsList { get; set; }
    }
    public class Output
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Type Type { get; set; }
        public bool IsList { get; set; }
    }

    public enum Type
    {
        String,
        Picture,
        Number,
        Boolean,
        Audio,
        Trigger,
        Object
    }

    public class Call
    {
        public string ProgramId { get; set; }
        public string ApiUserId { get; set; }
        public bool? IsTest { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Cost { get; set; }

        [JsonIgnore]
        public List<object> Input { get; set; }

        [JsonIgnore]
        public List<object> Output { get; set; }

        public string InputJson
        {
            get => JsonConvert.SerializeObject(Input);
            set => Input = JsonConvert.DeserializeObject<List<object>>(value);
        }

        public string OutputJson
        {
            get => JsonConvert.SerializeObject(Output);
            set => Output = JsonConvert.DeserializeObject<List<object>>(value);
        }

        public Call()
        {
            Input = new List<object>();
            Output = new List<object>();
        }
    }

    //Error handeling------------------------------------------------
    public class InvalidConnectionException : Exception
    {
        public InvalidConnectionException(string message) : base(message) { }
    }

    public class MissingRequiredInputException : Exception
    {
        public MissingRequiredInputException(string message) : base(message) { }
    }

    public class DisconnectedInputException : Exception
    {
        public DisconnectedInputException(string message) : base(message) { }
    }
}
