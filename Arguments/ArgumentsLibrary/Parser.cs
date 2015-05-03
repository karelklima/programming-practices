﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ArgumentsLibrary.Exceptions;

namespace ArgumentsLibrary {

    /// <summary>
    /// This class is used for processing command line input arguments.
    /// </summary>
    internal class Parser {

        /// <summary>
        /// Definition of a prefix for a short option, i.e. -o
        /// </summary>
        private const string SHORT_OPTION_PREFIX = "-";

        /// <summary>
        /// Regular expression for detecting a short option, i.e. -o, -option
        /// </summary>
        /// <remarks>
        /// An option is considered short if it starts with short option prefix
        /// </remarks>
        private const string SHORT_OPTION_REGEX = "^-([a-zA-Z][a-zA-Z-_]*)$";

        /// <summary>
        /// Regular expression for detecting a short option alias,
        /// i.e. o, -o, -option
        /// </summary>
        /// <remarks>
        /// A single letter is considered a short option
        /// </remarks>
        private const string SHORT_OPTION_ALIAS_REGEX =
            "^(([a-zA-Z])|-([a-zA-Z][a-zA-Z-_]*))$";

        /// <summary>
        /// Definition of a prefix for a long option, i.e. --option
        /// </summary>
        private const string LONG_OPTION_PREFIX = "--";

        /// <summary>
        /// Definition of a separator of option definition and its value,
        /// i.e. "=" in --option=value
        /// </summary>
        private const string LONG_OPTION_VALUE_SEPARATOR = "=";

        /// <summary>
        /// Regular expression for detecting a long option,
        /// i.e. --o, --option, --option=value
        /// </summary>
        /// <remarks>
        /// An option is considered long if it starts with long option prefix
        /// </remarks>
        private const string LONG_OPTION_REGEX =
            "^--([a-zA-Z][a-zA-Z-_]*)(=.+)?$";

        /// <summary>
        /// Regular expression for detecting a long option alias,
        /// i.e. option, --o, --option
        /// </summary>
        /// <remarks>
        /// An array of at least two letters is considered a long option
        /// </remarks>
        private const string LONG_OPTION_ALIAS_REGEX =
            "^(([a-zA-Z][a-zA-Z-_]+)|--([a-zA-Z][a-zA-Z-_]*))$";

        /// <summary>
        /// Definition of a separator of option aliases,
        /// i.e. "o|option" stands for -o and -option
        /// </summary>
        private const string OPTION_ALIAS_SEPARATOR = "|";

        /// <summary>
        /// Definition of a manual plain command line arguments separator,
        /// when this separator is detected, the rest of the arguments are
        /// considered plain arguments (not options) even though they could
        /// be formatted as options
        /// </summary>
        private const string PLAIN_ARGUMENTS_SEPARATOR = "--";

        /// <summary>
        /// Regex instance of <see cref="SHORT_OPTION_REGEX"/>
        /// </summary>
        private static readonly Regex _shortOptionRegex =
            new Regex(SHORT_OPTION_REGEX);

        /// <summary>
        /// Regex instance of <see cref="SHORT_OPTION_ALIAS_REGEX"/>
        /// </summary>
        private static readonly Regex _shortOptionAliasRegex =
            new Regex(SHORT_OPTION_ALIAS_REGEX);

        /// <summary>
        /// Regex instance of <see cref="LONG_OPTION_REGEX"/>
        /// </summary>
        private static readonly Regex _longOptionRegex =
            new Regex(LONG_OPTION_REGEX);

        /// <summary>
        /// Regex instance of <see cref="LONG_OPTION_ALIAS_REGEX"/>
        /// </summary>
        private static readonly Regex _longOptionAliasRegex =
            new Regex(LONG_OPTION_ALIAS_REGEX);

        /// <summary>
        /// CommandLine instance to insert detected options and arguments into
        /// </summary>
        private CommandLine CommandLine { get; set; }

        /// <summary>
        /// Input command line arguments to process
        /// </summary>
        private Queue<string> Args { get; set; }

        /// <summary>
        /// Converter instance to convert string arguments to various types
        /// </summary>
        private Converter Converter { get; set; }

        /// <summary>
        /// Definition of setup Options
        /// </summary>
        private Dictionary<OptionAlias, Option> Options { get; set; }

        /// <summary>
        /// Private constructor used for parsing command line arguments
        /// </summary>
        /// <param name="converter">Converter instance</param>
        /// <param name="options">Definition of setup Options</param>
        private Parser(IEnumerable<string> args, Converter converter,
            Dictionary<OptionAlias, Option> options) {
            CommandLine = new CommandLine(converter);
            Args = new Queue<string>(args);
            Converter = converter;
            Options = options;
        }

        /// <summary>
        /// Class entry point for parsing command line arguments. It accepts
        /// arguments, user definition of Options, user definition of Converter
        /// and uses this information to process the command line arguments
        /// and outputs a CommandLine instance with detected options, their
        /// values and plain arguments
        /// </summary>
        /// <param name="args">Command line input arguments</param>
        /// <param name="converter">Converter instance</param>
        /// <param name="options">Definition of setup Options</param>
        /// <returns>
        /// CommandLine instance with detected options and arguments
        /// </returns>
        internal static CommandLine ParseArguments(IEnumerable<string> args,
            Converter converter,
            Dictionary<OptionAlias, Option> options) {
            var parser = new Parser(args, converter, options);
            parser.ProcessArguments();
            return parser.CommandLine;
        }

