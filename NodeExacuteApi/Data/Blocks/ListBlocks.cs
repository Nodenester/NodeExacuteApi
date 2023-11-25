using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NodeBaseApi.Version2;
using Type = NodeBaseApi.Version2.Type;

namespace NodeExacuteApi.Data.Blocks
{
    public class AddItem : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var list = new List<object>();

            if(inputs[0] != null)
            {
                list = ((IEnumerable)inputs[0]).Cast<object>().ToList();
            }

            var item = inputs[1];
            list.Add(item);
            programStructure.InputValues[Outputs[0].Id] = list;
        }
    }

    public class RemoveItem : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var list = inputs[0] as List<object>;
            var index = Convert.ToInt32(inputs[1]);
            list.RemoveAt(index);
            programStructure.InputValues[Outputs[0].Id] = list;
        }
    }
    public class GetItem : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            try
            {
                if (!(inputs[0] is IList list))
                {
                    throw new InvalidOperationException("First input is not a list.");
                }

                var index = Convert.ToInt32(inputs[1]);
                if (index < 0 || index > list.Count)
                {
                    throw new IndexOutOfRangeException("Index is out of range.");
                }

                var item = list[index];
                programStructure.InputValues[Outputs[0].Id] = item;
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                throw;
            }
        }
    }

    public class SetItem : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var list = inputs[0] as List<object>;
            var index = Convert.ToInt32(inputs[1]);
            var item = inputs[2];
            list[index] = item;
            programStructure.InputValues[Outputs[0].Id] = list;
        }
    }

    public class FindItem : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var list = inputs[0] as List<object>;
            var item = inputs[1];
            var index = list.IndexOf(item);
            programStructure.InputValues[Outputs[0].Id] = index;
        }
    }
    public class ObjectToList : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            var inputObject = inputs[0];
            List<object> resultList;

            // Check if the input object is already a list
            if (inputObject is List<object> inputList)
            {
                // If it's a list, use it directly
                resultList = inputList;
            }
            else
            {
                // If it's not a list, create a new list and add the object to it
                resultList = new List<object> { inputObject };
            }

            // Set the result list as the output
            programStructure.InputValues[Outputs[0].Id] = resultList;
        }
    }

    public class CountList : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            List<Object> list = new List<Object>((IEnumerable<Object>)inputs[0]);

            if (list == null)
            {
                list = new List<object>();
                return;
            }
            var Count = list.Count();

            programStructure.InputValues[Outputs[0].Id] = Count;
        }
    }

    public class LastItem : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            List<Object> list = new List<Object>((IEnumerable<Object>)inputs[0]);

            if (list == null)
            {
                list = new List<object>();
                return;
            }
            var LastItem = list.Last();

            programStructure.InputValues[Outputs[0].Id] = LastItem;
        }
    }

    public class CombineObjects : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            var combinedList = new List<object>();
            foreach (var input in inputs)
            {
                combinedList.Add(input);
            }

            programStructure.InputValues[Outputs[0].Id] = combinedList;
        }
    }
}
