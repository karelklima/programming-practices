using System;
using System.Collections.Generic;
using System.Linq;
using ArgumentsLibrary;
using ArgumentsLibrary.Exceptions;

#if MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Category = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
#else
using NUnit.Framework;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestContext = System.Object;
using TestProperty = NUnit.Framework.PropertyAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
#endif

namespace ArgumentsTest {

    [TestClass]
    public class ArgumentsTest {

        [TestMethod]
        public void RegisterTypeConverter_CustomType() {
            var arguments = new Arguments();
            arguments.RegisterTypeConverter(long.Parse);
        }

        [TestMethod]
        [ExpectedException(typeof (SetupException))]
        public void RegisterTypeConverter_Null() {
            var arguments = new Arguments();
            arguments.RegisterTypeConverter<string>(null);
        }

        [TestMethod]
        public void AddOption_Aliases() {
            var arguments = new Arguments();
            var builder = arguments.AddOption("a|aaa");
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        [ExpectedException(typeof (SetupException))]
        public void AddOption_InvalidAlias() {
            var arguments = new Arguments();
            var builder = arguments.AddOption("a+");
        }

        [TestMethod]
        [ExpectedException(typeof (SetupException))]
        public void AddOption_Null() {
            var arguments = new Arguments();
            var builder = arguments.AddOption(null);
        }

        [TestMethod]
        public void AddMandatoryOption_Aliases() {
            var arguments = new Arguments();
            var builder = arguments.AddMandatoryOption("a|aaa");
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        [ExpectedException(typeof (SetupException))]
        public void AddMandatoryOption_InvalidAlias() {
            var arguments = new Arguments();
            var builder = arguments.AddMandatoryOption("a+");
        }

        [TestMethod]
        [ExpectedException(typeof (SetupException))]
        public void AddMandatoryOption_Null() {
            var arguments = new Arguments();
            var builder = arguments.AddMandatoryOption(null);
        }

        [TestMethod]
        public void Parse_PlainArguments() {
            var arguments = new Arguments();
            arguments.Parse(new [] {"arg1", "arg2", "arg3"});
        }

        [TestMethod]
        public void Parse_Option() {
            var arguments = new Arguments();
            arguments.AddOption("t|test");
            arguments.Parse(new [] {"-t"});
        }

        [TestMethod]
        [ExpectedException(typeof (ParseException))]
        public void Parse_Null() {
            var arguments = new Arguments();
            arguments.Parse(null);
        }

    }

}