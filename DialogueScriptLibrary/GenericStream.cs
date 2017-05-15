using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RangHo.DialogueScript
{
    public class GenericStream<T>
    {
        private T[] Input;

        private int Position;

        public GenericStream(T[] input)
        {
            this.Input = input;
            Position = 0;
        }

        public T Peek()
        {
            return Input[Position];
        }

        public T Read()
        {
            return Input[Position++];
        }

        public int BackupPosition()
        {
            return Position;
        }

        public void RecoverPosition(int position)
        {
            this.Position = position;
        }

        public bool IsEnd(int padding = 0)
        {
            return Input.Length <= Position + padding;
        }
    }
}
