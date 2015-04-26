using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ArgumentsLibrary
{
    internal class Argument<T>
    {
        internal const uint DEFAULT_MINIMUM_COUNT = 1;
        internal const uint DEFAULT_MAXIMUM_COUNT = 1;

        public List<T> DefaultValues { get; set; }

        public List<T> Values { get; set; }

        internal uint MinimumCount { get; set; }

        internal uint MaximumCount { get; set; }

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