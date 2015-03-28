using System;

namespace ArgumentsLibrary.Exceptions
{
    class ArgumentsException : Exception
    {
        internal ArgumentsException(string message, object args)
            : base(String.Format(message, args))
        {
            
        }
    }
}
