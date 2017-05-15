using System;

namespace RangHo.DialogueScript
{
    public class InvalidCharacterException : Exception
    {
    	public InvalidCharacterException() { }
        
    	public InvalidCharacterException(string message) : base(message) { }
        
    	public InvalidCharacterException(string message, Exception innerException) : base(message, innerException) { }
    }
    
    public class UnexpectedTokenException : Exception
    {
    	public UnexpectedTokenException() { }
    	
    	public UnexpectedTokenException(string message) : base(message) { }
    	
    	public UnexpectedTokenException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class UnexpectedStatementException : Exception
    {
        public UnexpectedStatementException() { }

        public UnexpectedStatementException(string message) : base(message) { }

        public UnexpectedStatementException(string message, Exception innerException) : base(message, innerException) { }
    }
    
    public class InvalidStatementPassedException : Exception
    {
    	public InvalidStatementPassedException() { }
    	
    	public InvalidStatementPassedException(string message) : base(message) { }
    	
    	public InvalidStatementPassedException(string message, Exception innerException) : base(message, innerException) { }
    }
    
    public class PropertyNotExistException : Exception
    {
    	public PropertyNotExistException() { }
    	
    	public PropertyNotExistException(string message) : base(message) { }
    	
    	public PropertyNotExistException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class LabelNotFoundException : Exception
    {
        public LabelNotFoundException() { }

        public LabelNotFoundException(string message) : base(message) { }

        public LabelNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}