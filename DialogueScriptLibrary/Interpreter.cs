using System;
using System.Collections.Generic;

namespace RangHo.DialogueScript
{
	/// <summary>
	/// The class that finally interpretes the AST structure of provided script.
	/// </summary>
	public class Interpreter
	{
		public AST[] Input;
		
		public int InputPosition;
		
		public class Identifier
		{
			public string Name;
			
			public bool Checked;
		}
		
		private readonly IOutputManager OutputManager;
		
		private delegate object Interprete(AST target);
		
		public Interpreter(AST[] input, IOutputManager output)
		{
			this.Input = input;
			this.InputPosition = 0;
			this.OutputManager = output;
		}
		
		public bool InterpreteNextAST()
		{
			if (IsInputDone())
				return false;
			
			try
			{
				switch (Input[InputPosition].ASTType)
				{
					case AST.Classification.Say:
						InterpreteSay();
						return true;
					case AST.Classification.Set:
						InterpreteSet();
						return true;
					case AST.Classification.Choice:
						InterpreteChoice();
						return true;
					case AST.Classification.Label:
						InterpreteLabel();
						return true;
					default:
						throw new InvalidStatementPassedException(string.Format("Invalid statement AST {0} was passed. An element such as Number cannot be a statement.", Input[InputPosition].ASTType));
				}
			}
			catch (Exception e)
			{
				OutputManager.Exception(e);
			}
			
			return true;
		}
		
		public void InterpreteSay()
		{
			AST CurrentAST = Input[InputPosition];
			Identifier speaker = InterpreteIdentifier((AST)CurrentAST.Target);
			string content = InterpreteString((AST)CurrentAST.Value) as string;
			OutputManager.Say(speaker.Name, content);
			
			InputPosition++;
		}
		
		public void InterpreteSet()
		{
			AST CurrentAST = Input[InputPosition];
			if ((CurrentAST.Target as AST).ASTType == AST.Classification.Of)
			{
				InterpreteOf(CurrentAST.Target as AST);
				OutputManager.Set(Maybe(CurrentAST.Value as AST, InterpreteIdentifier, InterpreteNumber, InterpreteBoolean, InterpreteString));
				return;
			}
			string property = InterpreteIdentifier(CurrentAST.Target as AST).Name;
			OutputManager.Select(property);
			OutputManager.Set(Maybe(CurrentAST.Value as AST, InterpreteIdentifier, InterpreteNumber, InterpreteBoolean, InterpreteString));
			
			InputPosition++;
		}
		
		public void InterpreteLabel()
		{
			OutputManager.Finish();
		}
		
		public void InterpreteChoice()
		{
			Dictionary<string, string> Choices = new Dictionary<string, string>();
			string ChosenLabel = null;
			
			for (int index = InputPosition; Input[index].ASTType == AST.Classification.Choice; index++)
			{
				string label = InterpreteString(Input[index].Target as AST) as string;
				string content = InterpreteString(Input[index].Value as AST) as string;
				Choices.Add(label, content);
				
				InputPosition++;
			}
			
			OutputManager.Choices(Choices, ref ChosenLabel);
			LocateLabel(ChosenLabel);
		}
		
		public void InterpreteOf(AST target)
		{
			string container = InterpreteIdentifier(target.Target as AST).Name;
			string property = InterpreteIdentifier(target.Value as AST).Name;
			OutputManager.Select(property, container);
		}
		
		public Identifier InterpreteIdentifier(AST target)
		{
			if (OutputManager.FindObject(target.Value as string) == null)
				return null;
			Identifier result = new Identifier();
			result.Name = target.Value as string;
			result.Checked = true;
			return result;
		}
		
		public object InterpreteNumber(AST target)
		{
			int result;
			bool parsable = int.TryParse(target.Value as string, out result);
			if (!parsable)
				return null;
			return result;
		}
		
		public object InterpreteString(AST target)
		{
			return (string)target.Value;
		}
		
		public object InterpreteBoolean(AST target)
		{
			if (target.Value is bool)
				return (bool)target.Value;
			return null;
		}
		
		private bool IsInputDone()
		{
			return Input.Length <= InputPosition;
		}
		
		private void LocateLabel(string name)
		{
			for (int index = 0; index < Input.Length; index++)
			{
				if (Input[index].ASTType == AST.Classification.Label &&
				    Input[index].Value as string == name)
				{
					InputPosition = index + 1;
					return;
				}
			}
			throw new InvalidStatementPassedException(string.Format("Cannot find label {0}. Did you make a typo?", name));
		}
		
		private object Maybe(AST target, params Interprete[] interpreters)
		{
			foreach (Interprete interpreter in interpreters)
				if (interpreter(target) != null)
					return interpreter(target);
			return null;
		}
	}
}