        /// <summary>
        /// Detects types of Option aliases separated by separator 
        /// <see cref="OPTION_ALIAS_SEPARATOR"/>.
        /// For details on parsing aliases see <see cref="ParseAlias"/>
        /// </summary>
        /// <param name="aliases">User specified aliases</param>
        /// <returns>Set of detected OptionAlias instances</returns>
        internal static IEnumerable<OptionAlias> ParseAliases(string aliases) {
            if (aliases == null) {
                throw new ArgumentNullException("aliases");
            }

            return aliases
                .Split(OPTION_ALIAS_SEPARATOR.ToCharArray())
                .Select(ParseAlias).ToList();
        }

        /// <summary>
        /// Detects type of the Option alias and removes its prefix if present.
        /// </summary>
        /// <example>
        /// Alias = "v", OptionType = Short:
        /// <code>
        /// var optionAlias1 = Parser.ParseAlias("v");
        /// var optionAlias2 = Parser.ParseAlias("-v");
        /// </code>
        /// Alias = "verbose", OptionType = Long:
        /// <code>
        /// var optionAlias3 = Parser.ParseAlias("verbose");
        /// var optionAlias4 = Parser.ParseAlias("--verbose");
        /// </code>
        /// Alias = "v", OptionType = Long:
        /// <code>
        /// var optionAlias5 = Parser.ParseAlias("--v");
        /// </code>
        /// Alias = "verbose", OptionType = Short:
        /// <code>
        /// var optionAlias6 = Parser.ParseAlias("-verbose");
        /// </code>
        /// </example>
        /// <param name="alias">User specified alias</param>
        /// <returns>OptionAlias with alias and its type</returns>
        internal static OptionAlias ParseAlias(string alias) {
            OptionType type;
            string realAlias;
            if (_shortOptionAliasRegex.IsMatch(alias)) {
                type = OptionType.Short;
                realAlias = RemoveOptionalPrefix(alias, SHORT_OPTION_PREFIX);
            }
            else if (_longOptionAliasRegex.IsMatch(alias)) {
                type = OptionType.Long;
                realAlias = RemoveOptionalPrefix(alias, LONG_OPTION_PREFIX);
            }
            else {
                throw new ArgumentException(
                    "Input alias is not in valid format");
            }

            return new OptionAlias(realAlias, type);
        }

        /// <summary>
        /// Inserts a "is set" marker to the CommandLine for input Option
        /// </summary>
        /// <param name="option">Option that is detected in arguments</param>
        private void SetCommandLineOptionIsSet(Option option) {
            option.Aliases.ForEach(alias => {
                if (!CommandLine.Options.ContainsKey(alias)) {
                    CommandLine.Options[alias] = null; // is set indicator
                }
            });
        }

        /// <summary>
        /// Inserts a value for specified Option to the CommandLine
        /// </summary>
        /// <param name="option">Detected Option</param>
        /// <param name="value">Option value</param>
        private void SetCommandLineOptionValue(Option option, object value) {
            option.Aliases.ForEach(
                alias => { CommandLine.Options.Add(alias, value); });
        }

        /// <summary>
        /// Inserts a plain argument to the CommandLine
        /// </summary>
        /// <param name="value">Plain argument value</param>
        private void AddCommandLinePlainArgument(string value) {
            CommandLine.PlainArguments.Add(value);
        }

        /// <summary>
        /// Processes input command line arguments, i.e. detects Options and
        /// their values, invokes user specified callbacks, detects plain
        /// arguments and constructs a inputs this information to the
        /// CommandLine object
        /// </summary>
        private void ProcessArguments() {
            while (Args.Any()) {
                if (DetectPlainArgumentsSeparator()) {
                    // The rest of the arguments are plain arguments except
                    // the first one, which is the detected separator
                    Args.Dequeue();
                    // Add all arguments to the CommandLine
                    Args.ToList().ForEach(AddCommandLinePlainArgument);
                    Args.Clear();
                    // Finish the cycle as there are no more args to process
                    break;
                }

                // Check if the next argument could be an Option
                var optionAlias = DetectPotentialOption();

                if (optionAlias == null) {
                    // All remaining arguments should be plain arguments
                    AddCommandLinePlainArgument(Args.Dequeue());
                }
                else if (!Options.ContainsKey(optionAlias)) {
                    // Unknown option was detected
                    throw new ParseException(
                        "Unknown program option detected: {0}",
                        optionAlias.ToString());
                }
                else if (CommandLine.PlainArguments.Any()) {
                    // There were already some plain arguments detected
                    throw new ParseException(
                        "Unexpected option detected in plain arguments: {0}",
                        optionAlias.ToString());
                }
                else {
                    // Detected option and correct placement in input arguments
                    ProcessDetectedOption(optionAlias);
                }
            }
        }

