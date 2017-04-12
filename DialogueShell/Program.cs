using System;
using System.Collections.Generic;
using System.IO;
using RangHo.DialogueScript;

namespace RangHo.DialogueScript.DialogueShell
{
	/// <summary>
	/// Main Entry Point
	/// </summary>
	public static class Program
	{
		public static void Main(string[] args)
		{
			InterpreterShit();
		}
		
		public static bool IsAST(object something, int number = 0)
		{
			if (something is AST)
			{
				int OriginalNumber = number;
				
				do {
					Console.Write("\t");
					number--;
				} while (number >= 0);
				number = OriginalNumber;
				Console.WriteLine("Type of inner AST: {0}", ((AST)something).ASTType);
				
				if (((AST)something).Target != null && !IsAST(((AST)something).Target as AST, number + 1)) {
					do {
						Console.Write("\t");
						number--;
					} while (number >= 0);
					number = OriginalNumber;
					Console.Write("Target of inner AST: ");
					Console.WriteLine(((AST)something).Target);
				}
				
				if (((AST)something).Value != null && !IsAST(((AST)something).Value as AST, number + 1)) {
					do {
						Console.Write("\t");
						number--;
					} while (number >= 0);
					number = OriginalNumber;
					Console.Write("Target of inner AST: ");
					Console.WriteLine(((AST)something).Value);
				}
				
				return true;
			}
			return false;
		}
		
		public static Stream ToStream(this string str)
		{
			MemoryStream s = new MemoryStream();
			StreamWriter w = new StreamWriter(s);
			w.Write(str);
			w.Flush();
			s.Position = 0;
			return s;
		}
		
		public static void TokenShit()
		{
			try
			{
				Console.WriteLine("Write a DialogueScript Expression below. The DialogueScript Lexer will try to tokenize the expression.");
				Console.Write(">>> "); string expression = Console.ReadLine();
				Console.WriteLine("- - - - - - - - - - - - - - -");
				
				if (expression == "exit")
					return;
				
				int count = 0;
				List<Token> tokens = new List<Token>();
				Tokenizer t = new Tokenizer(expression.ToStream());
				Console.WriteLine("==============================");
				while (true)
				{
					Token parsed = t.ReadNextToken();
					if (parsed == null) break;
					Console.WriteLine("Type of Token {0}: {1}", count, parsed.TokenType);
					Console.WriteLine("Content of Token {0}: {1}", count, parsed.Content);
					Console.WriteLine("==============================");
					tokens.Add(parsed);
					count++;
				}
				Console.WriteLine("Done.\n\n");
			}
			catch (Exception e)
			{
				Console.WriteLine("==============================");
				Console.WriteLine("!!! Exception thrown !!!");
				Console.WriteLine("Following Exception was thrown: {0}", e.GetType());
				Console.WriteLine("Message	  : {0}", e.Message);
				Console.WriteLine("Source	  : {0}", e.Source);
				Console.WriteLine("StackTrace : {0}", e.StackTrace);
				Console.WriteLine("==============================");
			}
		}
		
		public static void ASTShit()
		{
			while (true)
			{
				try
				{
					Console.WriteLine("Write a DialogueScript Expression below. The DialogueScript Parser will try to parse the expression.");
					Console.Write(">>> "); string expression = Console.ReadLine();
					Console.WriteLine("- - - - - - - - - - - - - - -");
					
					if (expression == "exit")
						return;
					
					int count = 0;
					List<Token> tokens = new List<Token>();
					Tokenizer t = new Tokenizer(expression.ToStream());
					Console.WriteLine("==============================");
					while (true)
					{
						Token parsed = t.ReadNextToken();
						if (parsed == null) break;
						Console.WriteLine("Type of Token {0}: {1}", count, parsed.TokenType);
						Console.WriteLine("Content of Token {0}: {1}", count, parsed.Content);
						Console.WriteLine("==============================");
						tokens.Add(parsed);
						count++;
					}
					Console.WriteLine("Tokenization complete.\n");
					
					count = 0;
					Parser p = new Parser(tokens.ToArray());
					Console.WriteLine("==============================");
					while (true)
					{
						AST parsed = p.ParseNextToken();
						if (parsed == null) break;
						Console.WriteLine("Type of AST {0}: {1}", count, parsed.ASTType);
						IsAST(parsed.Target);
						IsAST(parsed.Value);
						Console.WriteLine("==============================");
						count++;
					}
					Console.WriteLine("Done.\n\n");
				}
				catch (Exception e)
				{
					Console.WriteLine("==============================");
					Console.WriteLine("!!! Exception thrown !!!");
					Console.WriteLine("Following Exception was thrown: {0}", e.GetType());
					Console.WriteLine("Message	  : {0}", e.Message);
					Console.WriteLine("Source	  : {0}", e.Source);
					Console.WriteLine("StackTrace : {0}", e.StackTrace);
					Console.WriteLine("==============================");
				}
			}
		}
		
		public static void InterpreterShit()
		{
			while (true)
			{
				try
				{
					Console.WriteLine("Write a DialogueScript Expression below. The DialogueScript Parser will try to parse the expression.");
					Console.Write(">>> "); string expression = Console.ReadLine();
					Console.WriteLine("- - - - - - - - - - - - - - -");
					
					if (expression == "exit")
						return;
					
					#region Tokenizing
					int count = 0;
					List<Token> tokens = new List<Token>();
					Tokenizer t = new Tokenizer(expression.ToStream());
					Console.WriteLine("==============================");
					while (true)
					{
						Token parsed = t.ReadNextToken();
						if (parsed == null) break;
						Console.WriteLine("Type of Token {0}: {1}", count, parsed.TokenType);
						Console.WriteLine("Content of Token {0}: {1}", count, parsed.Content);
						Console.WriteLine("==============================");
						tokens.Add(parsed);
						count++;
					}
					Console.WriteLine("Tokenization complete.\n");
					#endregion
					
					#region Parsing
					count = 0;
					List<AST> asts = new List<AST>();
					Parser p = new Parser(tokens.ToArray());
					Console.WriteLine("==============================");
					while (true)
					{
						AST parsed = p.ParseNextToken();
						if (parsed == null) break;
						Console.WriteLine("Type of AST {0}: {1}", count, parsed.ASTType);
						IsAST(parsed.Target);
						IsAST(parsed.Value);
						Console.WriteLine("==============================");
						asts.Add(parsed);
						count++;
					}
					Console.WriteLine("Parsing complete.\n");
					#endregion
					
					#region Interpreting
					Interpreter i = new Interpreter(asts.ToArray(), new ShellOutputManager());
					Console.WriteLine("==============================");
					while (true)
					{
						if (i.InterpreteNextAST()) break;
						Console.WriteLine("==============================");
					}
					Console.WriteLine("Done.\n\n");
					#endregion
				}
				catch (Exception e)
				{
					Console.WriteLine("==============================");
					Console.WriteLine("!!! Exception thrown !!!");
					Console.WriteLine("Following Exception was thrown: {0}", e.GetType());
					Console.WriteLine("Message	  : {0}", e.Message);
					Console.WriteLine("Source	  : {0}", e.Source);
					Console.WriteLine("StackTrace : {0}", e.StackTrace);
					Console.WriteLine("==============================");
				}
			}
		}
	}
}
