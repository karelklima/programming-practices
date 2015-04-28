using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ArgumentsLibrary
{
    internal class Argument<T>
    {
        internal const int DEFAULT_MINIMUM_COUNT = 1;
        internal const int DEFAULT_MAXIMUM_COUNT = 1;

        public List<T> DefaultValues { get; set; }

        public List<T> Values { get; set; }

        internal int MinimumCount { get; set; }

        internal int MaximumCount { get; set; }

        internal List<Action<T>> Actions { get; set; } 

        internal List<Func<T, bool>> Conditions { get; set; } 

        internal Argument()
        {
            MinimumCount = DEFAULT_MINIMUM_COUNT;
            MaximumCount = DEFAULT_MAXIMUM_COUNT;
            Actions = new List<Action<T>>();
            Conditions = new List<Func<T, bool>>();
        }

    }

}