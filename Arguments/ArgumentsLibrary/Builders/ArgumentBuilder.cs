using System;
using System.Linq;

namespace ArgumentsLibrary.Builders
{
    /// <summary>
    /// Fluent interface provider for building Arguments
    /// </summary>
    /// <typeparam name="T">Type of the Argument</typeparam>
    public class ArgumentBuilder<T>
    {
        #region Internals

        internal Argument<T> Argument { get; private set; }

        internal Arguments Arguments { get; private set; }

        internal Option Option { get; private set; }

        internal ArgumentBuilder(Arguments arguments, Option option)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");
            if (option == null)
                throw new ArgumentNullException("option");

            Argument = new Argument<T>();
            Arguments = arguments;
            Option = option;
            Option.Argument = Argument;
        }

        #endregion

        #region API

        /// <summary>
        /// Sets an indicator whether the Argument is optional or not.
        /// </summary>
        /// <param name="flag">True if optional, False otherwise</param>
        /// <returns>ArgumentBuilder{T} fluent interface</returns>
        public ArgumentBuilder<T> SetOptional(bool flag)
        {
            Argument.MinimumCount = flag ? 0 : 1;
            return this;
        }

        /// <summary>
        /// Specifies the default value for the Argument
        /// </summary>
        /// <param name="value">Default value of type T</param>
        /// <returns>ArgumentBuilder{T} fluent interface</returns>
        public ArgumentBuilder<T> WithDefaultValue(T value)
        {
            // TODO implement
            return this;
        }

        /// <summary>
        /// Specifies a condition that the Argument value must satisfy.
        /// Multiple conditions may be defined; if so, the Argument value
        /// must satisfy all conditions.
        /// </summary>
        /// <param name="conditionFunc">Condition function (predicate)</param>
        /// <returns>ArgumentBuilder{T} fluent interface</returns>
        public ArgumentBuilder<T> WithCondition(Func<T, bool> conditionFunc)
        {
            Argument.Conditions.Add(conditionFunc);
            return this;
        }

        /// <summary>
        /// Specifies a list of possible values of the Argument
        /// Only one set of values should be defined. If there are more sets
        /// defined, the Argument value will be accepted only if it is
        /// present in all enumerations.
        /// </summary>
        /// <param name="valuesEnumeration">Enumeration of possible values
        /// </param>
        /// <returns>ArgumentBuilder{T} fluent interface</returns>
        public ArgumentBuilder<T> WithEnumeratedValue(
            params T[] valuesEnumeration)
        {
            return WithCondition(valuesEnumeration.Contains);
        }

        /// <summary>
        /// Specifies an Action to perform when the Argument value is detected
        /// among console input parameters.
        /// </summary>
        /// <param name="action">Action to be performed when the Argument value
        /// is detected among input parameters</param>
        /// <returns>ArgumentBuilder{T} fluent interface</returns>
        public ArgumentBuilder<T> WithAction(Action<T> action)
        {
            Argument.Actions.Add(action);
            return this;
        }

        /// <summary>
        /// Sets minimum count of Option arguments
        /// </summary>
        /// <param name="count">Minimum count of values</param>
        /// <returns>ArgumentBuilder{T} fluent interface</returns>
        public ArgumentBuilder<T> SetMinimumCount(int count)
        {
            Argument.MinimumCount = count;
            return this;
        }

        /// <summary>
        /// Sets maximum count of Option arguments
        /// </summary>
        /// <param name="count">Maximum count of values</param>
        /// <returns>ArgumentBuilder{T} fluent interface</returns>
        public ArgumentBuilder<T> SetMaximumCount(int count)
        {
            Argument.MaximumCount = count;
            return this;
        }

        #endregion
    }
}