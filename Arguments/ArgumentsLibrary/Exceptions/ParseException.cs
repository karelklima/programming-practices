﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentsLibrary.Exceptions {

    /// <summary>
    /// Exception thrown during Arguments parse phase when invalid command line
    /// input is detected
    /// </summary>
    public class ParseException : ArgumentsException {

        internal ParseException(string message, params object[] args)
            : base(message, args) {
        }

    }

}