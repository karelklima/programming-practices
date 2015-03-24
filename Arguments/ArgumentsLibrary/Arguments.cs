using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ArgumentsLibrary.Builders;

namespace ArgumentsLibrary
{
    public static class Arguments
    {

        private const string SHORT_OPTION_PREFIX = "-";
        private const string SHORT_OPTION_REGEX = "^-([a-zA-Z][a-zA-Z-_]*)$";
        private const string SHORT_OPTION_ALIAS_REGEX = "^([a-zA-Z])|-([a-zA-Z][a-zA-Z-_]*)$";
        
        private const string LONG_OPTION_PREFIX = "--";
        private const string LONG_OPTION_VALUE_SEPARATOR = "=";
        private const string LONG_OPTION_REGEX = "^--([a-zA-Z][a-zA-Z-_]*)(=.+)?$";
        private const string LONG_OPTION_ALIAS_REGEX = "^([a-zA-Z][a-zA-Z-_]+)|--([a-zA-Z][a-zA-Z-_]*)$";

        private const string OPTION_ALIAS_SEPARATOR = "|";
        private const string PLAIN_ARGUMENTS_SEPARATOR = "--";

        private static Regex _shortOptionRegex = new Regex(SHORT_OPTION_REGEX);
        private static Regex _shortOptionAliasRegex = new Regex(SHORT_OPTION_ALIAS_REGEX);

        private static Regex _longOptionRegex = new Regex(LONG_OPTION_REGEX);
        private static Regex _longOptionAliasRegex = new Regex(LONG_OPTION_ALIAS_REGEX);


        private static List<Option> Options { get; set; }
        private static Dictionary<Type, object> TypeConverters { get; set; }

        static Arguments()
        {
            Options = new List<Option>();
            TypeConverters = new Dictionary<Type, object>();
            PerformDefaultSetup();
        }

        #region API

        public static void RegisterTypeConverter<T>(Func<string, T> converterFunc)
        {
            TypeConverters.Add(typeof (T), converterFunc);
        }

        public static OptionBuilder AddOption(string aliases)
        {
            return new OptionBuilder().WithAlias(aliases);
        }

        public static OptionBuilder AddMandatoryOption(string aliases)
        {
            return AddOption(aliases).SetMandatory(true);
        }

        public static void Parse(string[] args)
        {
            // TODO implement
        }

        public static bool IsOptionSet(string alias)
        {
            // TODO implement
            return true;
        }

        public static T GetOptionValue<T>(string alias)
        {
            // TODO implement
            return Options.First().GetValue<T>();
        }

        public static string GetOptionValue(string alias)
        {
            return GetOptionValue<string>(alias);
        }

        public static void Reset()
        {
            // TODO implement
        }

        #endregion

        private static IEnumerable<string> ParseAliases(string aliases)
        {
            if (aliases == null)
                throw new ArgumentNullException("aliases");
            return aliases.Split(OPTION_ALIAS_SEPARATOR.ToCharArray());
        }

        private static void PerformDefaultSetup()
        {
            RegisterTypeConverter(string.Copy);
            RegisterTypeConverter(int.Parse);
            RegisterTypeConverter(float.Parse);
            RegisterTypeConverter(double.Parse);
            RegisterTypeConverter(bool.Parse);
        }

        internal static void RegisterOptionAlias(Option option, string alias)
        {

        }

        internal static T Convert<T>(string value)
        {
            // TODO
            if (!TypeConverters.ContainsKey(typeof (T)))
            {
                // TODO throw an exception
            }
            return ((Func<string, T>) TypeConverters[typeof(T)]).Invoke(value);
        }
    }
}