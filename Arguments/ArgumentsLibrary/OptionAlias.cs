using System;

namespace ArgumentsLibrary
{
    internal class OptionAlias : IEquatable<OptionAlias>
    {

        internal string Alias { get; private set; }

        internal OptionType Type { get; private set; }

        internal OptionAlias(string alias, OptionType type)
        {
            Alias = alias;
            Type = type;
        }

        #region IEquatable implementation

        public bool Equals (OptionAlias other){
            return other.Alias == Alias
                && other.Type == Type;
        }

        #endregion

        public override int GetHashCode (){
            return this.Alias.GetHashCode (); //If hash is equal (Dictionary.ContainsKey), than Equals() is used
        }

        public override bool Equals(object obj)
        {
            return obj.GetType() == typeof (OptionAlias)
                   && ((OptionAlias) obj).Alias == Alias
                   && ((OptionAlias) obj).Type == Type;
        }

        public override string ToString (){
            if (Type == OptionType.Short) {
                return "-" + Alias;
            } else {
                return "--" + Alias;
            }
        }
    }
}