using System;
using System.Runtime.Remoting.Messaging;
using ArgumentsLibrary;
using ArgumentsLibrary.Builders;
using ArgumentsLibrary.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArgumentsTest.Builders
{
    [TestClass]
    public class ArgumentBuilderTest
    {
        [TestMethod]
        public void CompleteArgumentBuilder()
        {
            var builder = new ArgumentBuilder<string>(a => { });
            builder
                .WithAction(s => { })
                .WithCondition(s => s.Length > 0)
                .WithEnumeratedValue("one", "two", "three")
                .SetOptional(true);

            Assert.AreEqual(builder.Argument.Actions.Count, 1);
            Assert.AreEqual(builder.Argument.Conditions.Count, 2);
            Assert.AreEqual(builder.Argument.Optional, true);
        }

        [TestMethod]
        public void Constructor_RegisterArgumentAction()
        {
            var registered = false;
            var builder = new ArgumentBuilder<string>(argument =>
            {
                if (argument.GetType() == typeof (Argument<string>))
                {
                    registered = true;
                }
            });
            Assert.IsTrue(registered);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Null()
        {
            var builder = new ArgumentBuilder<string>(null);
        }

        [TestMethod]
        public void SetName_Text()
        {
            var builder = new ArgumentBuilder<string>(a => { });
            builder.SetName("argument");
            Assert.AreEqual(builder.Argument.Name, "argument");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentsSetupException))]
        public void SetName_Null()
        {
            var builder = new ArgumentBuilder<string>(a => { });
            builder.SetName(null);
        }

        [TestMethod]
        public void SetOptional_True()
        {
            var builder = new ArgumentBuilder<string>(a => { });
            builder.SetOptional(true);
            Assert.AreEqual(builder.Argument.Optional, true);
        }

        [TestMethod]
        public void SetOptional_False()
        {
            var builder = new ArgumentBuilder<string>(a => { });
            builder.SetOptional(false);
            Assert.AreEqual(builder.Argument.Optional, false);
        }

        [TestMethod]
        public void WithCondition_Condition()
        {
            var builder = new ArgumentBuilder<string>(a => { });
            var conditionCount = builder.Argument.Conditions.Count;
            builder.WithCondition(s => true);
            Assert.AreEqual(builder.Argument.Conditions.Count, conditionCount + 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentsSetupException))]
        public void WithCondition_Null()
        {
            var builder = new ArgumentBuilder<string>(a => { });
            builder.WithCondition(null);
        }

        [TestMethod]
        public void WithEnumeratedValue_EnumeratedValue()
        {
            var builder = new ArgumentBuilder<string>(a => { });
            var conditionCount = builder.Argument.Conditions.Count;
            builder.WithEnumeratedValue("value1", "value2");
            Assert.AreEqual(builder.Argument.Conditions.Count, conditionCount + 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentsSetupException))]
        public void WithEnumeratedValue_Null()
        {
            var builder = new ArgumentBuilder<string>(a => { });
            builder.WithEnumeratedValue(null);
        }

        [TestMethod]
        public void WithAction_Action()
        {
            var builder = new ArgumentBuilder<string>(a => { });
            var conditionCount = builder.Argument.Actions.Count;
            builder.WithAction(s => { });
            Assert.AreEqual(builder.Argument.Actions.Count, conditionCount + 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentsSetupException))]
        public void WithAction_Null()
        {
            var builder = new ArgumentBuilder<string>(a => { });
            builder.WithAction(null);
        }

        
    }
}
