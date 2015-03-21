using System;

namespace ArgumentsLibrary.Builders
{
    public class OptionBuilder
    {
        private Option _option;

        internal OptionBuilder(Option option)
        {
            _option = option;
        }

        public OptionBuilder WithAlias(string alias)
        {
            return this;
        }

        public OptionBuilder WithDescription(string help)
        {
            return this;
        }

        public OptionBuilder WithDelegate(Action delegateAction)
        {
            return this;
        }

        public ArgumentBuilder<T> WithArgument<T>(string name)
        {
            return new ArgumentBuilder<T>(_option, true);
        }

        public ArgumentBuilder<string> WithArgument(string name)
        {
            return WithArgument<string>(name);
        }

        public ArgumentBuilder<T> WithOptionalArgument<T>(string name)
        {
            // TODO optional
            return new ArgumentBuilder<T>(_option, false);
        }

        public ArgumentBuilder<string> WithOptionalArgument(string name)
        {
            return WithOptionalArgument<string>(name);
        }

    }
}