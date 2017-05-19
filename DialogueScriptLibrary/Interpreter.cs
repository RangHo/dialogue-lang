using System;
using System.Collections.Generic;

namespace RangHo.DialogueScript
{
	/// <summary>
	/// The class that finally interpretes the AST structure of provided script.
	/// </summary>
	public class Interpreter
	{
		public GenericStream<AST> Input;
		
		private readonly IOutputManager OutputManager;
		
		private delegate object Interprete(AST target);

        private bool LabelScanned = false;
		
		public Interpreter(AST[] input, IOutputManager output)
		{
			this.Input = new GenericStream<AST>(input);
			this.OutputManager = output;
		}
		
		public bool InterpreteNextAST()
		{
			if (Input.IsEnd())
				return false;
			
			try
			{
				switch (Input.Peek().ASTType)
				{
                    // Valid cases...
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
                    case AST.Classification.Jump:
                        InterpreteJump();
                        return true;
                    case AST.Classification.Return:
                        InterpreteReturn();
                        return true;

                    case AST.Classification.LineBreak:
                        break;
                    
                    // Some invalid cases... Not a statement.
                    case AST.Classification.Identifier:
                        throw new InvalidStatementPassedException("An identifier cannot be used as a statement.");
                    case AST.Classification.String:
                        throw new InvalidStatementPassedException("A string cannot be used as a statement.");
                    case AST.Classification.Boolean:
                        throw new InvalidStatementPassedException("A boolean value cannot be used as a statement.");
                    case AST.Classification.Number:
                        throw new InvalidStatementPassedException("A number value cannot be used as a statement.");
                    case AST.Classification.Of:
                        throw new InvalidStatementPassedException("Of statements cannot be used alone. They must be used with Set statements.");
					default:
						throw new InvalidStatementPassedException($"Invalid statement AST {Input.Peek().ASTType} was passed. An element such as Number cannot be a statement.");
				}

                if (Input.Peek().ASTType != AST.Classification.LineBreak)
                    throw new InvalidStatementPassedException("Each statements must be separated by line break.");

                Input.Read();       // Remove the line break AST
			}
			catch (Exception e)
			{
				OutputManager.Exception(e);
			}
			
			return true;
		}

        public void ScanForLabels()
        {
            while (!Input.IsEnd())
            {
                if (Input.Peek().ASTType == AST.Classification.Label)
                    InterpreteLabel();
                else
                    Input.Read();
            }
            Input.RecoverPosition(Input.Beginning);
            LabelScanned = true;
        }
		
		public void InterpreteSay()
		{
			AST CurrentAST = Input.Read();
			string speaker = InterpreteIdentifier((AST)CurrentAST.Target) as string;
			string content = InterpreteString((AST)CurrentAST.Value) as string;
			OutputManager.Say(speaker, content);
		}
		
		public void InterpreteSet()
		{
			AST CurrentAST = Input.Read();
			if ((CurrentAST.Target as AST).ASTType == AST.Classification.Of)
			{
				InterpreteOf(CurrentAST.Target as AST);
				OutputManager.Set(Maybe(CurrentAST.Value as AST, InterpreteIdentifier, InterpreteNumber, InterpreteBoolean, InterpreteString));
				return;
			}
			string property = InterpreteIdentifier(CurrentAST.Target as AST) as string;
			OutputManager.Select(property);
			OutputManager.Set(Maybe(CurrentAST.Value as AST, InterpreteIdentifier, InterpreteNumber, InterpreteBoolean, InterpreteString));
		}
		
		public void InterpreteChoice()
		{
			Dictionary<string, string> Choices = new Dictionary<string, string>();
			string ChosenLabel = null;
			
			while (Expected(AST.Classification.Choice))
            {
                AST NextChoiceAST = Input.Read();
                string label = InterpreteIdentifier(NextChoiceAST.Target as AST) as string;
                string content = InterpreteString(NextChoiceAST.Value as AST) as string;
                Choices.Add(label, content);
                if (Expected(AST.Classification.LineBreak, RaiseException: true))
                    Input.Read();       // Get rid of the Line Break AST 
            }
			
			OutputManager.Choices(Choices, ref ChosenLabel);
			LocateLabel(ChosenLabel);
        }

        public void InterpreteLabel()
        {
            if (!LabelScanned)
            {
                AST CurrentAST = Input.Read();
                string Name = ((AST)CurrentAST.Value).Value as string;
                OutputManager.RegisterLabelLocation(Name, Input.BackupPosition());
            }
        }

        public void InterpreteJump()
        {
            AST CurrentAST = Input.Read();
            string name = ((AST)CurrentAST.Value).Value as string;
            int result = OutputManager.RetrieveLabelLocation(name);
            if (result == -1)
                throw new InvalidStatementPassedException($"Label {name} was not registered. Please check your script again.");
            Input.RecoverPosition(result);
        }

        public void InterpreteReturn()
        {
            Input.Read();
            OutputManager.Finish();
        }

        public void InterpreteOf(AST target)
		{
			string container = InterpreteIdentifier(target.Target as AST) as string;
			string property = InterpreteIdentifier(target.Value as AST) as string;
			OutputManager.Select(property, container);
		}
		
		public object InterpreteIdentifier(AST target)
		{
            if (Expected(AST.Classification.Identifier, target))
				return target.Value as string;
            return null;
		}
		
		public object InterpreteNumber(AST target)
		{
			if (Expected(AST.Classification.Number, target))
				return target.Value;
			return null;
		}
		
		public object InterpreteString(AST target)
		{
			if (Expected(AST.Classification.String, target))
				return target.Value;
			return null;
		}
		
		public object InterpreteBoolean(AST target)
		{
			if (Expected(AST.Classification.Boolean, target))
				return (bool)target.Value;
			return null;
		}
		


        // Some tool methods to make my life easier

		private void LocateLabel(string name)
		{
            int Location = OutputManager.RetrieveLabelLocation(name);
            if (Location < 0)
                throw new LabelNotFoundException($"Label {name} is not found. Please check once again.");
            else
                Input.RecoverPosition(Location);
		}
		
		private object Maybe(AST target, params Interprete[] interpreters)
		{
			foreach (Interprete interpreter in interpreters)
				if (interpreter(target) != null)
					return interpreter(target);
			return null;
		}

        private bool Expected(AST.Classification ExpectedASTType, AST NextAST = null, bool RaiseException = false)
        {
            // If NextAST is null, then peek the AST in next queue
            if (NextAST == null)
                NextAST = Input.Peek();

            if ((NextAST.ASTType & ExpectedASTType) != NextAST.ASTType)
                if (RaiseException)
                    throw new UnexpectedStatementException($"Unexpected {NextAST.ASTType} AST was provided. {ExpectedASTType} AST was expected. Please check your script again.");
                else
                    return false;

            return true;
        }
	}
}
