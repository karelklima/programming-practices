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

        /// <summary>
        /// Registers types to be used as Option or Plain Arguments, along with
        /// their converter function. The converter function converts input
        /// string to the given type and returns the result.
        /// </summary>
        /// <typeparam name="T">Target converted type</typeparam>
        /// <param name="converterFunc">Function to convert string to type T</param>
        public static void RegisterTypeConverter<T>(Func<string, T> converterFunc)
        {
            TypeConverters.Add(typeof (T), converterFunc);
        }

        /// <summary>
        /// Adds an Option to the current configuration with given aliases.
        /// Aliases is a string comprising of aliases separated by "|".
        /// One lettered alias is implicitly considered a short option,
        /// multiple letters indicate a long option alias. Short and long
        /// indication can be forced by prefixing the aliases with "-"
        /// and "--" respectively.
        /// </summary>
        /// <example>
        /// // The following examples do exactly the same thing
        /// <code>
        /// Arguments.AddOption("v|verbose");
        /// Arguments.AddOption("-v|--verbose");
        /// Arguments.AddOption("v").WithAlias("verbose");
        /// Arguments.AddOption("-v").WithAlias("--verbose");
        /// </code>
        /// The following examples present non-standard usage:
        /// <code>
        /// Arguments.AddOption("v|-verbose");
        /// Arguments.AddOption("--v").WithAlias("--verbose");
        /// </code>
        /// </example>
        /// <param name="aliases">One or more option aliases</param>
        /// <returns>OptionBuilder instance</returns>
        public static OptionBuilder AddOption(string aliases)
        {
            return new OptionBuilder().WithAliases(aliases);
        }

        /// <summary>
        /// Adds a mandatory Option to the current configuration with given
        /// aliases. Same as <see cref="AddOption">AddOption</see>
        /// </summary>
        /// <param name="aliases"></param>
        /// <returns></returns>
        public static OptionBuilder AddMandatoryOption(string aliases)
        {
            return AddOption(aliases).SetMandatory(true);
        }

        /// <summary>
        /// Processes the command line input arguments.
        /// </summary>
        /// <param name="args">Arguments as passed to the Main</param>
        public static void Parse(string[] args)
        {
            // TODO implement
        }

        /// <summary>
        /// Checks whether the user specified an option or not.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns>True if user specified an option or if a default
        /// value is defined</returns>
        public static bool IsOptionSet(string alias)
        {
            // TODO implement
            return true;
        }

        /// <summary>
        /// Gets Option argument converted to the specified type.
        /// </summary>
        /// <typeparam name="T">Return type of the value</typeparam>
        /// <param name="alias">One of the Option aliases</param>
        /// <returns>Typed Option value</returns>
        public static T GetOptionValue<T>(string alias)
        {
            // TODO implement
            return Options.First().GetValue<T>();
        }

        /// <summary>
        /// Gets Option argument as string. Same as
        /// <see cref="GetOptionValue{T}"/>, implicitly typed.
        /// </summary>
        /// <param name="alias">One of the Option aliases</param>
        /// <returns>Option value as string</returns>
        public static string GetOptionValue(string alias)
        {
            return GetOptionValue<string>(alias);
        }

        /// <summary>
        /// Returns a list of all arguments that do not correspond to Options.
        /// </summary>
        /// <typeparam name="T">Type of all arguments</typeparam>
        /// <returns>List of all plain arguments</returns>
        public static IEnumerable<T> GetPlainArguments<T>()
        {
            // TODO implement
            return new List<T>();
        }

        /// <summary>
        /// Implicit alternative to <see cref="GetPlainArguments{T}"/>.
        /// Returns a list of all arguments that do not correspond to Options
        /// as a list of strings.
        /// </summary>
        /// <returns>List of all plain arguments</returns>
        public static IEnumerable<string> GetPlainArguments()
        {
            return GetPlainArguments<string>();
        }

        /// <summary>
        /// Removes all Option definitions and type converters, and resets
        /// Arguments to the default state.
        /// </summary>
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

        internal static void RegisterOptionAliases(Option option, string aliases)
        {
            foreach (var alias in ParseAliases(aliases))
            {
                RegisterOptionAlias(option, alias);
            }
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