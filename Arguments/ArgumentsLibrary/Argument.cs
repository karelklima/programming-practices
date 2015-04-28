using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ArgumentsLibrary
{
    internal class Argument<T>
    {
        internal const int DEFAULT_MINIMUM_COUNT = 1;
        internal const int DEFAULT_MAXIMUM_COUNT = 1;
        internal const string DEFAULT_NAME = "argument";
        internal const bool DEFAULT_OPTIONAL = false;

        internal string Name { get; set; }

        internal bool Optional { get; set; }

        internal T DefaultValue { get; set; }

        internal List<Action<T>> Actions { get; set; } 

        internal List<Func<T, bool>> Conditions { get; set; }

        internal T Value { get; set; }

        internal Argument()
        {
            Name = DEFAULT_NAME;
            Optional = DEFAULT_OPTIONAL;
            Actions = new List<Action<T>>();
            Conditions = new List<Func<T, bool>>();
        }

    }

}