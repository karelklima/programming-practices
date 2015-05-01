using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentsLibrary.Exceptions {

    internal class CommandLineException : ArgumentsException {

        internal CommandLineException(string message, params object[] args)
            : base(message, args) {
        }

    }

}