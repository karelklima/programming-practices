using System;
using System.Collections.Generic;
using ArgumentsLibrary.Exceptions;

namespace ArgumentsLibrary {

    public class CommandLine {

        internal Converter Converter { get; private set; }

        internal Dictionary<OptionAlias, string> Options { get; private set; }

        internal List<string> PlainArguments { get; private set; }

        internal CommandLine(Converter converter) {
            Converter = converter;
            Options = new Dictionary<OptionAlias, string>();
            PlainArguments = new List<string>();
        }

        /// <summary>
        /// Checks whether the user specified an option or not.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns>True if user specified an option or if a default
        /// value is defined</returns>
        public bool IsOptionSet(string alias) {
            if (alias == null) {
                throw new CommandLineException("Option alias cannot be null");
            }
            try {
                return Options.ContainsKey(Parser.ParseAlias(alias));
            }
            catch (ArgumentException) {
                throw new CommandLineException("Option alias format invalid");
            }
        }

        /// <summary>
        /// Gets Option argument converted to the specified type.
        /// </summary>
        /// <typeparam name="T">Return type of the value</typeparam>
        /// <param name="alias">One of the Option aliases</param>
        /// <returns>Typed Option value</returns>
        public T GetOptionValue<T>(string alias) {
            if (IsOptionSet(alias)) {
                var optionAlias = Parser.ParseAlias(alias);
                try {
                    return Converter.Convert<T>(Options[optionAlias]);
                }
                catch (Exception) {
                    throw new CommandLineException(
                        "Unable to convert option {0} to type {1}", alias,
                        typeof (T).ToString());
                }
            }

            throw new CommandLineException("Option with alias {0} is not set",
                alias);
        }

        /// <summary>
        /// Gets Option argument as string. Same as
        /// <see cref="GetOptionValue{T}"/>, implicitly typed.
        /// </summary>
        /// <param name="alias">One of the Option aliases</param>
        /// <returns>Option value as string</returns>
        public string GetOptionValue(string alias) {
            return GetOptionValue<string>(alias);
        }

        public IEnumerable<T> GetPlainArguments<T>() {
            try {
                return PlainArguments.ConvertAll(s => Converter.Convert<T>(s));
            }
            catch (Exception) {
                throw new CommandLineException(
                    "Cannot convert all arguments to type {0}", typeof (T));
            }
        }

        /// <summary>
        /// Implicit alternative to <see cref="GetPlainArguments{T}"/>.
        /// Returns a list of all arguments that do not correspond to Options
        /// as a list of strings.
        /// </summary>
        /// <returns>List of all plain arguments</returns>
        public IEnumerable<string> GetPlainArguments() {
            return GetPlainArguments<string>();
        }

    }

}