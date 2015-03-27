using System;

namespace ArgumentsLibrary.Builders
{
    public class ArgumentBuilder<T>
    {

        internal Argument<T> Argument { get; private set; }

        internal ArgumentBuilder()
        {
            Argument = new Argument<T>();
        }

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
        /// Specifies a condition that the Argument value must satisfy
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
        /// </summary>
        /// <param name="valuesList">Enumeration of possible values</param>
        /// <returns>ArgumentBuilder{T} fluent interface</returns>
        public ArgumentBuilder<T> WithEnumeratedValue(params T[] valuesList)
        {
            // TODO implement
            return this;
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

        #endregion
    }
}