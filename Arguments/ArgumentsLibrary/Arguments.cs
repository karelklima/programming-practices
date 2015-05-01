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
    /// var arguments = new Arguments();
    /// arguments.AddOption(...)
    /// ...
    /// arguments.parse(args)
    /// </code>
    /// </example>
    public sealed class Arguments {
        #region Internals

        private Converter Converter { get; set; }

        private Dictionary<OptionAlias, Option> Options { get; set; }

        public Arguments() {
            Converter = new Converter();
            Options = new Dictionary<OptionAlias, Option>();
            RegisterDefaultTypeConverters();
        }

        private void RegisterDefaultTypeConverters() {
            RegisterTypeConverter(string.Copy);
            RegisterTypeConverter(int.Parse);
            RegisterTypeConverter(float.Parse);
            RegisterTypeConverter(double.Parse);
            RegisterTypeConverter(bool.Parse);
        }

        private void RegisterOptionAliases(Option option, string aliases) {
            foreach (var alias in Parser.ParseAliases(aliases)) {
                RegisterOptionAlias(option, alias);
            }
        }

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
        public CommandLine Parse(string[] args) {
            if (args == null) {
                throw new ParseException("Passed arguments cannot be null");
            }

            return Parser.ParseArguments(args, Converter, Options);
        }

        /// <summary>
        /// Builds help text for all defined options with their descriptions
        /// </summary>
        public string BuildHelpText() {
            return HelpTextGenerator.Generate(Options);
        }

        #endregion
    }

}