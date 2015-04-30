using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using ArgumentsLibrary;
using ArgumentsLibrary.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArgumentsTest
{
    [TestClass]
    public class CommandLineTest
    {
        [TestMethod]
        public void Constructor_Ok()
        {
            var commandLine = new CommandLine(new Converter());
        }

        [TestMethod]
        public void IsOptionSet_ValidAlias()
        {
            var commandLine = new CommandLine(new Converter());
            commandLine.Options.Add(new OptionAlias("t", OptionType.Short), null);
            Assert.IsTrue(commandLine.IsOptionSet("-t"));
            Assert.IsTrue(commandLine.IsOptionSet("t"));
            Assert.IsFalse(commandLine.IsOptionSet("-x"));
        }

        [TestMethod]
        [ExpectedException(typeof(CommandLineException))]
        public void IsOptionSet_InvalidAlias()
        {
            var commandLine = new CommandLine(new Converter());
            commandLine.IsOptionSet("t*");
        }

        [TestMethod]
        [ExpectedException(typeof(CommandLineException))]
        public void IsOptionSet_Null()
        {
            var commandLine = new CommandLine(new Converter());
            commandLine.IsOptionSet(null);
        }

        [TestMethod]
        public void GetOptionValue_Int_Ok()
        {
            var converter = new Converter();
            converter.RegisterTypeConverter<int>(int.Parse);
            var commandLine = new CommandLine(converter);
            commandLine.Options.Add(new OptionAlias("n", OptionType.Short), "10");
            Assert.AreEqual(commandLine.GetOptionValue<int>("n"), 10);
        }

        [TestMethod]
        [ExpectedException(typeof(CommandLineException))]
        public void GetOptionValue_Int_NotSet()
        {
            var converter = new Converter();
            converter.RegisterTypeConverter<int>(int.Parse);
            var commandLine = new CommandLine(converter);
            commandLine.GetOptionValue<int>("n");
        }

        [TestMethod]
        [ExpectedException(typeof(CommandLineException))]
        public void GetOptionValue_Int_InvalidAlias()
        {
            var converter = new Converter();
            converter.RegisterTypeConverter<int>(int.Parse);
            var commandLine = new CommandLine(converter);
            commandLine.Options.Add(new OptionAlias("n", OptionType.Short), "10");
            commandLine.GetOptionValue<int>("n*");
        }

        [TestMethod]
        [ExpectedException(typeof(CommandLineException))]
        public void GetOptionValue_Int_MissingConverter()
        {
            var converter = new Converter();
            var commandLine = new CommandLine(converter);
            commandLine.Options.Add(new OptionAlias("n", OptionType.Short), "10");
            commandLine.GetOptionValue<int>("n");
        }

        [TestMethod]
        public void GetOptionValue_DefaultType_Ok()
        {
            var converter = new Converter();
            converter.RegisterTypeConverter<string>(string.Copy);
            var commandLine = new CommandLine(converter);
            commandLine.Options.Add(new OptionAlias("t", OptionType.Short), "text");
            Assert.AreEqual(commandLine.GetOptionValue("t"), "text");
        }

        [TestMethod]
        [ExpectedException(typeof(CommandLineException))]
        public void GetOptionValue_DefaultType_NotSet()
        {
            var converter = new Converter();
            converter.RegisterTypeConverter<string>(string.Copy);
            var commandLine = new CommandLine(converter);
            commandLine.GetOptionValue("t");
        }

        [TestMethod]
        [ExpectedException(typeof(CommandLineException))]
        public void GetOptionValue_DefaultType_InvalidAlias()
        {
            var converter = new Converter();
            converter.RegisterTypeConverter<string>(string.Copy);
            var commandLine = new CommandLine(converter);
            commandLine.Options.Add(new OptionAlias("t", OptionType.Short), "text");
            commandLine.GetOptionValue("n*");
        }

        [TestMethod]
        public void GetPlainArguments_Int_ValidInput()
        {
            var converter = new Converter();
            converter.RegisterTypeConverter<int>(int.Parse);
            var commandLine = new CommandLine(converter);
            commandLine.PlainArguments.Add("10");
            commandLine.PlainArguments.Add("-10");
            var inputNumbers = commandLine.GetPlainArguments<int>();
            var testNumbers = new List<int>(new int[] {10, -10});
            Assert.IsTrue(inputNumbers.All(testNumbers.Contains));
            Assert.IsTrue(inputNumbers.Count() == testNumbers.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(CommandLineException))]
        public void GetPlainArguments_Int_InvalidInput()
        {
            var converter = new Converter();
            converter.RegisterTypeConverter<int>(int.Parse);
            var commandLine = new CommandLine(converter);
            commandLine.PlainArguments.Add("10");
            commandLine.PlainArguments.Add("x10");
            var inputNumbers = commandLine.GetPlainArguments<int>();
        }

        [TestMethod]
        public void GetPlainArguments_DefaultType_ValidInput()
        {
            var converter = new Converter();
            converter.RegisterTypeConverter<string>(string.Copy);
            var commandLine = new CommandLine(converter);
            commandLine.PlainArguments.Add("abc");
            commandLine.PlainArguments.Add("def");
            var inputArgs = commandLine.GetPlainArguments();
            var testArgs = new List<string>(new string[] { "abc", "def" });
            Assert.IsTrue(inputArgs.All(testArgs.Contains));
            Assert.IsTrue(inputArgs.Count() == testArgs.Count());
        }
    }
}
