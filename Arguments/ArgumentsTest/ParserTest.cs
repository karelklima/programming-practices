using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using ArgumentsLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArgumentsTest {

    /// <summary>
    /// Summary description for ParserTest
    /// </summary>
    [TestClass]
    public class ParserTest {

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
        [ExpectedException(typeof(ArgumentNullException))]
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

    }

}