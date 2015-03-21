using System;

namespace ArgumentsLibrary.Builders {
    public class ArgumentBuilder< T > {
        internal ArgumentBuilder( Option option, bool required ) {}

        public ArgumentBuilder< T > WithPredicate( Func< T, bool > predicateFunc ) {
            return this;
        }

        public ArgumentBuilder< T > WithEnumeratedValue( params T [] valuesList ) {
            return this;
        }

        public ArgumentBuilder< T > WithAction( Action< T > action ) {
            return this;
        }
    }
}