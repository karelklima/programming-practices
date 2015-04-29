using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ArgumentsLibrary.Builders;
using ArgumentsLibrary.Exceptions;

namespace ArgumentsLibrary
{
    /// <summary>
    /// Arguments Library entry point
    /// </summary>
    /// <example>
    /// <code>
    /// var arguments = new Arguments();
    /// arguments.AddOption(...)
    /// ...
    /// arguments.parse(args)
    /// </code>
    /// </example>
    public sealed class Arguments
    {

        #region Internals

        private const string SHORT_OPTION_PREFIX = "-";
        private const string SHORT_OPTION_REGEX = "^-([a-zA-Z][a-zA-Z-_]*)$";
        private const string SHORT_OPTION_ALIAS_REGEX =
            "^(([a-zA-Z])|-([a-zA-Z][a-zA-Z-_]*))$";
        
        private const string LONG_OPTION_PREFIX = "--";
        private const string LONG_OPTION_VALUE_SEPARATOR = "=";
        private const string LONG_OPTION_REGEX =
            "^--([a-zA-Z][a-zA-Z-_]*)(=.+)?$";
        private const string LONG_OPTION_ALIAS_REGEX =
            "^(([a-zA-Z][a-zA-Z-_]+)|--([a-zA-Z][a-zA-Z-_]*))$";

        private const string OPTION_ALIAS_SEPARATOR = "|";
        private const string PLAIN_ARGUMENTS_SEPARATOR = "--";

        private static readonly Regex _shortOptionRegex =
            new Regex(SHORT_OPTION_REGEX);
        private static readonly Regex _shortOptionAliasRegex =
            new Regex(SHORT_OPTION_ALIAS_REGEX);

        private static readonly Regex _longOptionRegex =
            new Regex(LONG_OPTION_REGEX);
        private static readonly Regex _longOptionAliasRegex =
            new Regex(LONG_OPTION_ALIAS_REGEX);

        private Dictionary<Type, object> TypeConverters { get; set; }

        private Dictionary<OptionAlias, Option> Options { get; set; }

        private List<string> PlainArguments { get; set; } 

        private bool Sealed { get; set; }

        public Arguments()
        {
            TypeConverters = new Dictionary<Type, object>();
            Options = new Dictionary<OptionAlias, Option>();
            PlainArguments = new List<string>();
            Sealed = false;
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
                throw new ArgumentsSetupException(
                    "Invalid Option alias: {0}",
                    alias
                    );
            }

            return new OptionAlias(realAlias, type);
        }

        private string RemoveOptionalPrefix(string value, string prefix)
        {
            return value.StartsWith(prefix)
                ? value.Remove(0, prefix.Length)
                : value;
        }

        private void RegisterOptionAliases(Option option, string aliases)
        {
            foreach (var alias in ParseAliases(aliases))
            {
                RegisterOptionAlias(option, alias);
            }
        }

        private void RegisterOptionAlias(Option option, OptionAlias alias)
        {
            Options.Add(alias, option);
            option.Aliases.Add(alias);
        }

        private void ParseArguments(IEnumerable<string> args)
        {
            var queue = new Queue<string>(args);
            var separatorHit = false;

            while (queue.Any())
            {
                var arg = queue.Dequeue();

                if (arg.Equals(PLAIN_ARGUMENTS_SEPARATOR))
                {
                    separatorHit = true;
                }

                var optionAlias = DetectPotentialOption(arg);

                if (optionAlias == null || !Options.ContainsKey(optionAlias) || separatorHit)
                {
                    PlainArguments.Add(arg);
                    continue;
                }

                ProcessDetectedOption(optionAlias, arg, queue);
            }

        }

        private void ProcessDetectedOption(OptionAlias optionAlias, string arg, Queue<string> argQueue)
        {
            var option = Options[optionAlias];
            option.IsSet = true;
            option.Actions.ForEach(action => action.Invoke());

            if (option.Argument != null)
            {
                var value = optionAlias.Type == OptionType.Long ? ExtractLongOptionValue(arg) : argQueue.Peek();
                var result = ProcessPotentialOptionArgument(option, value);
            }
            
        }

        private bool ProcessPotentialOptionArgument(Option option, string arg)
        {
            dynamic argument = option.Argument;
            if (arg == null && argument.Optional)
                throw new ArgumentsParseException("Option {0} has a mandato");
        }

        private void ProcessLongOptionArgument(OptionAlias optionAlias, string arg)
        {

        }

        private void ProcessShortOption(OptionAlias optionAlias, string arg, Queue<string> argQueue)
        {

        }

        private string ExtractLongOptionValue(string arg)
        {
            var longMatch = _longOptionRegex.Match(arg);
            return RemoveOptionalPrefix(longMatch.Groups[2].Value, LONG_OPTION_VALUE_SEPARATOR);
        }

        /// <summary>
        /// Decides whether the input argument is an Option or not,
        /// i.e. whether the input string is in format "-o" or "--option=..."
        /// </summary>
        /// <param name="arg">Input argument from a command line</param>
        /// <returns>OptionAlias of detected potential Option or null</returns>
        private OptionAlias DetectPotentialOption(string arg)
        {
            var longMatch = _longOptionRegex.Match(arg);
            if (longMatch.Success)
            {
                return new OptionAlias(longMatch.Groups[1].Value, OptionType.Long);
            }
            var shortMatch = _shortOptionRegex.Match(arg);
            if (shortMatch.Success)
            {
                return new OptionAlias(shortMatch.Groups[1].Value, OptionType.Short);
            }
            return null;
        }

        /// <summary>
        /// Converts a given string value to desired type using defined
        /// type converters.
        /// </summary>
        /// <typeparam name="T">Type to be converted into</typeparam>
        /// <param name="value">String value to be converted</param>
        /// <returns>Converted value</returns>
        private T Convert<T>(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (!TypeConverters.ContainsKey(typeof(T)))
            {
                throw new ArgumentsParseException("Converter for type {0} is not defined", typeof(T));
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
        /// <param name="converterFunc">Function to convert string to type T
        /// </param>
        public void RegisterTypeConverter<T>(Func<string, T> converterFunc)
        {
            if (converterFunc == null)
                throw new ArgumentsSetupException("Converter function cannot be null");
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
            if (Sealed)
                throw new ArgumentsSetupException("Definition of Options is already sealed");
            return new OptionBuilder(RegisterOptionAliases).WithAliases(aliases);
        }

        /// <summary>
        /// Adds a mandatory Option to the current configuration with given
        /// aliases. Same as <see cref="AddOption">AddOption</see>
        /// </summary>
        /// <param name="aliases">One or more option aliases</param>
        /// <returns>OptionBuilder instance</returns>
        public OptionBuilder AddMandatoryOption(string aliases)
        {
            return AddOption(aliases).SetMandatory();
        }

        /// <summary>
        /// Processes the command line input arguments.
        /// </summary>
        /// <param name="args">Arguments as passed to the Main</param>
        /// <exception cref="ArgumentsParseException">Arguments do not satisfy
        /// the definition</exception>
        public void Parse(string[] args)
        {
            if (args == null)
                throw new ArgumentsParseException("Passed arguments cannot be null");
            if (Sealed)
                throw new ArgumentsParseException("Arguments class is already sealed");
            else
                Sealed = true;

            ParseArguments(args);
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
            return new T[]{}.First();
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