using System;

namespace ArgumentsLibrary.Exceptions
{
    /// <summary>
    /// Exception is thrown via its descendants
    /// </summary>
    public abstract class ArgumentsException : Exception
    {
        internal ArgumentsException(string message, object args = null)
            : base(args == null ? message : String.Format(message, args))
        {
            
        }
    }
}
