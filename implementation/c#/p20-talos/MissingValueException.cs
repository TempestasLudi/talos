using System;

namespace p20_talos
{
    public class MissingValueException : Exception
    {
        public MissingValueException(string message) : base(message) {}
    }
}