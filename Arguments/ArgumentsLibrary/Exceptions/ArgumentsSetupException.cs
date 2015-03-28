using System;

namespace ArgumentsLibrary.Exceptions
{
    class ArgumentsSetupException : ArgumentsException
    {
        internal ArgumentsSetupException(string message, object args)
            : base(message, args)
        {
        }
    }
}
