using System;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using ArgumentsLibrary;
using ArgumentsLibrary.Builders;
using ArgumentsLibrary.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArgumentsTest.Builders
{
    [TestClass]
    public class OptionBuilderTest
    {
        [TestMethod]
        public void Constructor_Arguments()
        {
            var arguments = new Arguments();
            var optionBuilder = new OptionBuilder(arguments);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Null()
        {
            var optionBuilder = new OptionBuilder(null);
        }

        [TestMethod]
        public void SetMandatory_True()
        {
            var optionBuilder = new OptionBuilder(new Arguments());
            optionBuilder.SetMandatory(true);
            Assert.IsTrue(optionBuilder.Option.Mandatory);
        }

        [TestMethod]
        public void SetMandatory_False()
        {
            var optionBuilder = new OptionBuilder(new Arguments());
            optionBuilder.SetMandatory(false);
            Assert.IsFalse(optionBuilder.Option.Mandatory);
        }

        [TestMethod]
        public void WithAliases_CorrectFormat()
        {
            var optionBuilder = new OptionBuilder(new Arguments());
            optionBuilder.WithAliases("a|aaaaa");

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentsSetupException))]
        public void WithAliases_IncorrectFormat()
        {
            var optionBuilder = new OptionBuilder(new Arguments());
            optionBuilder.WithAliases("1|11111");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentsSetupException))]
        public void WithAliases_Null()
        {
            var optionBuilder = new OptionBuilder(new Arguments());
            optionBuilder.WithAlias(null);
        }

        [TestMethod]
        public void WithAlias_CorrectFormat()
        {
            var optionBuilder = new OptionBuilder(new Arguments());
            optionBuilder.WithAlias("a|aaaaa");

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentsSetupException))]
        public void WithAlias_IncorrectFormat()
        {
            var optionBuilder = new OptionBuilder(new Arguments());
            optionBuilder.WithAlias("1|11111");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentsSetupException))]
        public void WithAlias_Null()
        {
            var optionBuilder = new OptionBuilder(new Arguments());
            optionBuilder.WithAliases(null);
        }

        [TestMethod]
        public void WithDescription_Text()
        {
            var optionBuilder = new OptionBuilder(new Arguments());
            optionBuilder.WithDescription("description");
            Assert.Equals(optionBuilder.Option.Description, "description");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentsSetupException))]
        public void WithDescription_Null()
        {
            var optionBuilder = new OptionBuilder(new Arguments());
            optionBuilder.WithDescription(null);
        }

        [TestMethod]
        public void WithAction_Action()
        {
            var optionBuilder = new OptionBuilder(new Arguments());
            var actionsCount = optionBuilder.Option.Actions.Count;
            optionBuilder.WithAction(() => { });
            Assert.AreEqual(optionBuilder.Option.Actions.Count, actionsCount + 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentsSetupException))]
        public void WithAction_Null()
        {
            var optionBuilder = new OptionBuilder(new Arguments());
            optionBuilder.WithAction(null);
        }



    }
}
