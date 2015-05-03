using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ArgumentsLibrary {

    /// <summary>
    /// Option argument representation.
    /// It is internal class, to use it, see <see cref="ArgumentBuilder<T>" />. 
    /// <example>
    /// <code>
    /// var arguments = new Arguments();
    /// arguments.AddOption("s|size")
    ///     .WithDescription("Size option with default value of 42")
    ///     // OptionBuilder.WithArgument<T> returns ArgumentBuilder<T> instance
    ///     .WithArgument<int>("SIZE")
    ///     .WithDefaultValue(42)
    ///     .WithCondition(v => v > 0);
    /// </code>
    /// </example>
    /// </summary>
    internal class Argument<T> {
        
        /// <summary>
        /// Definition of default name for argument
        /// </summary>
        internal const string DEFAULT_NAME = "argument";

        /// <summary>
        /// Definition of default behaviour: Optional or Mandatory
        /// </summary>
        internal const bool DEFAULT_OPTIONAL = false;

        /// <summary>
        /// Name of argument
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// Is argument optinal?
        /// </summary>
        internal bool Optional { get; set; }

        /// <summary>
        /// Default value of argument. 
        /// It is ignored, if argument is not optional.
        /// Constructor = default(T)
        /// </summary>
        internal T DefaultValue { get; set; }

        /// <summary>
        /// List of actions, which are invoked when argument value appears in args string
        /// It will not be invoked, when default value is used.
        /// It will be invoked after value is parsed and tesed with defined conditions.
        /// Action signature: void function(T value){ ... }
        /// </summary>
        internal List<Action<T>> Actions { get; set; }

        /// <summary>
        /// List of conditions, which are checked when argument value appears in args string
        /// Default value will not be checked.
        /// Condition signature: bool function(T value){ ... }
        /// </summary>
        internal List<Func<T, bool>> Conditions { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        internal Argument()
        {
            Name = DEFAULT_NAME;
            Optional = DEFAULT_OPTIONAL;
            DefaultValue = default(T);
            Actions = new List<Action<T>>();
            Conditions = new List<Func<T, bool>>();
        }

        /// <summary>
        /// Parse the argument value from string with converters.
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="converter">Type converters</param>
        internal T Parse(string value, Converter converter) {
            return converter.Convert<T>(value);
        }

        /// <summary>
        /// Invokes the actions.
        /// </summary>
        /// <param name="value">Argument value</param>
        internal void InvokeActions(T value)
        {
            Actions.ForEach(action => action.Invoke(value));
        }

        /// <summary>
        /// Asserts the conditions.
        /// </summary>
        /// <param name="value">Argument value</param>
        internal void AssertConditions(T value) {
            if (!Conditions.All(condition => condition.Invoke(value))) {
                throw new ArgumentOutOfRangeException("value",
                    "Argument does not satisfy required conditions");
            }
        }

    }

}