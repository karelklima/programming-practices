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

        internal ArgumentBuilder()
        {
            Argument = new Argument<T>();
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
            // TODO implement
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
            // TODO implement
            return this;
        }

        /// <summary>
        /// Specifies a list of possible values of the Argument
        /// Only one set of values should be defined. If there are more sets
        /// defined, the Argument value will be accepted only if it is
        /// present in all enumerations.
        /// </summary>
        /// <param name="valuesEnumeration">Enumeration of possible values</param>
        /// <returns>ArgumentBuilder{T} fluent interface</returns>
        public ArgumentBuilder<T> WithEnumeratedValue(params T[] valuesEnumeration)
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
            // TODO implement
            return this;
        }

        /// <summary>
        /// Sets minimal count of required values
        /// </summary>
        /// <param name="count">Count of required values</param>
        /// <returns>ArgumentBuilder{T} fluent interface</returns>
        public ArgumentBuilder<T> SetMinimalCount(uint count)
        {
            // TODO implement
            return this;
        }

        /// <summary>
        /// Sets maximum acceptable count of required values
        /// </summary>
        /// <param name="count">Count of values</param>
        /// <returns>ArgumentBuilder{T} fluent interface</returns>
        public ArgumentBuilder<T> SetMaximalCount(uint count)
        {
            // TODO implement
            return this;
        }

        #endregion
    }
}