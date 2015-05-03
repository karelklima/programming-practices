using System;

namespace ArgumentsLibrary {

    internal class OptionAlias : IEquatable<OptionAlias> {

        /// <summary>
        /// Definition of a prefix for a short option, i.e. -o
        /// </summary>
        private const string SHORT_OPTION_PREFIX = "-";

        /// <summary>
        /// Definition of a prefix for a long option, i.e. --option
        /// </summary>
        private const string LONG_OPTION_PREFIX = "--";

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

        public override bool Equals(object obj) {
            return obj is OptionAlias && Equals((OptionAlias) obj);
        }

        public override int GetHashCode() {
            // If hash is equal (in Dictionary.ContainsKey), than Equals() is used
            return Alias.GetHashCode();
        }

        public override string ToString() {
            if (Type == OptionType.Short) {
                return SHORT_OPTION_PREFIX + Alias;
            }
            else {
                return LONG_OPTION_PREFIX + Alias;
            }
        }

    }

}