using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using RangHo.DialogueScript;

namespace RangHo.DialogueScript.DialogueShell
{
	public sealed class ShellOutputManager : IOutputManager
	{
		private Dictionary<string, object> RegisteredObjects;
		
		public string SelectedObject;
		
		public class SampleObject
		{
			public int Number = 0;
			
			public string String = string.Empty;
		}
		
		public ShellOutputManager()
		{
			RegisteredObjects = new Dictionary<string, object>();
			RegisteredObjects.Add("Character", "Good Name");
			RegisteredObjects.Add("Scene", "Good Scene");
			RegisteredObjects.Add("Object", new SampleObject());
		}
		
		public void Say(object speaker, string content)
		{
			Console.WriteLine("{0} says, \"{1}\"", (string)speaker, content);
		}
		
		public void Set(object value)
		{
			if (SelectedObject.IndexOf('.') >= 0)
			{
				// TODO: Implement reflection searching
			}
			else
				RegisteredObjects[SelectedObject] = value;
			
			Console.WriteLine("Property {0} was changed to {1}", SelectedObject, value);
		}
		
		public void RegisterObject(object target, string name)
		{
			Console.WriteLine("This feature is not suppored yet.");
		}
		
		public object FindObject(string name)
		{
			object result;
			if (!RegisteredObjects.TryGetValue(name, out result))
				return null;
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
			Console.WriteLine("Following Exception was thrown: {0}", e.GetType());
			Console.WriteLine("Message	  : {0}", e.Message);
			Console.WriteLine("Source	  : {0}", e.Source);
			Console.WriteLine("StackTrace : {0}", e.StackTrace);
			Console.WriteLine("\nRestarting Interpreter...");
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
			int ChoiceNumber = 0;
			int.TryParse(Console.ReadLine(),out ChoiceNumber);
			
			Console.WriteLine("You chose {0}. Going to label {1}...", ChoicesDictionary.Values.ToList()[ChoiceNumber], ChoicesDictionary.Keys.ToList()[ChoiceNumber]);
			
			ChosenLabel = ChoicesDictionary.Keys.ToList()[ChoiceNumber];
		}
		
		public void Finish()
		{
			
		}
	}
}
