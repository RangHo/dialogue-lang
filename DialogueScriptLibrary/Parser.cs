using System;

namespace RangHo.DialogueScript
{
	/// <summary>
	/// Description of Parser.
	/// </summary>
	public class Parser
	{
		public Parser(Token[] tokens)
		{
			this.Input = tokens;
			this.InputPosition = 0;
		}
		
		public Token[] Input;
		
		public int InputPosition;
		
		public delegate AST Parse();
		
		public AST ParseNextToken()
		{
			if(IsInputDone())
				return null;
			
			AST ParsedAST = null;
			int OriginalPosition = InputPosition;
			
			if (IsKeyword("set"))
				ParsedAST = ParseSet();
			if (IsKeyword("true") || IsKeyword("false"))
				ParsedAST = ParseBoolean();
			if (IsKeyword("choice"))
			    ParsedAST = ParseChoice();
			if (IsKeyword("label"))
				ParsedAST = ParseLabel();
			if (!IsInputDone() && Input[InputPosition].TokenType == Token.Classification.Number)
				ParsedAST = ParseNumber();
			if (!IsInputDone() && Input[InputPosition].TokenType == Token.Classification.String)
				ParsedAST = ParseString();
			if (!IsInputDone() && Input[InputPosition].TokenType == Token.Classification.Identifier)
				ParsedAST = Maybe(ParseSay, ParseOf);
			if (!IsInputDone() && Input[InputPosition].TokenType == Token.Classification.LineBreak)
				ParsedAST = AST.CreateLineBreakAST();
			
			if (ParsedAST.ASTType != AST.Classification.LineBreak)
			{
				if (Input.Length <= InputPosition || Input[InputPosition + 1].TokenType == Token.Classification.LineBreak)
					return ParsedAST;
			}
			else
			{
				return ParseNextToken();
			}
			
			throw new UnexpectedTokenException(string.Format("Unexpected AST parsed: {0} ({1} expected).", ParseNextToken().ASTType, AST.Classification.LineBreak));
		}
		
		public AST ParseSay()
		{
			AST result = null;
			if (!IsInputDone() &&
				Input[InputPosition + 1].TokenType == Token.Classification.Punctuation && Input[InputPosition + 1].Content == ":" &&
			    Input[InputPosition + 2].TokenType == Token.Classification.String)
				result = AST.CreateSayAST(AST.CreateIdentifierAST(Input[InputPosition].Content), AST.CreateStringAST(Input[InputPosition + 2].Content));
			this.InputPosition += 3;
			return result;
		}
		
		public AST ParseSet()
		{
			AST result = null, target = null;
			if (!IsInputDone() &&
				Input[++InputPosition].TokenType == Token.Classification.Identifier)	// Make sure the next token is an identifier
				target = Maybe(ParseOf, ParseIdentifier);		// Statement can be followed by Of statement or Identifier statement.
			if (Input[InputPosition].TokenType == Token.Classification.Number ||
			    Input[InputPosition].TokenType == Token.Classification.String ||
			    Input[InputPosition].TokenType == Token.Classification.Identifier)
				result = AST.CreateSetAST(target, Maybe(ParseNumber, ParseString, ParseIdentifier));
			return result;
		}
		
		public AST ParseOf()
		{
			AST result = null, target = null, property = null;
			if (!IsInputDone() &&
				Input[InputPosition + 1].TokenType == Token.Classification.Keyword && Input[InputPosition + 1].Content == "of" &&
			    Input[InputPosition + 2].TokenType == Token.Classification.Identifier)
			{
				target = AST.CreateIdentifierAST(Input[InputPosition + 2].Content);
				property = AST.CreateIdentifierAST(Input[InputPosition].Content);
				result = AST.CreateOfAST(target, property);
				this.InputPosition += 3;
			}
			
			return result;
		}
		
		public AST ParseChoice()
		{
			AST result = null;
			if (!IsInputDone() &&
				Input[InputPosition + 1].TokenType == Token.Classification.Identifier &&
			    Input[InputPosition + 2].TokenType == Token.Classification.Punctuation && Input[InputPosition + 2].Content == ":" &&
			    Input[InputPosition + 3].TokenType == Token.Classification.String)
				result = AST.CreateChoiceAST(
							 AST.CreateIdentifierAST(Input[InputPosition + 1].Content),
							 AST.CreateStringAST(Input[InputPosition + 3].Content));
			this.InputPosition += 4;
			return result;
		}
		
		public AST ParseLabel()
		{
			return !IsInputDone() ? AST.CreateLabelAST(Input[++InputPosition].Content) : null;
		}
		
		public AST ParseBoolean()
		{
			return (!IsInputDone() && Input[InputPosition++].Content == "true") ? AST.CreateBooleanAST(true) : AST.CreateBooleanAST(false);
		}
		
		public AST ParseNumber()
		{
			int result = 0;
			return (!IsInputDone() && int.TryParse(Input[InputPosition++].Content, out result)) ? AST.CreateNumberAST(result) : null;
		}
		
		public AST ParseString()
		{
			return (!IsInputDone() && Input[InputPosition].TokenType == Token.Classification.String) ? AST.CreateStringAST(Input[InputPosition++].Content) : null;
		}
		
		public AST ParseIdentifier()
		{
			return !IsInputDone() ? AST.CreateIdentifierAST(Input[InputPosition++].Content) : null;
		}
		
		private bool IsKeyword(string keyword)
		{
			if (!IsInputDone() &&
				Input[InputPosition].TokenType == Token.Classification.Keyword &&
			    Input[InputPosition].Content.ToLower() == keyword.ToLower())
				return true;
			return false;
		}
		
		private AST Maybe(params Parse[] parsers)
		{
			AST result = null;
			int PreviousInputPosition = InputPosition;
			foreach (Parse parser in parsers)
			{
				result = parser();
				if (result == null)
				{
					InputPosition = PreviousInputPosition;
					continue;
				}
				return result;
			}
			throw new UnexpectedTokenException("Unknown error happened. Please check the input array.");
		}
		
		private bool IsInputDone()
		{
			return Input.Length <= InputPosition;
		}
	}
}
