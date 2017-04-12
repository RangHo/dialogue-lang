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
        	StringBuilder str = new StringBuilder();
        	bool escaped = false;	// True if escaped
        	Input.Read();			// Removes the extra " character at the beginning of token
        	while (Input.Peek() > -1)
        	{
        		char target = (char)Input.Read();
        		
        		if (escaped)
        			switch (target)
        			{
        			case 'n':
        				str.Append('\n');
        				escaped = false;
        				break;
        			case 't':
        				str.Append('\t');
        				escaped = false;
        				break;
        			case '\\':
        				str.Append('\\');
        				escaped = false;
        				break;
        			default:
        				throw new InvalidCharacterException(string.Format("Invalid escape character was supplied: {0}", target));
        			}
        		else if (target == '\\')
        			escaped = true;
        		else if (target == '"')
        			break;
        		else
        			str.Append(target);
        	}
            
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
        
        public Token ReadLineBreak()
        {
        	Input.Read();
        	return new Token(Token.Classification.LineBreak, "Line Break");
        }
    }
	
    public class Token
    {
    	[Flags]
    	public enum Classification
    	{
    		Punctuation	= 1,
    		Number		= 2,
    		String		= 4,
    		Keyword		= 8,
    		Identifier	= 16,
    		LineBreak	= 32
    	}
    	
    	public Token(Classification type, string content)
    	{
    		this.TokenType = type;
    		this.Content = content;
    	}
    	
    	public Classification TokenType;
    	
    	public string Content;
    }
    
    public static class Predicates
    {
		public static readonly string[] Keywords = {"set", "of", "null", "true", "false", "done", "label", "choice"};
        
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