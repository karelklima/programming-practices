using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArgumentsLibrary.Builders;

namespace ArgumentsLibrary
{
    public sealed class Arguments
    {
        private static readonly List<Option> _options = new List<Option>();
        private static Dictionary<Type, object> _typeConverters = new Dictionary<Type, object>();

        static Arguments()
        {
            RegisterTypeConverter(string.Copy);
            RegisterTypeConverter(int.Parse);
            RegisterTypeConverter(bool.Parse);
        }

        public static void RegisterTypeConverter<T>(Func<string, T> converterFunc)
        {
            _typeConverters.Add(typeof(T), converterFunc);
        }

        public static OptionBuilder AddOption(string alias)
        {
            var option = new Option(alias);
            _options.Add(option);
            return new OptionBuilder(option);
        }

        public static void Parse(string[] args)
        {
            // TODO implement
        }
    }
}
