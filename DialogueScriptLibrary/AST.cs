using System;

namespace RangHo.DialogueScript
{
    /// <summary>
    /// This represents each AST node.
    /// </summary>
    public class AST
    {
        public AST()
        {
        }
        
        [Flags]
        public enum Classification
        {
            Number        = 1,        // Number values (0, 1, 2, 3)
            String        = 2,        // String values ("Hello, world!")
            Boolean     = 4,        // Boolean values (true, false)
            Identifier    = 8,        // Identifiers (var1, var2)
            Say            = 16,        // Say statements (<character>: "<content>")
            Set            = 32,        // Set statements (set <property> <value>)
            Of            = 64,        // Of statements (<property> of <object>)
            Label        = 128,        // Label statements (label <name>)
            Choice        = 256,        // Choice statements (choice <label>: "<content>")
            Jump        = 512,      // Jump statements (jump <label>)
            Return      = 1024,     // Return statement (return)
            LineBreak   = 2048      // Line Break
        }
        
        public Classification ASTType { get; set; }
        
        public object Target { get; set; }
        
        public object Value { get; set; }
        
        /// <summary>
        /// Creates a number AST.
        /// </summary>
        /// <param name="value">Value of the AST</param>
        /// <returns>A Number AST Object</returns>
        public static AST CreateNumberAST(int value)
        {
            AST result = new AST()
            {
                ASTType = Classification.Number,
                Value = value
            };
            return result;
        }
        
        /// <summary>
        /// Creates a string AST.
        /// </summary>
        /// <param name="value">Value of the AST</param>
        /// <returns>A String AST Object</returns>
        public static AST CreateStringAST(string value)
        {
            AST result = new AST()
            {
                ASTType = Classification.String,
                Value = value
            };
            return result;
        }
        
        /// <summary>
        /// Creates a boolean AST.
        /// </summary>
        /// <param name="value">Value of the AST</param>
        /// <returns>A Boolean AST Object</returns>
        public static AST CreateBooleanAST(bool value)
        {
            AST result = new AST()
            {
                ASTType = Classification.Boolean,
                Value = value
            };
            return result;
        }
        
        /// <summary>
        /// Creates an identifier AST.
        /// </summary>
        /// <param name="name">Name of the identifier</param>
        /// <returns>An Identifier AST</returns>
        public static AST CreateIdentifierAST(string name)
        {
            AST result = new AST()
            {
                ASTType = Classification.Identifier,
                Value = name
            };
            return result;
        }
        
        /// <summary>
        /// Creates a say statement AST.
        /// </summary>
        /// <param name="character">Identifier AST of the character</param>
        /// <param name="content">String AST of the content</param>
        /// <returns>A Say Statement AST</returns>
        public static AST CreateSayAST(AST character, AST content)
        {
            AST result = new AST()
            {
                ASTType = Classification.Say,
                Target = character,
                Value = content
            };
            return result;
        }
        
        /// <summary>
        /// Creates a set statement AST.
        /// </summary>
        /// <param name="target">Identifier or Of Statement AST of the target</param>
        /// <param name="value">Value-type AST (String, Number, Identifer, etc.)</param>
        /// <returns>A Set Statement AST</returns>
        public static AST CreateSetAST(AST target, AST value)
        {
            AST result = new AST()
            {
                ASTType = Classification.Set,
                Target = target,
                Value = value
            };
            return result;
        }
        
        /// <summary>
        /// Creates an of statement AST.
        /// </summary>
        /// <param name="target">Identifier AST of the target object</param>
        /// <param name="property">Identifier AST of the property</param>
        /// <returns>An Of Statement AST</returns>
        public static AST CreateOfAST(AST target, AST property)
        {
            AST result = new AST()
            {
                ASTType = Classification.Of,
                Target = target,
                Value = property
            };
            return result;
        }
        
        /// <summary>
        /// Creates a label statement AST.
        /// </summary>
        /// <param name="name">Name of the AST</param>
        /// <returns>A Label Statement AST</returns>
        public static AST CreateLabelAST(AST name)
        {
            AST result = new AST()
            {
                ASTType = Classification.Label,
                Value = name
            };
            return result;
        }
        
        /// <summary>
        /// Creates a choice statement AST.
        /// </summary>
        /// <param name="destination">Identifer AST of the target</param>
        /// <param name="content">String AST of the content</param>
        /// <returns>A Choice Statement AST</returns>
        public static AST CreateChoiceAST(AST destination, AST content)
        {
            AST result = new AST
            {
                ASTType = Classification.Choice,
                Target = destination,
                Value = content
            };
            return result;
        }

        /// <summary>
        /// Creates a jump statement AST.
        /// </summary>
        /// <param name="destination">Name of label to jump to.</param>
        /// <returns>A Jump Statement AST</returns>
        public static AST CreateJumpAST(AST destination)
        {
            AST result = new AST
            {
                ASTType = Classification.Jump,
                Value = destination
            };
            return result;
        }

        /// <summary>
        /// Creates a return statement AST.
        /// </summary>
        /// <returns>A Return Statement AST</returns>
        public static AST CreateReturnAST()
        {
            AST result = new AST
            {
                ASTType = Classification.Return
            };
            return result;
        }

        /// <summary>
        /// Creates a line break AST.
        /// </summary>
        /// <returns>A line break AST</returns>
        public static AST CreateLineBreakAST()
        {
            AST result = new AST
            {
                ASTType = Classification.LineBreak
            };
            return result;
        }
    }
}
