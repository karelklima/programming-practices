using System;

namespace ArgumentsLibrary {

    internal class OptionAlias : IEquatable<OptionAlias> {

        internal string Alias { get; private set; }

        internal OptionType Type { get; private set; }

        internal OptionAlias(string alias, OptionType type) {
            if (alias == null) {
                throw new ArgumentNullException("alias");
            }
            if (alias.Equals(string.Empty)) {
                throw new ArgumentException(
                    "Argument alias cannot be an empty string");
            }
            Alias = alias;
            Type = type;
        }

        public bool Equals(OptionAlias other) {
            return other.Alias == Alias && other.Type == Type;
        }

        public override int GetHashCode() {
            // If hash is equal (Dictionary.ContainsKey), than Equals() is used
            return Alias.GetHashCode();
        }

        public override string ToString() {
            // strings to constants
            if (Type == OptionType.Short) {
                return "-" + Alias;
            }
            else {
                return "--" + Alias;
            }
        }

    }

}