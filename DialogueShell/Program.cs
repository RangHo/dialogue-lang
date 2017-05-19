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
#if DEBUG
        private static bool verbose = true;
#else
        private static bool verbose = false;
#endif

        private static string WhatShouldIDo = "interprete";

        private static string FilePath = null;

        private static IOutputManager Output = new ShellOutputManager();

        public static void Main(string[] args)
		{
            bool RegisterFlag = false;
			foreach (string arg in args)
			{
                if (RegisterFlag)
                {
                    Output.RegisterObject(new object(), arg);
                    RegisterFlag = false;
                    continue;
                }

				switch (arg)
				{
					case "-v":
						verbose = true;
						break;
					
					case "--token":
						WhatShouldIDo = "token";
						break;
					
					case "--ast":
						WhatShouldIDo = "ast";
						break;
						
					case "--interprete":
						WhatShouldIDo = "interprete";
						break;

                    case "--register":
                        RegisterFlag = true;
                        break;

					default:
                        if (File.Exists(arg))
                            FilePath = arg;
                        else
                            Console.WriteLine("The given file was not found.");
						break;
				}
			}

            Console.WriteLine("DialogueShell - DialogueScript command line tester and parser");

            Stream Source;

            if (FilePath == null)
            {
                while (true)
                {
                    Console.Write(">>> "); string Expression = Console.ReadLine();
                    Console.WriteLine("- - - - - - - - - - - - - - -");

                    if (Expression.ToLower() == "exit")
                        return;

                    try
                    {
                        using (Source = Expression.ToStream())
                            DoEverything(Source, Output);
                    }
                    catch (Exception e)
                    {
                        Output.Exception(e);
                        Console.WriteLine("Restarting DialogueShell... \n\n");
                    }
                }
            }
            else
            {
                try
                {
                    using (Source = new FileStream(FilePath, FileMode.Open))
                        DoEverything(Source, Output);
                }
                catch (Exception e)
                {
                    Output.Exception(e);
                    Console.WriteLine("\nPress any key to close DialogueShell...");
                    Console.ReadKey();
                }
            }
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

        public static Stream ToStream(this string str)
        {
            MemoryStream s = new MemoryStream();
            StreamWriter w = new StreamWriter(s);
            w.Write(str);
            w.Flush();
            s.Position = 0;
            return s;
        }

        public static List<Token> TokenizeIt(Stream input)
        {
            List<Token> Tokens = new List<Token>();
            Tokenizer Automata = new Tokenizer(input);

            int count = 0;
            while (true)
            {
                Token Parsed = Automata.ReadNextToken();
                if (Parsed == null) break;
                if (verbose)
                {
                    Console.WriteLine("Type of Token {0}: {1}", count, Parsed.TokenType);
                    Console.WriteLine("Content of Token {0}: {1}", count, Parsed.Content);
                    Console.WriteLine("==============================");
                }
                Tokens.Add(Parsed);
                count++;
            }

            return Tokens;
        }

        public static List<AST> ParseIt(List<Token> tokens)
        {
            List<AST> ASTs = new List<AST>();
            Parser Automata = new Parser(tokens.ToArray());

            int count = 0;
            while (true)
            {
                AST Parsed = Automata.ParseNextToken();
                if (Parsed == null) break;
                if (verbose)
                {
                    Console.WriteLine("Type of AST {0}: {1}", count, Parsed.ASTType);
                    IsAST(Parsed.Target);
                    IsAST(Parsed.Value);
                    Console.WriteLine("==============================");
                }
                ASTs.Add(Parsed);
                count++;
            }

            return ASTs;
        }

        public static void InterpreteIt(List<AST> asts, IOutputManager om)
        {
            Interpreter Automata = new Interpreter(asts.ToArray(), om);
            Automata.ScanForLabels();
            while (true)
            {
                if (!Automata.InterpreteNextAST()) break;
            }
        }

        public static void DoEverything(Stream source, IOutputManager om)
        {

            List<Token> Tokens = TokenizeIt(source);
            if (WhatShouldIDo != "token")
            {
                List<AST> ASTs = ParseIt(Tokens);
                if (WhatShouldIDo != "ast")
                {
                    if (FilePath != null)
                    {
                        Console.WriteLine("Tokenizing and Parsing finished.\n");
                        Console.ReadKey();
                        Console.Clear();
                    }
                    InterpreteIt(ASTs, om);
                }
            }
        }
    }
}
