using System;

namespace ArgumentsLibrary.Exceptions
{
    /// <summary>
    /// Exception thrown during Arguments setup phase
    /// </summary>
    public class ArgumentsSetupException : ArgumentsException
    {
        internal ArgumentsSetupException(string message, object args = null)
            : base(message, args)
        {
        }
    }
}
