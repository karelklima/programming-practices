using System;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using ArgumentsLibrary;
using ArgumentsLibrary.Builders;
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
        public void RegisterArgument_ValidInstanceAndType()
        {
            var optionBuilder = new OptionBuilder((o, a) => { });
            var argument = new Argument<string>();
            optionBuilder.RegisterArgument(argument, typeof(string));
            Assert.IsTrue(optionBuilder.Option.Argument.Equals(argument));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterArgument_NullInstance()
        {
            var optionBuilder = new OptionBuilder((o, a) => { });
            optionBuilder.RegisterArgument(null, typeof(string));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterArgument_InvalidInstanceAndType()
        {
            var optionBuilder = new OptionBuilder((o, a) => { });
            var argument = new Argument<int>();
            optionBuilder.RegisterArgument(argument, typeof(string));
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
        [ExpectedException(typeof(SetupException))]
        public void WithAliases_InvalidFormat()
        {
            // registering aliases action mock that checks the alias format
            var optionBuilder = new OptionBuilder((o, a) =>
            {
                if (a.Equals("1|11111"))
                    throw new SetupException("Invalid alias format");
            });
            optionBuilder.WithAliases("1|11111");
        }

        [TestMethod]
        [ExpectedException(typeof(SetupException))]
        public void WithAliases_Null()
        {
            // registering aliases action mock that checks if alias is null
            var optionBuilder = new OptionBuilder((o, a) =>
            {
                if (a == null)
                    throw new SetupException("Alias cannot be null");
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
        [ExpectedException(typeof(SetupException))]
        public void WithAlias_InvalidFormat()
        {
            // registering aliases action mock that checks the alias format
            var optionBuilder = new OptionBuilder((o, a) =>
            {
                if (a.Equals("1|11111"))
                    throw new SetupException("Invalid alias format");
            });
            optionBuilder.WithAlias("1|11111");
        }

        [TestMethod]
        [ExpectedException(typeof(SetupException))]
        public void WithAlias_Null()
        {
            // registering aliases action mock that checks if alias is null
            var optionBuilder = new OptionBuilder((o, a) =>
            {
                if (a == null)
                    throw new SetupException("Alias cannot be null");
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
        [ExpectedException(typeof(SetupException))]
        public void WithDescription_Null()
        {
            var optionBuilder = new OptionBuilder((o, a) => { });
            optionBuilder.WithDescription(null);
        }

        [TestMethod]
        public void WithAction_Action() {
            var optionBuilder = new OptionBuilder((o, a) => { });
            var actionsCount = optionBuilder.Option.Actions.Count;
            var detected = false;
            optionBuilder.WithAction(() => { detected = true; });
            optionBuilder.Option.InvokeActions();
            Assert.AreEqual(optionBuilder.Option.Actions.Count, actionsCount + 1);
            Assert.IsTrue(detected);
        }

        [TestMethod]
        [ExpectedException(typeof(SetupException))]
        public void WithAction_Null()
        {
            var optionBuilder = new OptionBuilder((o, a) => { });
            optionBuilder.WithAction(null);
        }

        [TestMethod]
        public void WithArgument_Int_ValidName()
        {
            var optionBuilder = new OptionBuilder((o, a) => { });
            var argumentBuilder = optionBuilder.WithArgument<int>("number");
            Assert.IsTrue(optionBuilder.Option.Argument != null);
            Assert.IsTrue(optionBuilder.Option.Argument is Argument<int>);
        }

        [TestMethod]
        [ExpectedException(typeof(SetupException))]
        public void WithArgument_Int_InvalidName()
        {
            var optionBuilder = new OptionBuilder((o, a) => { });
            var argumentBuilder = optionBuilder.WithArgument<int>(null);
        }

        [TestMethod]
        public void WithArgument_DefaultType_ValidName()
        {
            var optionBuilder = new OptionBuilder((o, a) => { });
            var argumentBuilder = optionBuilder.WithArgument("text");
            Assert.IsTrue(optionBuilder.Option.Argument != null);
            Assert.IsTrue(optionBuilder.Option.Argument is Argument<string>);
        }

        [TestMethod]
        public void WithOptionalArgument_Int_ValidName()
        {
            var optionBuilder = new OptionBuilder((o, a) => { });
            var argumentBuilder = optionBuilder.WithOptionalArgument<int>("number");
            Assert.IsTrue(optionBuilder.Option.Argument != null);
            Assert.IsTrue(optionBuilder.Option.Argument.Optional);
            Assert.IsTrue(optionBuilder.Option.Argument is Argument<int>);
        }

        [TestMethod]
        [ExpectedException(typeof(SetupException))]
        public void WithOptionalArgument_Int_InvalidName()
        {
            var optionBuilder = new OptionBuilder((o, a) => { });
            var argumentBuilder = optionBuilder.WithOptionalArgument<int>(null);
        }

        [TestMethod]
        public void WithOptionalArgument_DefaultType_ValidName()
        {
            var optionBuilder = new OptionBuilder((o, a) => { });
            var argumentBuilder = optionBuilder.WithOptionalArgument("text");
            Assert.IsTrue(optionBuilder.Option.Argument != null);
            Assert.IsTrue(optionBuilder.Option.Argument.Optional);
            Assert.IsTrue(optionBuilder.Option.Argument is Argument<string>);
        }



    }
}
