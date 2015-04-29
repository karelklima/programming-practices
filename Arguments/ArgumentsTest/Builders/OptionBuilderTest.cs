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
        public void Constructor_RegisterAliasesAction()
        {
            var registered = false;
            // registering aliases action mock
            var optionBuilder = new OptionBuilder((o, a) =>
            {
                if (o != null && a.Equals("a"))
                {
                    registered = true;
                }
            });
            optionBuilder.WithAliases("a");
            Assert.IsTrue(registered);
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
            var optionBuilder = new OptionBuilder((o, a) => { });
            optionBuilder.SetMandatory(true);
            Assert.IsTrue(optionBuilder.Option.Mandatory);
        }

        [TestMethod]
        public void SetMandatory_False()
        {
            var optionBuilder = new OptionBuilder((o, a) => { });
            optionBuilder.SetMandatory(false);
            Assert.IsFalse(optionBuilder.Option.Mandatory);
        }

        [TestMethod]
        public void WithAliases_ValidFormat()
        {
            var optionBuilder = new OptionBuilder((o, a) => { });
            optionBuilder.WithAliases("a|aaaaa");

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentsSetupException))]
        public void WithAliases_InvalidFormat()
        {
            // registering aliases action mock that checks the alias format
            var optionBuilder = new OptionBuilder((o, a) =>
            {
                if (a.Equals("1|11111"))
                    throw new ArgumentsSetupException("Invalid alias format");
            });
            optionBuilder.WithAliases("1|11111");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentsSetupException))]
        public void WithAliases_Null()
        {
            // registering aliases action mock that checks if alias is null
            var optionBuilder = new OptionBuilder((o, a) =>
            {
                if (a == null)
                    throw new ArgumentsSetupException("Alias cannot be null");
            });
            optionBuilder.WithAlias(null);
        }

        [TestMethod]
        public void WithAlias_ValidFormat()
        {
            var optionBuilder = new OptionBuilder((o, a) => { });
            optionBuilder.WithAlias("a|aaaaa");

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentsSetupException))]
        public void WithAlias_InvalidFormat()
        {
            // registering aliases action mock that checks the alias format
            var optionBuilder = new OptionBuilder((o, a) =>
            {
                if (a.Equals("1|11111"))
                    throw new ArgumentsSetupException("Invalid alias format");
            });
            optionBuilder.WithAlias("1|11111");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentsSetupException))]
        public void WithAlias_Null()
        {
            // registering aliases action mock that checks if alias is null
            var optionBuilder = new OptionBuilder((o, a) =>
            {
                if (a == null)
                    throw new ArgumentsSetupException("Alias cannot be null");
            });
            optionBuilder.WithAliases(null);
        }

        [TestMethod]
        public void WithDescription_Text()
        {
            var optionBuilder = new OptionBuilder((o, a) => { });
            optionBuilder.WithDescription("description");
            Assert.AreEqual(optionBuilder.Option.Description, "description");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentsSetupException))]
        public void WithDescription_Null()
        {
            var optionBuilder = new OptionBuilder((o, a) => { });
            optionBuilder.WithDescription(null);
        }

        [TestMethod]
        public void WithAction_Action()
        {
            var optionBuilder = new OptionBuilder((o, a) => { });
            var actionsCount = optionBuilder.Option.Actions.Count;
            optionBuilder.WithAction(() => { });
            Assert.AreEqual(optionBuilder.Option.Actions.Count, actionsCount + 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentsSetupException))]
        public void WithAction_Null()
        {
            var optionBuilder = new OptionBuilder((o, a) => { });
            optionBuilder.WithAction(null);
        }



    }
}
