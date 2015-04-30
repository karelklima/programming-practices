using System;

namespace ArgumentsLibrary.Exceptions
{
    /// <summary>
    /// Exception thrown during Arguments setup phase
    /// </summary>
    public class SetupException : ArgumentsException
    {
        internal SetupException(string message, object args = null)
            : base(message, args)
        {
        }
    }
}
