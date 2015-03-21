using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArgumentsLibrary.Builders;

namespace ArgumentsLibrary {
    public static class Arguments {
        private static readonly List < Option > _options =
            new List < Option > ();

        private static readonly Dictionary < Type, object > _typeConverters =
            new Dictionary < Type, object > ();

        static Arguments () {
            RegisterTypeConverter ( string.Copy );
            RegisterTypeConverter ( int.Parse );
            RegisterTypeConverter ( float.Parse );
            RegisterTypeConverter ( double.Parse );
            RegisterTypeConverter ( bool.Parse );
        }

        public static void RegisterTypeConverter < T > (
            Func < string, T > converterFunc ) {
            _typeConverters.Add ( typeof ( T ), converterFunc );
        }

        public static OptionBuilder AddOption ( string alias ) {
            return new OptionBuilder ( alias, false );
        }

        public static OptionBuilder AddRequiredOption ( string alias ) {
            return new OptionBuilder ( alias, true );
        }

        public static void Parse ( string [] args ) {
            // TODO implement
        }

        internal static T Convert < T > ( string value ) {
            // TODO
            if ( !_typeConverters.ContainsKey ( typeof ( T ) ) ) {}
            T ret;
            return ret;
        }
    }
}