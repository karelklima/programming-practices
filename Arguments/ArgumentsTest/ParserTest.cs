using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using ArgumentsLibrary;
using ArgumentsLibrary.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArgumentsTest {

    /// <summary>
    /// Summary description for ParserTest
    /// </summary>
    [TestClass]
    public class ParserTest {

        /// <summary>
        /// Creates a mock Converter with string and int conversion support
        /// </summary>
        /// <returns>A Converter instance</returns>
        private Converter CreateConverter() {
            var converter = new Converter();
            converter.RegisterTypeConverter(string.Copy);
            converter.RegisterTypeConverter(int.Parse);
            return converter;
        }

        /// <summary>
        /// Factory for OptionAlias
        /// </summary>
        /// <param name="alias">Alias of the Option</param>
        /// <param name="type">Type of the Option</param>
        /// <returns>Constructed OptionAlias instance</returns>
        private OptionAlias CreateOptionAlias(string alias, OptionType type) {
            return new OptionAlias(alias, type);
        }

        /// <summary>
        /// Creates a mock Option with specified alias
        /// </summary>
        /// <param name="alias">OptionAlias of the Option</param>
        /// <returns>An Option instance</returns>
        private Option CreateOption(OptionAlias alias) {
            var option = new Option();
            option.Aliases.Add(alias);
            return option;
        }

        /// <summary>
        /// Creates a mock Option with specified alias and Argument
        /// </summary>
        /// <typeparam name="T">Type of the Argument</typeparam>
        /// <param name="alias">OptionAlias of the Option</param>
        /// <returns>An Option instance with specified Argument</returns>
        private Option CreateOptionWithArgument<T>(OptionAlias alias) {
            var option = CreateOption(alias);
            var arg = new Argument<T> {Name = "arg"};
            option.Argument = arg;
            return option;
        }

        /// <summary>
        /// Creates a mock Options dictionary
        /// </summary>
        /// <param name="options">List of Options to include</param>
        /// <returns>A Dictionary{OptionAlias, Option} instance</returns>
        private Dictionary<OptionAlias, Option> CreateOptionsDictionary(
            params Option[] options) {
            var dictionary = new Dictionary<OptionAlias, Option>();
            options.ToList()
                .ForEach(
                    option => {
                        option.Aliases.ForEach(
                            alias => dictionary.Add(alias, option));
                    });
            return dictionary;
        }

        [TestMethod]
        public void ParseAliases_Multiple_Ok() {
            var aliases = Parser.ParseAliases(
                "o|-o|--o|option|--option|-option").ToList();
            Assert.IsTrue(aliases.Count() == 6);
            Assert.IsTrue(aliases.Take(3).All(a => a.Alias == "o"));
            Assert.IsTrue(
                aliases.Skip(3).Take(3).All(a => a.Alias == "option"));
            Assert.IsTrue(
                aliases.Skip(2).Take(3).All(a => a.Type == OptionType.Long));
            Assert.IsTrue(
                aliases.FindAll(a => a.Type == OptionType.Short).Count() == 3);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void ParseAliases_Multiple_Invalid() {
            Parser.ParseAliases("o*|option");
        }

        [TestMethod]
        public void ParseAliases_Single_Ok() {
            var aliases = Parser.ParseAliases("option");
            Assert.IsTrue(aliases.Count() == 1);
            Assert.IsTrue(aliases.First().Alias.Equals("option"));
            Assert.IsTrue(aliases.First().Type == OptionType.Long);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void ParseAliases_Single_Invalid() {
            Parser.ParseAliases("*");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void ParseAliases_Null() {
            Parser.ParseAliases(null);
        }

        [TestMethod]
        public void ParseAlias_Ok() {
            Assert.AreEqual(
                Parser.ParseAlias("o"),
                new OptionAlias("o", OptionType.Short));
            Assert.AreEqual(
                Parser.ParseAlias("-o"),
                new OptionAlias("o", OptionType.Short));
            Assert.AreEqual(
                Parser.ParseAlias("--o"),
                new OptionAlias("o", OptionType.Long));
            Assert.AreEqual(
                Parser.ParseAlias("option"),
                new OptionAlias("option", OptionType.Long));
            Assert.AreEqual(
                Parser.ParseAlias("--option"),
                new OptionAlias("option", OptionType.Long));
            Assert.AreEqual(
                Parser.ParseAlias("-option"),
                new OptionAlias("option", OptionType.Short));
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void ParseAlias_Invalid() {
            Parser.ParseAlias("*");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void ParseAlias_Null() {
            Parser.ParseAlias(null);
        }

        [TestMethod]
        [ExpectedException(typeof (ParseException))]
        public void ParseArguments_UnknownLongOption() {
            var dictionary = CreateOptionsDictionary();
            string[] args = {"--option"};
            Parser.ParseArguments(args, CreateConverter(), dictionary);
        }

        [TestMethod]
        public void ParseArguments_LongOption_ValidArgument() {
            var alias = CreateOptionAlias("option", OptionType.Long);
            var option = CreateOptionWithArgument<int>(alias);
            var dictionary = CreateOptionsDictionary(option);
            string[] args = {"--option=10"};
            var cmd = Parser.ParseArguments(args, CreateConverter(), dictionary);
            Assert.IsTrue((int) cmd.Options[alias] == 10);
        }

        [TestMethod]
        [ExpectedException(typeof (ParseException))]
        public void ParseArguments_LongOption_InvalidArgument() {
            var alias = CreateOptionAlias("option", OptionType.Long);
            var option = CreateOptionWithArgument<int>(alias);
            var dictionary = CreateOptionsDictionary(option);
            string[] args = {"--option=text"};
            Parser.ParseArguments(args, CreateConverter(), dictionary);
        }

        [TestMethod]
        [ExpectedException(typeof (ParseException))]
        public void ParseArguments_LongOption_MissingArgument() {
            var alias = CreateOptionAlias("option", OptionType.Long);
            var option = CreateOptionWithArgument<string>(alias);
            var dictionary = CreateOptionsDictionary(option);
            string[] args = {"--option"};
            Parser.ParseArguments(args, CreateConverter(), dictionary);
        }

        [TestMethod]
        [ExpectedException(typeof (ParseException))]
        public void ParseArguments_UnknownShortOption() {
            var dictionary = CreateOptionsDictionary();
            string[] args = {"-o"};
            Parser.ParseArguments(args, CreateConverter(), dictionary);
        }

        [TestMethod]
        public void ParseArguments_ShortOption_ValidArgument() {
            var alias = CreateOptionAlias("o", OptionType.Short);
            var option = CreateOptionWithArgument<int>(alias);
            var dictionary = CreateOptionsDictionary(option);
            string[] args = {"-o", "10"};
            var cmd = Parser.ParseArguments(args, CreateConverter(), dictionary);
            Assert.IsTrue((int) cmd.Options[alias] == 10);
        }

        [TestMethod]
        [ExpectedException(typeof (ParseException))]
        public void ParseArguments_ShortOption_InvalidArgument() {
            var alias = CreateOptionAlias("o", OptionType.Short);
            var option = CreateOptionWithArgument<int>(alias);
            var dictionary = CreateOptionsDictionary(option);
            string[] args = {"-o", "text"};
            Parser.ParseArguments(args, CreateConverter(), dictionary);
        }

        [TestMethod]
        [ExpectedException(typeof (ParseException))]
        public void ParseArguments_ShortOption_MissingArgument() {
            var alias = CreateOptionAlias("o", OptionType.Short);
            var option = CreateOptionWithArgument<string>(alias);
            var dictionary = CreateOptionsDictionary(option);
            string[] args = {"-o"};
            Parser.ParseArguments(args, CreateConverter(), dictionary);
        }

        [TestMethod]
        public void ParseArguments_PlainArguments() {
            string[] args = {"arg1", "arg2"};
            var cmd = Parser.ParseArguments(args, CreateConverter(),
                CreateOptionsDictionary());
            Assert.IsTrue(cmd.PlainArguments.Count() == 2);
            Assert.AreEqual(cmd.PlainArguments.First(), "arg1");
            Assert.AreEqual(cmd.PlainArguments.Last(), "arg2");
        }

    }

}