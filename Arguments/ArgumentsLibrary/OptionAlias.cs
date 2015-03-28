namespace ArgumentsLibrary
{
    internal class OptionAlias
    {

        internal string Alias { get; private set; }

        internal OptionType Type { get; private set; }

        internal OptionAlias(string alias, OptionType type)
        {
            Alias = alias;
            Type = type;
        }
    }
}