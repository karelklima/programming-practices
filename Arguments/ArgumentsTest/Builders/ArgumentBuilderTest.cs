using System;
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

namespace ArgumentsTest.Builders {

    [TestClass]
    public class ArgumentBuilderTest {

        [TestMethod]
        public void CompleteArgumentBuilder() {
            var builder = new ArgumentBuilder<string>( ( a, t ) => { } );
            builder
                .WithAction( s => { } )
                .WithCondition( s => s.Length > 0 )
                .WithEnumeratedValue( "one", "two", "three" )
                .SetOptional( true );

            Assert.AreEqual( builder.Argument.Actions.Count, 1 );
            Assert.AreEqual( builder.Argument.Conditions.Count, 2 );
            Assert.AreEqual( builder.Argument.Optional, true );
        }

        [TestMethod]
        public void Constructor_RegisterArgumentAction() {
            var registered = false;
            var builder = new ArgumentBuilder<string>( ( argument, type ) => {
                // TODO check argument and type
                registered = true;
            } );
            Assert.IsTrue( registered );
        }

        [TestMethod]
        [ExpectedException( typeof ( ArgumentNullException ) )]
        public void Constructor_Null() {
            var builder = new ArgumentBuilder<string>( null );
        }

        [TestMethod]
        public void SetName_Text() {
            var builder = new ArgumentBuilder<string>( ( a, t ) => { } );
            builder.SetName( "argument" );
            Assert.AreEqual( builder.Argument.Name, "argument" );
        }

        [TestMethod]
        [ExpectedException( typeof ( SetupException ) )]
        public void SetName_Null() {
            var builder = new ArgumentBuilder<string>( ( a, t ) => { } );
            builder.SetName( null );
        }

        [TestMethod]
        public void SetOptional_True() {
            var builder = new ArgumentBuilder<string>( ( a, t ) => { } );
            builder.SetOptional( true );
            Assert.AreEqual( builder.Argument.Optional, true );
        }

        [TestMethod]
        public void SetOptional_False() {
            var builder = new ArgumentBuilder<string>( ( a, t ) => { } );
            builder.SetOptional( false );
            Assert.AreEqual( builder.Argument.Optional, false );
        }

        [TestMethod]
        public void WithDefaultValue_ValidString() {
            var builder = new ArgumentBuilder<string>( ( a, t ) => { } );
            builder.WithDefaultValue( "text" );
            Assert.AreEqual( builder.Argument.DefaultValue, "text" );
            Assert.IsTrue( builder.Argument.DefaultValueIsSet );
        }

        [TestMethod]
        [ExpectedException( typeof ( SetupException ) )]
        public void WithDefaultValue_Null() {
            var builder = new ArgumentBuilder<string>( ( a, t ) => { } );
            builder.WithDefaultValue( null );
        }

        [TestMethod]
        public void WithCondition_Condition() {
            var builder = new ArgumentBuilder<string>( ( a, t ) => { } );
            var conditionCount = builder.Argument.Conditions.Count;
            builder.WithCondition( s => true );
            Assert.AreEqual( builder.Argument.Conditions.Count,
                conditionCount + 1 );
        }

        [TestMethod]
        [ExpectedException( typeof ( SetupException ) )]
        public void WithCondition_Null() {
            var builder = new ArgumentBuilder<string>( ( a, t ) => { } );
            builder.WithCondition( null );
        }

        [TestMethod]
        public void WithEnumeratedValue_EnumeratedValue() {
            var builder = new ArgumentBuilder<string>( ( a, t ) => { } );
            var conditionCount = builder.Argument.Conditions.Count;
            builder.WithEnumeratedValue( "value1", "value2" );
            Assert.AreEqual( builder.Argument.Conditions.Count,
                conditionCount + 1 );
        }

        [TestMethod]
        [ExpectedException( typeof ( SetupException ) )]
        public void WithEnumeratedValue_Null() {
            var builder = new ArgumentBuilder<string>( ( a, t ) => { } );
            builder.WithEnumeratedValue( null );
        }

        [TestMethod]
        public void WithAction_Action() {
            var builder = new ArgumentBuilder<string>( ( a, t ) => { } );
            var conditionCount = builder.Argument.Actions.Count;
            string detectedValue = null;
            builder.WithAction( s => { detectedValue = s; } );
            builder.Argument.InvokeActions( "test" );
            Assert.AreEqual( builder.Argument.Actions.Count, conditionCount + 1 );
            Assert.AreEqual( detectedValue, "test" );
        }

        [TestMethod]
        [ExpectedException( typeof ( SetupException ) )]
        public void WithAction_Null() {
            var builder = new ArgumentBuilder<string>( ( a, t ) => { } );
            builder.WithAction( null );
        }

    }

}