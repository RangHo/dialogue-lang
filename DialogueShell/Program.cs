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
			bool verbose = false;
			string WhatShouldIDo = string.Empty;
			string FilePath = string.Empty;
			foreach (string arg in args)
			{
				switch (arg)
				{
					case "-v":
						verbose = true;
						break;
					
					case "--Token":
						WhatShouldIDo = "token";
						break;
					
					case "--AST":
						WhatShouldIDo = "ast";
						break;
						
					case "--Interprete":
						WhatShouldIDo = "interprete";
						break;
						
					default:
						if (File.Exists(arg))
							FilePath = arg;
						break;
				}
			}
			
			if (WhatShouldIDo == string.Empty)
				WhatShouldIDo = "interprete";
			
			if (FilePath == string.Empty)
				if (WhatShouldIDo == "token")
					TokenShit(verbose);
				else if (WhatShouldIDo == "ast")
					ASTShit(verbose);
				else if (WhatShouldIDo == "interprete")
					InterpreteShit(verbose);
				else
					Console.WriteLine("Something went terribly wrong.");
			else
				if (WhatShouldIDo == "token")
					TokenShit(verbose, FilePath);
				else if (WhatShouldIDo == "ast")
					ASTShit(verbose, FilePath);
				else if (WhatShouldIDo == "interprete")
					InterpreteShit(verbose, FilePath);
				else
					Console.WriteLine("Something went terribly wrong.");
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
		
		public static void TokenShit(bool verbose = false, string FilePath = null)
		{
			try
			{
				int count = 0;
				List<Token> tokens = new List<Token>();
				Tokenizer t;
				
				if (FilePath == null)
				{
					Console.WriteLine("Write a DialogueScript Expression below. The DialogueScript Lexer will try to tokenize the expression.");
					Console.Write(">>> "); string expression = Console.ReadLine();
					Console.WriteLine("- - - - - - - - - - - - - - -");
					
					if (expression == "exit")
						return;
					
					t = new Tokenizer(expression.ToStream());
				}
				else
					using (FileStream fs = new FileStream(FilePath, FileMode.Open))
						t = new Tokenizer(fs);
				
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
		
		public static void ASTShit(bool verbose = false, string FilePath = null)
		{
			while (true)
			{
				try
				{
					int count = 0;
					List<Token> tokens = new List<Token>();
					Tokenizer t;
					
					if (FilePath == null)
					{
						Console.WriteLine("Write a DialogueScript Expression below. The DialogueScript Lexer will try to tokenize the expression.");
						Console.Write(">>> "); string expression = Console.ReadLine();
						Console.WriteLine("- - - - - - - - - - - - - - -");
						
						if (expression == "exit")
							return;
						
						t = new Tokenizer(expression.ToStream());
					}
					else
						using (FileStream fs = new FileStream(FilePath, FileMode.Open))
							t = new Tokenizer(fs);
					
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
		
		public static void InterpreteShit(bool verbose = false, string FilePath = null)
		{
			ShellOutputManager om = new ShellOutputManager();
			
			while (true)
			{
				try
				{
					int count = 0;
					List<Token> tokens = new List<Token>();
					Tokenizer t;
					
					if (FilePath == null)
					{
						Console.WriteLine("Write a DialogueScript Expression below. The DialogueScript Lexer will try to tokenize the expression.");
						Console.Write(">>> "); string expression = Console.ReadLine();
						Console.WriteLine("- - - - - - - - - - - - - - -");
						
						if (expression == "exit")
							return;
						
						t = new Tokenizer(expression.ToStream());
					}
					else
						using (FileStream fs = new FileStream(FilePath, FileMode.Open))
							t = new Tokenizer(fs);
					
					Console.WriteLine("==============================");
					while (true)
					{
						Token parsed = t.ReadNextToken();
						if (parsed == null) break;
						if (verbose)
						{
							Console.WriteLine("Type of Token {0}: {1}", count, parsed.TokenType);
							Console.WriteLine("Content of Token {0}: {1}", count, parsed.Content);
							Console.WriteLine("==============================");
						}
						tokens.Add(parsed);
						count++;
					}
					Console.WriteLine("Tokenization complete.\n");
					
					count = 0;
					List<AST> asts = new List<AST>();
					Parser p = new Parser(tokens.ToArray());
					Console.WriteLine("==============================");
					while (true)
					{
						AST parsed = p.ParseNextToken();
						if (parsed == null) break;
						if (verbose)
						{
							Console.WriteLine("Type of AST {0}: {1}", count, parsed.ASTType);
							IsAST(parsed.Target);
							IsAST(parsed.Value);
							Console.WriteLine("==============================");
						}
						asts.Add(parsed);
						count++;
					}
					Console.WriteLine("Parsing complete.\n");
					
					Interpreter i = new Interpreter(asts.ToArray(), om);
					Console.WriteLine("==============================");
					while (true)
					{
						if (i.InterpreteNextAST()) break;
						Console.WriteLine("==============================");
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
	}
}
