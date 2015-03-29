using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ArgumentsLibrary.Builders;
using ArgumentsLibrary.Exceptions;

namespace ArgumentsLibrary
{
    public class Arguments
    {

        #region Internals

        private const string SHORT_OPTION_PREFIX = "-";
        private const string SHORT_OPTION_REGEX = "^-([a-zA-Z][a-zA-Z-_]*)$";
        private const string SHORT_OPTION_ALIAS_REGEX = "^([a-zA-Z])|-([a-zA-Z][a-zA-Z-_]*)$";
        
        private const string LONG_OPTION_PREFIX = "--";
        private const string LONG_OPTION_VALUE_SEPARATOR = "=";
        private const string LONG_OPTION_REGEX = "^--([a-zA-Z][a-zA-Z-_]*)(=.+)?$";
        private const string LONG_OPTION_ALIAS_REGEX = "^([a-zA-Z][a-zA-Z-_]+)|--([a-zA-Z][a-zA-Z-_]*)$";

        private const string OPTION_ALIAS_SEPARATOR = "|";
        private const string PLAIN_ARGUMENTS_SEPARATOR = "--";

        private static readonly Regex _shortOptionRegex = new Regex(SHORT_OPTION_REGEX);
        private static readonly Regex _shortOptionAliasRegex = new Regex(SHORT_OPTION_ALIAS_REGEX);

        private static readonly Regex _longOptionRegex = new Regex(LONG_OPTION_REGEX);
        private static readonly Regex _longOptionAliasRegex = new Regex(LONG_OPTION_ALIAS_REGEX);


        private List<Option> Options { get; set; }
        private Dictionary<Type, object> TypeConverters { get; set; }

        public Arguments()
        {
            Options = new List<Option>();
            TypeConverters = new Dictionary<Type, object>();
            PerformDefaultSetup();
        }

        private void PerformDefaultSetup()
        {
            RegisterTypeConverter(string.Copy);
            RegisterTypeConverter(int.Parse);
            RegisterTypeConverter(float.Parse);
            RegisterTypeConverter(double.Parse);
            RegisterTypeConverter(bool.Parse);
        }

        private IEnumerable<OptionAlias> ParseAliases(string aliases)
        {
            if (aliases == null)
                throw new ArgumentNullException("aliases");
            return aliases
                .Split(OPTION_ALIAS_SEPARATOR.ToCharArray())
                .Select(ParseAlias);
        }

        /// <summary>
        /// Detects type of the Option alias and removes its prefix if present.
        /// </summary>
        /// <example>
        /// Alias = "v", OptionType = Short:
        /// <code>
        /// var optionAlias1 = Arguments.ParseAlias("v");
        /// var optionAlias2 = Arguments.ParseAlias("-v");
        /// </code>
        /// Alias = "verbose", OptionType = Long:
        /// <code>
        /// var optionAlias3 = Arguments.ParseAlias("verbose");
        /// var optionAlias4 = Arguments.ParseAlias("--verbose");
        /// </code>
        /// Alias = "v", OptionType = Long:
        /// <code>
        /// var optionAlias5 = Arguments.ParseAlias("--v");
        /// </code>
        /// Alias = "verbose", OptionType = Short:
        /// <code>
        /// var optionAlias6 = Arguments.ParseAlias("-verbose");
        /// </code>
        /// </example>
        /// <param name="alias">User specified alias</param>
        /// <returns>OptionAlias with alias and its type</returns>
        private OptionAlias ParseAlias(string alias)
        {
            OptionType type;
            string realAlias;
            if (_shortOptionAliasRegex.IsMatch(alias))
            {
                type = OptionType.Short;
                realAlias = RemoveOptionalPrefix(alias, SHORT_OPTION_PREFIX);
            }
            else if (_longOptionAliasRegex.IsMatch(alias))
            {
                type = OptionType.Long;
                realAlias = RemoveOptionalPrefix(alias, LONG_OPTION_PREFIX);
            }
            else
            {
                throw new ArgumentsSetupException("Invalid Option alias: {0}", alias);
            }

            return new OptionAlias(realAlias, type);
        }

