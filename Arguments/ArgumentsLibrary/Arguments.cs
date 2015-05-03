using System;
using System.Collections.Generic;
using ArgumentsLibrary.Builders;
using ArgumentsLibrary.Exceptions;

namespace ArgumentsLibrary {

    /// <summary>
    /// Arguments Library entry point
    /// </summary>
    /// <example>
    /// <code>
    /// var arguments = new Arguments(); <BR/>
    /// arguments.AddOption(...) <BR/>
    /// ...  <BR/>
    /// arguments.parse(args) <BR/>
    /// </code>
    /// </example>
    public sealed class Arguments {
        #region Internals

        /// <summary>
        /// Gets or sets the converter.
        /// </summary>
        /// <value>Type converters.</value>
        private Converter Converter { get; set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>Option-Alias dictionary</value>
        private Dictionary<OptionAlias, Option> Options { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentsLibrary.Arguments"/> class.
        /// </summary>
        public Arguments() {
            Converter = new Converter();
            Options = new Dictionary<OptionAlias, Option>();
            RegisterDefaultTypeConverters();
        }

        /// <summary>
        /// Registers the default type converters.
        /// Types are: string, int, float, double, bool
        /// </summary>
        private void RegisterDefaultTypeConverters() {
            RegisterTypeConverter(string.Copy);
            RegisterTypeConverter(int.Parse);
            RegisterTypeConverter(float.Parse);
            RegisterTypeConverter(double.Parse);
            RegisterTypeConverter(bool.Parse);
        }

        /// <summary>
        /// Registers multiple option aliases.
        /// </summary>
        /// <param name="option">Option.</param>
        /// <param name="aliases">List of aliases. See <see cref="Parser.ParseAliases"/> method.</param>
        private void RegisterOptionAliases(Option option, string aliases) {
            try {
                foreach (var alias in Parser.ParseAliases(aliases)) {
                    RegisterOptionAlias(option, alias);
                }
            }
            catch (ArgumentException exception) {
                throw new SetupException(exception.Message);
            }
        }

        /// <summary>
        /// Registers one option alias.
        /// </summary>
        /// <param name="option">Option.</param>
        /// <param name="alias">Alias. See <see cref="OptionAlias"/> class.</param>
        private void RegisterOptionAlias(Option option, OptionAlias alias) {
            Options.Add(alias, option);
            option.Aliases.Add(alias);
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
        public void RegisterTypeConverter<T>(Func<string, T> converterFunc) {
            if (converterFunc == null) {
                throw new SetupException("Converter function cannot be null");
            }
            Converter.RegisterTypeConverter(converterFunc);
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
        /// Arguments.AddOption("v|verbose"); <BR/>
        /// Arguments.AddOption("-v|--verbose"); <BR/>
        /// Arguments.AddOption("v").WithAlias("verbose"); <BR/>
        /// Arguments.AddOption("-v").WithAlias("--verbose"); <BR/>
        /// </code>
        ///  <BR/>
        /// The following examples present non-standard usage:
        /// <code>
        /// Arguments.AddOption("v|-verbose"); <BR/>
        /// Arguments.AddOption("--v").WithAlias("--verbose"); <BR/>
        /// </code>
        /// </example>
        /// <param name="aliases">One or more option aliases</param>
        /// <returns>OptionBuilder instance</returns>
        public OptionBuilder AddOption(string aliases) {
            return new OptionBuilder(RegisterOptionAliases)
                .WithAliases(aliases);
        }

        /// <summary>
        /// Adds a mandatory Option to the current configuration with given
        /// aliases. Same as <see cref="AddOption">AddOption</see>
        /// </summary>
        /// <param name="aliases">One or more option aliases</param>
        /// <returns>OptionBuilder instance</returns>
        public OptionBuilder AddMandatoryOption(string aliases) {
            return AddOption(aliases).SetMandatory();
        }

        /// <summary>
        /// Processes the command line input arguments.
        /// </summary>
        /// <param name="args">Arguments as passed to the Main</param>
        /// <exception cref="ParseException">Arguments do not satisfy
        /// the definition</exception>
        public CommandLine Parse(IEnumerable<string> args) {
            if (args == null) {
                throw new ParseException("Passed arguments cannot be null");
            }

            return Parser.ParseArguments(args, Converter, Options);
        }

        /// <summary>
        /// Builds help text for all defined options with their descriptions
        /// </summary>
        public string BuildHelpText(string usage = null) {
            return HelpTextGenerator.Generate(Options, usage);
        }

        #endregion
    }

}