using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentsLibrary.Exceptions {

    /// <summary>
    /// Exception is thrown when the library user performs invalid operations
    /// with CommandLine class
    /// </summary>
    internal class CommandLineException : ArgumentsException {

        internal CommandLineException(string message, params object[] args)
            : base(message, args) {
        }

    }

}