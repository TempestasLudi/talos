using System;

namespace p20_talos
{
    public class ParseException : Exception
    {
        public ParseException(string message) : base(message) {}
    }
}