using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ArgumentsLibrary.Exceptions;

namespace ArgumentsLibrary {

    internal class Parser {

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

        private CommandLine CommandLine { get; set; }

        private Converter Converter { get; set; }

        private Dictionary<OptionAlias, Option> Options { get; set; }

        private Parser(Converter converter,
            Dictionary<OptionAlias, Option> options) {
            CommandLine = new CommandLine(converter);
            Converter = converter;
            Options = options;
        }

        internal static CommandLine ParseArguments(IEnumerable<string> args,
            Converter converter,
            Dictionary<OptionAlias, Option> options) {
            var parser = new Parser(converter, options);
            parser.ProcessArguments(args);
            return parser.CommandLine;
        }

        private void SetCommandLineOptionValue(Option option, string value) {
            option.Aliases.ForEach(
                alias => { CommandLine.Options.Add(alias, value); });
        }

        private void AddCommandLinePlainArgument(string value) {
            CommandLine.PlainArguments.Add(value);
        }

        private void ProcessArguments(IEnumerable<string> args) {
            var queue = new Queue<string>(args);
            var separatorHit = false;

            while (queue.Any()) {
                var arg = queue.Dequeue();

                if (arg.Equals(PLAIN_ARGUMENTS_SEPARATOR)) {
                    separatorHit = true;
                }

                var optionAlias = DetectPotentialOption(arg);
                if (optionAlias == null || !Options.ContainsKey(optionAlias) ||
                    separatorHit) {
                    AddCommandLinePlainArgument(arg);

                    //Reason for next line:
                    //test case: -a -unknownOption -b a b c
                    //test case: -a -unknownOption -b -- a b c
                    //plains: -unknownOption a b c - it is bug (probably)
                    // TODO
                    // test case: arg1 arg2 -o ? i.e. options after arguments
                    // test case: -o oarg arg1 -o2 > is -o2 a plain argument? exception?
                    separatorHit = true;

                    continue;
                }

                ProcessDetectedOption(optionAlias, arg, queue);
            }
        }

        private void ProcessDetectedOption(OptionAlias optionAlias,
            string arg,
            Queue<string> argQueue) {
            var option = Options[optionAlias];
            option.IsSet = true;
            option.Actions.ForEach(action => action.Invoke());

            if (option.Argument != null) {
                var value = optionAlias.Type == OptionType.Long
                    ? ExtractLongOptionValue(arg)
                    : argQueue.Peek();
                ProcessPotentialOptionArgument(optionAlias, option, value);
            }
        }

        private void ProcessPotentialOptionArgument(OptionAlias optionAlias,
            Option option,
            string arg) {
            dynamic argument = option.Argument;

            var nextOptionAlias = DetectPotentialOption(arg);
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

        /// <summary>
        /// Decides whether the input argument is an Option or not,
        /// i.e. whether the input string is in format "-o" or "--option=..."
        /// </summary>
        /// <param name="arg">Input argument from a command line</param>
        /// <returns>OptionAlias of detected potential Option or null</returns>
        private static OptionAlias DetectPotentialOption(string arg) {
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

        internal static IEnumerable<OptionAlias> ParseAliases(string aliases) {
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
                throw new ArgumentException("Input alias is not in valid format");
            }

            return new OptionAlias(realAlias, type);
        }

        private static string RemoveOptionalPrefix(string value, string prefix) {
            return value.StartsWith(prefix)
                ? value.Remove(0, prefix.Length)
                : value;
        }

    }

}