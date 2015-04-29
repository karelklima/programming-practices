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

        public override bool Equals(object obj)
        {
            return obj.GetType() == typeof (OptionAlias)
                   && ((OptionAlias) obj).Alias == Alias
                   && ((OptionAlias) obj).Type == Type;
        }
    }
}