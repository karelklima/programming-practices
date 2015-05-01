using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ArgumentsLibrary {

    // TODO comment whole class
    internal class Argument<T> {

        internal const int DEFAULT_MINIMUM_COUNT = 1;
        internal const int DEFAULT_MAXIMUM_COUNT = 1;
        internal const string DEFAULT_NAME = "argument";
        internal const bool DEFAULT_OPTIONAL = false;

        internal string Name { get; set; }

        internal bool Optional { get; set; }

        internal T DefaultValue { get; set; }

        internal Type GetValueType() {
            return typeof (T);
        }

        internal List<Action<T>> Actions { get; set; }

        internal void InvokeActions() {
            Actions.ForEach(action => action.Invoke(Value));
        }

        internal List<Func<T, bool>> Conditions { get; set; }

        internal bool AssertConditions() {
            foreach (var condition in Conditions) {
                if (!condition.Invoke(Value)) {
                    return false;
                }
            }
            return true;
        }

        internal T Value { get; set; }

        internal T Parse(string arg, Converter converter) {
            return converter.Convert<T>(arg);
        }

        internal Argument() {
            Name = DEFAULT_NAME;
            DefaultValue = default(T);
            Optional = DEFAULT_OPTIONAL;
            Actions = new List<Action<T>>();
            Conditions = new List<Func<T, bool>>();
        }

    }

}