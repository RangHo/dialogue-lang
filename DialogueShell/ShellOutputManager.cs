using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace RangHo.DialogueScript.DialogueShell
{
    public sealed class ShellOutputManager : IOutputManager
    {
        private Dictionary<string, object> RegisteredObjects;
        
        public string SelectedObject;
        
        public class SampleObject
        {
            public string left;

            public string right;
        }

        private Dictionary<string, int> RegisteredLabelPosition;
        
        public ShellOutputManager()
        {
            RegisteredObjects = new Dictionary<string, object>();
            RegisteredObjects.Add("character", "John Doe");
            RegisteredObjects.Add("scene", new SampleObject());

            RegisteredLabelPosition = new Dictionary<string, int>();
        }
        
        public void Say(object speaker, string content)
        {
            Console.WriteLine("{0} says, \"{1}\"", RegisteredObjects[(string)speaker], content);
        }
        
        public void Set(object value)
        {
            if (SelectedObject.IndexOf('.') >= 0)
            {
                string[] Separated = SelectedObject.Split('.');
                FieldInfo ContainerField = RegisteredObjects[Separated[0]].GetType().GetField(Separated[1], BindingFlags.Public | BindingFlags.Instance);
                if (ContainerField != null)
                    ContainerField.SetValue(RegisteredObjects[Separated[0]], value);
            }
            else
                RegisteredObjects[SelectedObject] = value;

            Console.WriteLine("Property {0} was changed to {1}", SelectedObject, value);
        }
        
        public void RegisterObject(object target, string name)
        {
            RegisteredObjects.Add(name, target);
            Console.WriteLine("New object {0} was registered successfully. The value of {0} is {1}.", name, target);
        }
        
        public object FindObject(string name)
        {
            if (!RegisteredObjects.TryGetValue(name, out object result))
                throw new InvalidStatementPassedException($"Object {name} was not found in this context.");
            return result;
        }
        
        public void Select(string target, string container = null)
        {
            if (container == null)
                SelectedObject = target;
            else
                SelectedObject = string.Join(".", new string[] {container, target});
        }
        
        public void Exception(Exception e)
        {
            Console.WriteLine("==============================");
            Console.WriteLine("!!! Exception thrown !!!");
            Console.WriteLine("Following Exception was thrown: {0}\n", e.GetType());
            Console.WriteLine("Message      : {0}", e.Message);
            Console.WriteLine("Source       : {0}\n", e.Source);
            Console.WriteLine("StackTrace   :\n{0}", e.StackTrace);
            Finish();
        }
        
        public void Choices(Dictionary<string, string> ChoicesDictionary, ref string ChosenLabel)
        {
            Console.WriteLine("Please make a choice.");
            
            int count = 0;
            foreach (KeyValuePair<string, string> Choice in ChoicesDictionary)
            {
                Console.WriteLine("\t{0}: {1}", count, Choice.Value);
                count++;
            }
            
            Console.Write("Input a number here: ");
            int.TryParse(Console.ReadLine(), out int ChoiceNumber);

            if (ChoiceNumber >= ChoicesDictionary.Count)
            {
                Console.WriteLine("Invalid number was provided.");
                Console.WriteLine("Press any key to exit...");
            }
            
            Console.WriteLine("You chose {0}. Going to label {1}...", ChoicesDictionary.Values.ToList()[ChoiceNumber], ChoicesDictionary.Keys.ToList()[ChoiceNumber]);
            
            ChosenLabel = ChoicesDictionary.Keys.ToList()[ChoiceNumber];
        }
        
        public void Finish()
        {
            Console.WriteLine("\nDone.");
            Console.ReadKey();
        }

        public void RegisterLabelLocation(string name, int position)
        {
            RegisteredLabelPosition.Add(name, position + 1);
            Console.WriteLine($"Label {name} located in {position - 1} was successfully registered.");
        }

        public int RetrieveLabelLocation(string name)
        {
            if (!RegisteredLabelPosition.TryGetValue(name, out int position))
                position = -1;
            return position;
        }
    }
}
