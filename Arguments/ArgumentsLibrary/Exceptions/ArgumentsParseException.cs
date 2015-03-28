using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentsLibrary.Exceptions
{
    class ArgumentsParseException : ArgumentsException
    {
        internal ArgumentsParseException(string message, object args)
            : base(message, args)
        {
        }
    }
}
