using System;

namespace ArgumentsLibrary.Exceptions
{
    /// <summary>
    /// Exception is thrown via its descendants
    /// </summary>
    public abstract class ArgumentsException : Exception
    {
        internal ArgumentsException(string message, params object[] args)
            : base(args == null ? message : String.Format(message, args))
        {
            
        }
    }
}
