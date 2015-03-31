using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentsLibrary.Exceptions
{
    /// <summary>
    /// Exception thrown during Arguments parse phase
    /// </summary>
    public class ArgumentsParseException : ArgumentsException
    {
        internal ArgumentsParseException(string message, object args = null)
            : base(message, args)
        {
        }
    }
}
