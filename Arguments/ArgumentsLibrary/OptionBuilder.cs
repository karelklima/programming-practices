using System;

namespace ArgumentsLibrary
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

        public OptionBuilder WithRequiredArgument(string name)
        {
            return this;
        }

        public OptionBuilder WithOptionalArgument(string name)
        {
            return this;
        }

        public OptionBuilder WithArgumentType(ArgumentType type)
        {
            return this;
        }

        public OptionBuilder WithArgumentTest(Func<object, bool> test)
        {
            return this;
        }
    }
}