using System;
using System.Collections.Generic;
using System.IO;

namespace RangHo.DialogueScript.DialogueShell
{
	/// <summary>
	/// Main Entry Point
	/// </summary>
	public static class Program
	{
        private static bool verbose = false;

        private static string WhatShouldIDo = null;

        private static string FilePath = null;

        public static void Main(string[] args)
		{
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
			
			if (WhatShouldIDo == null)
				WhatShouldIDo = "interprete";

#if DEBUG
            verbose = true;
#endif

            if (WhatShouldIDo == "token")
				TokenShit();
			else if (WhatShouldIDo == "ast")
				ASTShit();
			else if (WhatShouldIDo == "interprete")
				InterpreteShit();
			else
				Console.WriteLine("Something went terribly wrong.");
		}
		
		public static bool IsAST(object something, int number = 0)
		{
			if (something is AST)
			{
				int OriginalNumber = number;
				
				do
                {
					Console.Write("\t");
					number--;
				}
                while (number >= 0);
				number = OriginalNumber;
				Console.WriteLine("Type of inner AST: {0}", ((AST)something).ASTType);
				
				if (((AST)something).Target != null && !IsAST(((AST)something).Target as AST, number + 1)) {
					do
                    {
						Console.Write("\t");
						number--;
					}
                    while (number >= 0);
					number = OriginalNumber;
					Console.Write("Target of inner AST: ");
					Console.WriteLine(((AST)something).Target);
				}
				
				if (((AST)something).Value != null && !IsAST(((AST)something).Value as AST, number + 1)) {
					do
                    {
						Console.Write("\t");
						number--;
					}
                    while (number >= 0);
					Console.Write("Target of inner AST: ");
					Console.WriteLine(((AST)something).Value);
				}
				return true;
			}
			return false;
		}
		
		public static void TokenShit()
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
		
		public static void ASTShit()
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
		
		public static void InterpreteShit()
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

        public static Stream ToStream(this string str)
        {
            MemoryStream s = new MemoryStream();
            StreamWriter w = new StreamWriter(s);
            w.Write(str);
            w.Flush();
            s.Position = 0;
            return s;
        }
    }
}
