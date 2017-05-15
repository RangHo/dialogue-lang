using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RangHo.DialogueScript
{
    public class Token
    {
        [Flags]
        public enum Classification
        {
            Punctuation = 1,
            Number = 2,
            String = 4,
            Keyword = 8,
            Identifier = 16,
            LineBreak = 32
        }

        public Token(Classification type, string content)
        {
            this.TokenType = type;
            this.Content = content;
        }

        public Classification TokenType { get; set; }

        public string Content { get; set; }
    }
}
