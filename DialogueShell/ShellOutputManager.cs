using System;
using System.Collections.Generic;
using System.Reflection;
using RangHo.DialogueScript;

namespace RangHo.DialogueScript.DialogueShell
{
	public sealed class ShellOutputManager : IOutputManager
	{
		private readonly Dictionary<string, object> RegisteredObjects;
		
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
				this.Exception(new Exception("Cannot find the specified object."));
			return result;
		}
		
		public void Select(string target, string container = null)
		{
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
			
		}
		
		public void Finish()
		{
			
		}
	}
}
