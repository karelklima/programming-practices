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

        private Converter Converter { get; set; }

        private Dictionary<OptionAlias, Option> Options { get; set; }

        private List<string> PlainArguments { get; set; } 

        private bool Sealed { get; set; }

        public Arguments()
        {
            Converter = new Converter();
            Options = new Dictionary<OptionAlias, Option>();
            PlainArguments = new List<string>();
            Sealed = false;
            RegisterDefaultTypeConverters();
        }

        private void RegisterDefaultTypeConverters()
        {
            RegisterTypeConverter(string.Copy);
            RegisterTypeConverter(int.Parse);
            RegisterTypeConverter(float.Parse);
            RegisterTypeConverter(double.Parse);
            RegisterTypeConverter(bool.Parse);
        }

        private void RegisterOptionAliases(Option option, string aliases)
        {
            foreach (var alias in Parser.ParseAliases(aliases))
            {
                RegisterOptionAlias(option, alias);
            }
        }

        private void RegisterOptionAlias(Option option, OptionAlias alias)
        {
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
        public void RegisterTypeConverter<T>(Func<string, T> converterFunc)
        {
            if (converterFunc == null)
                throw new ArgumentsSetupException("Converter function cannot be null");
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

            Parser.ProcessArguments(args, Converter, Options, PlainArguments);
        }

        /// <summary>
        /// Checks whether the user specified an option or not.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns>True if user specified an option or if a default
        /// value is defined</returns>
        public bool IsOptionSet(string alias)
        {
            OptionAlias optionAlias = Parser.ParseAlias(alias);
            if (Options.ContainsKey (optionAlias)) {
                return Options [optionAlias].IsSet;
            }
            //TODO throw exception "options is not found"?
            return false;
        }

        /// <summary>
        /// Gets Option argument converted to the specified type.
        /// </summary>
        /// <typeparam name="T">Return type of the value</typeparam>
        /// <param name="alias">One of the Option aliases</param>
        /// <returns>Typed Option value</returns>
        public T GetOptionValue<T>(string alias)
        {
            OptionAlias optionAlias = Parser.ParseAlias (alias);
            if (Options.ContainsKey (optionAlias)) {
                Option option = Options [optionAlias];
                if (option.Argument != null)
                    return option.Argument.Value;
                //TODO throw exception "argument is not defined"
            }
            //TODO throw exception "options is not found"?
            return default(T);
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
        /// Implicit alternative to <see cref="GetPlainArguments{T}"/>.
        /// Returns a list of all arguments that do not correspond to Options
        /// as a list of strings.
        /// </summary>
        /// <returns>List of all plain arguments</returns>
        public IEnumerable<string> GetPlainArguments()
        {
            return PlainArguments;
        }

        /// <summary>
        /// Builds help text for all defined options with their descriptions
        /// </summary>
        public string BuildHelpText()
        {
            return HelpTextGenerator.Generate(Options);
        }

        #endregion

    }
}