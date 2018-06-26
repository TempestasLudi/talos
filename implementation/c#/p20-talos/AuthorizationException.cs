using System;

namespace p20_talos
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException(string message) : base(message) {}
    }
}