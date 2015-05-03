using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ArgumentsLibrary {

    // TODO comment whole class
    internal class Argument<T> {

        internal const string DEFAULT_NAME = "argument";
        internal const bool DEFAULT_OPTIONAL = false;

        internal string Name { get; set; }

        internal bool Optional { get; set; }

        internal T DefaultValue { get; set; }

        internal bool DefaultValueIsSet { get; set; }

        internal List<Action<T>> Actions { get; set; }

        internal List<Func<T, bool>> Conditions { get; set; }

        internal Type Type {
            get { return typeof (T); }
        }

        internal Argument()
        {
            Name = DEFAULT_NAME;
            DefaultValue = default(T);
            Optional = DEFAULT_OPTIONAL;
            Actions = new List<Action<T>>();
            Conditions = new List<Func<T, bool>>();
        }

        internal T Parse(string value, Converter converter) {
            return converter.Convert<T>(value);
        }

        internal void InvokeActions(T value)
        {
            Actions.ForEach(action => action.Invoke(value));
        }

        internal void AssertConditions(T value) {
            if (!Conditions.All(condition => condition.Invoke(value))) {
                throw new ArgumentOutOfRangeException("value",
                    "Argument does not satisfy required conditions");
            }
        }

    }

}