        /// <summary>
        /// Checks if the next argument is plain arguments separator,
        /// i.e. if it equals to <see cref="PLAIN_ARGUMENTS_SEPARATOR"/>
        /// </summary>
        /// <returns>
        /// True if next argument in Args is a plain arguments separator
        /// </returns>
        private bool DetectPlainArgumentsSeparator() {
            return Args.First().Equals(PLAIN_ARGUMENTS_SEPARATOR);
        }

        /// <summary>
        /// Decides whether the next argument is an Option or not,
        /// i.e. whether next argument string is in format "-o" or "--option=*"
        /// </summary>
        /// <returns>OptionAlias of detected potential Option or null</returns>
        private OptionAlias DetectPotentialOption() {
            var arg = Args.First();
            var longMatch = _longOptionRegex.Match(arg);
            if (longMatch.Success) {
                return new OptionAlias(longMatch.Groups[1].Value,
                    OptionType.Long);
            }
            var shortMatch = _shortOptionRegex.Match(arg);
            if (shortMatch.Success) {
                return new OptionAlias(shortMatch.Groups[1].Value,
                    OptionType.Short);
            }
            return null;
        }

        /// <summary>
        /// Processes detected Option, i.e. invokes user specified callbacks
        /// and checks potential Option arguments
        /// </summary>
        /// <param name="optionAlias">Option specified by its alias</param>
        private void ProcessDetectedOption(OptionAlias optionAlias) {
            // locate corresponding Option
            var option = Options[optionAlias];
            // set default value to null
            SetCommandLineOptionIsSet(option);
            // invoke user callbacks
            option.InvokeActions();

            if (optionAlias.Type == OptionType.Long) {
                ProcessLongOptionArgument(optionAlias, option);
            }
            else {
                ProcessShortOptionArgument(optionAlias, option);
            }
        }

        private void ProcessLongOptionArgument(OptionAlias optionAlias,
            Option option) {
            var stringValue = ExtractLongOptionValue(Args.First());

            if (stringValue.Length > 0 && option.Argument == null) {
                // There is an Option value present, but it should not be there
                throw new ParseException(
                    "Unexpected long option value: {0}", Args.First());
            }
            else if (stringValue.Length == 0 && option.Argument != null) {
                // An argument or default value is expected
                if (!option.Argument.Optional) {
                    // Argument is mandatory and no value is provided
                    throw new ParseException(
                        "No option value specified for option {0}",
                        Args.First());
                }
                SetCommandLineOptionValue(option, option.Argument.DefaultValue);
            }
            else if (stringValue.Length > 0 && option.Argument != null) {
                // An argument is expected and its value is set
                try {
                    dynamic value = option.Argument.Parse(stringValue);
                    option.Argument.AssertConditions(value);
                    option.Argument.InvokeActions(value);
                    SetCommandLineOptionValue(option, value);
                }
                catch (ArgumentOutOfRangeException) {
                    // Value does not satisfy Argument Conditions
                    throw new ParseException(
                        "Value for option {0} does not satisfy required conditions",
                        optionAlias.ToString());
                }
                catch (Exception) {
                    // Value cannot be converted to specified format
                    throw new ParseException(
                        "Value for option {0} is not of required type",
                        optionAlias.ToString());
                }
            }

            // Remove processed option from arguments list
            Args.Dequeue();
        }

        private void ProcessShortOptionArgument(OptionAlias optionAlias,
            Option option) {
        }

        private void ProcessPotentialOptionArgument(OptionAlias optionAlias,
            Option option, string arg) {
            dynamic argument = option.Argument;

            var nextOptionAlias = DetectPotentialOption();
            if (nextOptionAlias != null && Options.ContainsKey(nextOptionAlias)) {
                arg = null;
            }

            if (arg == null && !argument.Optional) {
                throw new ParseException(
                    "Option {0} has mandatory argument {1}", optionAlias,
                    argument.Name);
            }

            if (arg != null) {
                try {
                    argument.Value = argument.Parse(arg, Converter);
                    argument.InvokeActions();
                    if (!argument.AssertConditions()) {
                        throw new ParseException(
                            "Option: {1}, {0}={2}: conditions failed",
                            argument.Name, optionAlias, argument.Value);
                    }
                }
                catch (FormatException) {
                    if (!argument.Optional) {
                        throw new ParseException(
                            "Cannot parse \"{0}\" as `{1}` for {2}", arg,
                            argument.GetValueType().FullName, optionAlias);
                    }
                    else {
                        argument.Value = argument.DefaultValue;
                    }
                    //TODO conditions
                }
            }
            else {
                argument.Value = argument.DefaultValue;
                //TODO conditions
            }
        }

        private static string ExtractLongOptionValue(string arg) {
            var longMatch = _longOptionRegex.Match(arg);
            return RemoveOptionalPrefix(longMatch.Groups[2].Value,
                LONG_OPTION_VALUE_SEPARATOR);
        }

        private static string RemoveOptionalPrefix(string value, string prefix) {
            return value.StartsWith(prefix)
                ? value.Remove(0, prefix.Length)
                : value;
        }

    }

}