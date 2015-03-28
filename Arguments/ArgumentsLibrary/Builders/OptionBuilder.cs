using System;

namespace ArgumentsLibrary.Builders
{
    /// <summary>
    /// Fluent interface provider for building Options
    /// </summary>
    public class OptionBuilder
    {
        #region Internals

        internal Option Option { get; private set; }

        internal OptionBuilder()
        {
            Option = new Option();
        }

        #endregion

        #region API

        /// <summary>
        /// Sets an indicator whether the Option is mandatory or not.
        /// </summary>
        /// <param name="flag">True if mandatory, False otherwise</param>
        /// <returns></returns>
        public OptionBuilder SetMandatory(bool flag)
        {
            // TODO implement
            return this;
        }

        /// <summary>
        /// Adds aliases to current Option, same as <see cref="WithAlias"/>
        /// </summary>
        /// <param name="aliases">One or more aliases</param>
        /// <returns>OptionBuilder fluent interface</returns>
        public OptionBuilder WithAliases(string aliases)
        {
            Arguments.RegisterOptionAliases(Option, aliases);
            return this;
        }

        /// <summary>
        /// Adds aliases to current Option, same as <see cref="WithAliases"/>
        /// </summary>
        /// <param name="alias">One or more aliases</param>
        /// <returns>OptionBuilder fluent interface</returns>
        public OptionBuilder WithAlias(string alias)
        {
            return WithAliases(alias);
        }

        /// <summary>
        /// Description of the Option to be used when showing help
        /// </summary>
        /// <param name="description"></param>
        /// <returns>OptionBuilder fluent interface</returns>
        public OptionBuilder WithDescription(string description)
        {
            // TODO implement
            return this;
        }

        /// <summary>
        /// Adds an Action to be called when the Option is detected
        /// among console input arguments. Multiple actions can be specified.
        /// </summary>
        /// <param name="delegateAction">Action to be performed when
        /// the Option is detected among input arguments</param>
        /// <returns>OptionBuilder fluent interface</returns>
        public OptionBuilder WithAction(Action delegateAction)
        {
            // TODO implement
            return this;
        }

        /// <summary>
        /// Defines the Option Argument with specific type and name.
        /// </summary>
        /// <typeparam name="T">Type of the Option Argument</typeparam>
        /// <param name="name">Name of the argument to be used in help</param>
        /// <returns>ArgumentBuilder{T} fluent interface</returns>
        public ArgumentBuilder<T> WithArgument<T>(string name)
        {
            // TODO
            var builder = new ArgumentBuilder<T>();
            return builder;
        }

        /// <summary>
        /// Same as <see cref="WithArgument{T}"/>, implicitly typed as string.
        /// </summary>
        /// <param name="name">Name of the argument to be used in help</param>
        /// <returns>ArgumentBuilder{string} fluent interface</returns>
        public ArgumentBuilder<string> WithArgument(string name)
        {
            return WithArgument<string>(name);
        }

        /// <summary>
        /// Defines an optional Argument
        /// Same as <see cref="WithArgument{T}"/>, 
        /// </summary>
        /// <typeparam name="T">Type of the Option Argument</typeparam>
        /// <param name="name">Name of the argument to be used in help</param>
        /// <returns>ArgumentBuilder{T} fluent interface</returns>
        public ArgumentBuilder<T> WithOptionalArgument<T>(string name)
        {
            return WithArgument<T>(name).SetOptional(true);
        }

        /// <summary>
        /// Same as <see cref="WithOptionalArgument{T}"/>, implicitly typed as string
        /// </summary>
        /// <param name="name">Name of the argument to be used in help</param>
        /// <returns>ArgumentBuilder{string} fluent interface</returns>
        public ArgumentBuilder<string> WithOptionalArgument(string name)
        {
            return WithOptionalArgument<string>(name);
        }

        #endregion

    }
}