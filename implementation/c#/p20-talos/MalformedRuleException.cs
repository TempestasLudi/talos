using System;

namespace p20_talos
{
    public class MalformedRuleException : Exception
    {
        public MalformedRuleException(string message) : base(message) {}
    }
}