        private string RemoveOptionalPrefix(string value, string prefix)
        {
            return value.StartsWith(prefix)
                ? value.Remove(0, prefix.Length)
                : value;
        }

        internal void RegisterOptionAliases(Option option, string aliases)
        {
            foreach (var alias in ParseAliases(aliases))
            {
                RegisterOptionAlias(option, alias);
            }
        }

        private void RegisterOptionAlias(Option option, OptionAlias alias)
        {

        }

        internal T Convert<T>(string value)
        {
            // TODO
            if (!TypeConverters.ContainsKey(typeof(T)))
            {
                // TODO throw an exception
            }
            return ((Func<string, T>)TypeConverters[typeof(T)]).Invoke(value);
        }

        #endregion

        #region API

        /// <summary>
        /// Registers types to be used as Option or Plain Arguments, along with
        /// their converter function. The converter function converts input
        /// string to the given type and returns the result.
        /// </summary>
        /// <typeparam name="T">Target converted type</typeparam>
        /// <param name="converterFunc">Function to convert string to type T</param>
        public void RegisterTypeConverter<T>(Func<string, T> converterFunc)
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
        public OptionBuilder AddOption(string aliases)
        {
            return new OptionBuilder(this).WithAliases(aliases);
        }

        /// <summary>
        /// Adds a mandatory Option to the current configuration with given
        /// aliases. Same as <see cref="AddOption">AddOption</see>
        /// </summary>
        /// <param name="aliases"></param>
        /// <returns></returns>
        public OptionBuilder AddMandatoryOption(string aliases)
        {
            return AddOption(aliases).SetMandatory(true);
        }

        /// <summary>
        /// Processes the command line input arguments.
        /// </summary>
        /// <param name="args">Arguments as passed to the Main</param>
        /// <exception cref="ArgumentsParseException">Arguments do not satisfy
        /// the definition</exception>
        public void Parse(string[] args)
        {
            // TODO implement
            if (false)
                throw new ArgumentsParseException("Something went terribly wrong", true);

        }

        /// <summary>
        /// Checks whether the user specified an option or not.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns>True if user specified an option or if a default
        /// value is defined</returns>
        public bool IsOptionSet(string alias)
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
        public T GetOptionValue<T>(string alias)
        {
            // TODO implement
            return Options.First().GetValue<T>();
        }

        /// <summary>
        /// Gets Option arguments converted to the specified type.
        /// </summary>
        /// <typeparam name="T">Return type of the values</typeparam>
        /// <param name="alias">One of the Option aliases</param>
        /// <returns>Typed Option values</returns>
        public IEnumerable<T> GetOptionValues<T>(string alias)
        {
            // TODO implement
            return new T[]{};
        }

        /// <summary>
        /// Gets Option arguments as string. Same as
        /// <see cref="GetOptionValuse{T}"/>, implicitly typed.
        /// </summary>
        /// <param name="alias">One of the Option aliases</param>
        /// <returns>Option values as string</returns>
        public IEnumerable<string> GetOptionValues(string alias)
        {
            return GetOptionValues<string>(alias);
        }

        /// <summary>
        /// Gets Option argument as string. Same as
        /// <see cref="GetOptionValue{T}"/>, implicitly typed.
        /// </summary>
        /// <param name="alias">One of the Option aliases</param>
        /// <returns>Option value as string</returns>
        public string GetOptionValue(string alias)
        {
            return GetOptionValue<string>(alias);
        }

        /// <summary>
        /// Returns a list of all arguments that do not correspond to Options.
        /// </summary>
        /// <typeparam name="T">Type of all arguments</typeparam>
        /// <returns>List of all plain arguments</returns>
        public IEnumerable<T> GetPlainArguments<T>()
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
        public IEnumerable<string> GetPlainArguments()
        {
            return GetPlainArguments<string>();
        }

        /// <summary>
        /// Removes all Option definitions and type converters, and resets
        /// Arguments to the default state.
        /// </summary>
        public void Reset()
        {
            // TODO implement
        }

        /// <summary>
        /// Build help text for all defined options with their descriptions
        /// </summary>
        public IEnumerable<string> BuildHelpText()
        {
            // TODO implement
            return new String[]{""};
        }
        #endregion

    }
}