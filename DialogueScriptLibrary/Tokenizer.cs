using System;
using System.IO;
using System.Linq;
using System.Text;

namespace RangHo.DialogueScript
{        
    public class Tokenizer
    {
        public delegate bool Predicate(char target);
        
        public StreamReader Input { get; set; }
        
        public Tokenizer(Stream script)
        {
        	this.Input = new StreamReader(script);
        }
        
        public string ReadWhile(Predicate CharacterChecker)
        {
        	StringBuilder str = new StringBuilder();
        	while (Input.Peek() > -1 && CharacterChecker((char)Input.Peek()))
        	{
        		str.Append((char)Input.Read());
        	}
        	return str.ToString();
        }

        public string ReadWhile(char UntilThisCharacter)
        {
            StringBuilder str = new StringBuilder();
            while (Input.Peek() != UntilThisCharacter)
            {
                char temp = (Input.EndOfStream) ? throw new InvalidCharacterException("The tokenizer reached EOF while processing a token.") : (char)Input.Read();

                if (temp == '\n')
                    throw new InvalidCharacterException("Line break happened while reading a token.");

                str.Append(temp);
            }
            Input.Read();       // Remove the extra " character
            return str.ToString();
        }
        
        public Token ReadNextToken()
        {
        	ReadWhile(Predicates.IsWhitespace);
        	if (Input.Peek() <= 0)
        		return null;
        	char target = (char)Input.Peek();
        	
        	if (target == '#')
        		return SkipComment();
        	if (target == '"')
        		return ReadString();
            if (target == '\r')                 // FUCK YOU CRLF
                return ReadLineBreak(true);
        	if (target == '\n')
        		return ReadLineBreak();
        	if (Predicates.IsDigit(target))
        		return ReadNumber();
        	if (Predicates.IsStartingIdentifier(target))
        		return ReadIdentifier();
        	if (Predicates.IsPunctuation(target))
        		return new Token(Token.Classification.Punctuation, ((char)Input.Read()).ToString());
        	
        	throw new InvalidCharacterException(string.Format("Cannot handle the following character: {0}", target));
        	
        }
        
        public Token SkipComment()
        {
        	Input.ReadLine();
        	return ReadNextToken();
        }
        
        public Token ReadString()
        {
            Input.Read();           // Removes the extra " character at the beginning of token
            StringBuilder str = new StringBuilder(ReadWhile('"'));

            // Replace some escaped characters
            str.Replace("\\n", "\n");
            str.Replace("\\t", "\t");

            return new Token(Token.Classification.String, str.ToString());
        }
        
        public Token ReadIdentifier()
        {
        	string str = ReadWhile(Predicates.IsIdentifier);
        	return Predicates.IsKeyword(str) ? new Token(Token.Classification.Keyword, str) : new Token(Token.Classification.Identifier, str);
        }
        
        public Token ReadNumber()
        {
        	string str = ReadWhile(Predicates.IsDigit);
        	return new Token(Token.Classification.Number, str);
        }
        
        public Token ReadLineBreak(bool CRLF = false)
        {
        	Input.Read();
            if (CRLF)
                Input.Read();
        	return new Token(Token.Classification.LineBreak, "Line Break");
        }
    }
    
    public static class Predicates
    {
        private static readonly string[] keywords = { "set", "of", "null", "true", "false", "done", "label", "choice", "return", "jump" };

        public static string[] Keywords => keywords;

        public static bool IsWhitespace(char target)
        {
            return " \t".IndexOf(target) >= 0;
        }
        
        public static bool IsDigit(char target)
        {
            return "0123456789".IndexOf(target) >= 0;
        }
        
        public static bool IsStartingIdentifier(char target)
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_".IndexOf(target) >= 0;
        }
        
        public static bool IsIdentifier(char target)
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_0123456789".IndexOf(target) >= 0;
        }
        
        public static bool IsOperator(char target)
        {
            // TODO: Is operator needed? I don't know.
            return false;
        }
        
        public static bool IsPunctuation(char target)
        {
            return ":\"'`".IndexOf(target) >= 0;
        }
        
        public static bool IsKeyword(string target)
        {
        	return Keywords.Contains<string>(target);
        }
    } 
}