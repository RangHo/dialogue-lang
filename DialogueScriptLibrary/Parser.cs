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
            this.Input = new GenericStream<Token>(tokens);
        }

        public GenericStream<Token> Input;
        
        public delegate AST Parse();

        public AST ParseNextToken()
        {
            if (Input.IsEnd())
                return null;

            AST ParsedAST = null;

            // Keyword Parsing
            if (IsKeyword("set"))
                ParsedAST = ParseSet();
            else if (IsKeyword("true") || IsKeyword("false"))
                ParsedAST = ParseBoolean();
            else if (IsKeyword("choice"))
                ParsedAST = ParseChoice();
            else if (IsKeyword("label"))
                ParsedAST = ParseLabel();
            else if (IsKeyword("return") || IsKeyword("done"))
                ParsedAST = ParseReturn();
            else if (IsKeyword("Jump"))
                ParsedAST = ParseJump();

            // Literals and Identifiers
            else if (Input.Peek().TokenType == Token.Classification.Number)
                ParsedAST = ParseNumber();
            else if (Input.Peek().TokenType == Token.Classification.String)
                ParsedAST = ParseString();
            else if (Input.Peek().TokenType == Token.Classification.Identifier)
                ParsedAST = Maybe(ParseSay, ParseOf);
            else if (Input.Peek().TokenType == Token.Classification.LineBreak)
                ParsedAST = ParseLineBreak();

            // Highly unlikely, but just in case...
            else
                throw new UnexpectedTokenException($"The type of provided token is unknown ({Input.Peek().TokenType}). What have you done.");

            return ParsedAST;
        }
        


        // Parse statements

        public AST ParseSay()
        {
            AST result = null;
            if (!Input.IsEnd(3))
                return result;

            Token Speaker = Input.Read(), Colon = Input.Read(), Conversation = Input.Read();
            if (Colon.TokenType == Token.Classification.Punctuation && Colon.Content == ":" &&
                Conversation.TokenType == Token.Classification.String)
                result = AST.CreateSayAST(AST.CreateIdentifierAST(Speaker.Content), AST.CreateStringAST(Conversation.Content));
            return result;
        }
        
        public AST ParseSet()
        {
            AST result = null, target = null;

            Input.Read();   // Discard the set keyword
            if (!Input.IsEnd() &&
                Input.Peek().TokenType == Token.Classification.Identifier)    // Make sure the next token is an identifier
                target = Maybe(ParseOf, ParseIdentifier);        // Statement can be followed by Of statement or Identifier statement.

            if (Input.Peek().TokenType == Token.Classification.Number ||
                Input.Peek().TokenType == Token.Classification.String ||
                Input.Peek().TokenType == Token.Classification.Identifier)
                result = AST.CreateSetAST(target, Maybe(ParseNumber, ParseString, ParseIdentifier));
            return result;
        }
        
        public AST ParseOf()
        {
            AST result = null, target = null, property = null;
            if (Input.IsEnd(3))
                return result;

            Token TargetIdentifier = Input.Read(), OfKeyword = Input.Read(), ContainerIdentifier = Input.Read();
            if (OfKeyword.TokenType == Token.Classification.Keyword && OfKeyword.Content == "of" &&
                ContainerIdentifier.TokenType == Token.Classification.Identifier)
            {
                target = AST.CreateIdentifierAST(ContainerIdentifier.Content);
                property = AST.CreateIdentifierAST(TargetIdentifier.Content);
                result = AST.CreateOfAST(target, property);
            }
            
            return result;
        }
        
        public AST ParseChoice()
        {
            AST result = null;
            if (Input.IsEnd(4))
                return result;

            Input.Read();   // Discard the choice keyword

            Token DestinationIdentifier = Input.Read(), Colon = Input.Read(), DescriptionString = Input.Read();
            if (DestinationIdentifier.TokenType == Token.Classification.Identifier &&
                Colon.TokenType == Token.Classification.Punctuation && Colon.Content == ":" &&
                DescriptionString.TokenType == Token.Classification.String)
                result = AST.CreateChoiceAST(
                             AST.CreateIdentifierAST(DestinationIdentifier.Content),
                             AST.CreateStringAST(DescriptionString.Content));

            return result;
        }
        
        public AST ParseLabel()
        {
            Input.Read();   // Discard the label keyword
            return !Input.IsEnd() ? AST.CreateLabelAST(AST.CreateIdentifierAST(Input.Read().Content)) : null;
        }

        public AST ParseJump()
        {
            Input.Read();   // Discard the jump keyword
            return !Input.IsEnd() ? AST.CreateJumpAST(AST.CreateIdentifierAST(Input.Read().Content)) : null;
        }

        public AST ParseReturn()
        {
            Input.Read();   // Discard the return keyword
            return AST.CreateReturnAST();
        }



        // Parse literals

        public AST ParseLineBreak()
        {
            Input.Read();   // Discard the line break character
            return AST.CreateLineBreakAST();
        }
        
        public AST ParseBoolean()
        {
            return (Input.Read().Content == "true") ? AST.CreateBooleanAST(true) : AST.CreateBooleanAST(false);
        }
        
        public AST ParseNumber()
        {
            return (int.TryParse(Input.Read().Content, out int result)) ? AST.CreateNumberAST(result) : null;
        }
        
        public AST ParseString()
        {
            return (Input.Peek().TokenType == Token.Classification.String) ? AST.CreateStringAST(Input.Read().Content) : null;
        }
        
        public AST ParseIdentifier()
        {
            return !Input.IsEnd() ? AST.CreateIdentifierAST(Input.Read().Content) : null;
        }
        


        // Tool methods to make my life easier

        private bool IsKeyword(string keyword)
        {
            if (!Input.IsEnd() &&
                Input.Peek().TokenType == Token.Classification.Keyword &&
                Input.Peek().Content.ToLower() == keyword.ToLower())
                return true;
            return false;
        }

        private AST Maybe(params Parse[] parsers)
        {
            AST result = null;
            int BackedUpPosition = Input.BackupPosition();
            foreach (Parse parser in parsers)
            {
                result = parser();
                if (result == null)
                {
                    Input.RecoverPosition(BackedUpPosition);
                    continue;
                }
                else
                    return result;
            }
            throw new UnexpectedTokenException("Unknown error happened. Please check the input array.");
        }
    }
}
