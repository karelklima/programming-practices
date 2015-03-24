using System;

namespace ArgumentsLibrary.Builders
{
    public class OptionBuilder
    {
        internal Option Option { get; private set; }

        internal OptionBuilder()
        {
            Option = new Option();
        }

        #region API

        public OptionBuilder SetMandatory(bool flag)
        {
            // TODO implement
            return this;
        }

        public OptionBuilder WithAlias(string alias)
        {
            Arguments.RegisterOptionAlias(Option, alias);
            return this;
        }

        public OptionBuilder WithDescription(string help)
        {
            // TODO implement
            return this;
        }

        public OptionBuilder WithAction(Action delegateAction)
        {
            // TODO implement
            return this;
        }

        public ArgumentBuilder<T> WithArgument<T>(string name)
        {
            var builder = new ArgumentBuilder<T>();
            return builder;
        }

        public ArgumentBuilder<string> WithArgument(string name)
        {
            return WithArgument<string>(name);
        }

        public ArgumentBuilder<T> WithOptionalArgument<T>(string name)
        {
            return WithArgument<T>(name).SetOptional(true);
        }

        public ArgumentBuilder<string> WithOptionalArgument(string name)
        {
            return WithOptionalArgument<string>(name);
        }

        #endregion
    }
}