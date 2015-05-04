using System;
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
        public void ArgumentsComplete() {
            var arguments = new Arguments();

            var verbose = false;
            var size = 0;

            arguments.AddOption( "v|verbose" )
                .WithDescription( "Verbose option description" )
                .WithAction( () => verbose = true );

            arguments.AddOption( "s|size" )
                .WithDescription( "Size option with default value of 42" )
                .WithArgument<int>( "SIZE" )
                .WithDefaultValue( 42 )
                .WithCondition( v => v > 0 )
                .WithAction( s => size = s );

            string[] args = {"-v", "--size=42", "plain1", "plain2"};

            var cmd = arguments.Parse( args );

            Assert.IsTrue( verbose );
            Assert.IsTrue( cmd.IsOptionSet( "v" ) );
            Assert.IsTrue( size == 42 );
            Assert.IsTrue( cmd.GetOptionValue<int>( "--size" ) == 42 );
            Assert.AreEqual( cmd.GetPlainArguments().First(), "plain1" );
            Assert.AreEqual( cmd.GetPlainArguments().Last(), "plain2" );
        }

        [TestMethod]
        public void RegisterTypeConverter_CustomType() {
            var arguments = new Arguments();
            arguments.RegisterTypeConverter( long.Parse );
        }

        [TestMethod]
        [ExpectedException( typeof ( SetupException ) )]
        public void RegisterTypeConverter_Null() {
            var arguments = new Arguments();
            arguments.RegisterTypeConverter<string>( null );
        }

        [TestMethod]
        public void AddOption_Aliases() {
            var arguments = new Arguments();
            var builder = arguments.AddOption( "a|aaa" );
            Assert.IsNotNull( builder );
        }

        [TestMethod]
        [ExpectedException( typeof ( SetupException ) )]
        public void AddOption_InvalidAlias() {
            var arguments = new Arguments();
            var builder = arguments.AddOption( "a+" );
        }

        [TestMethod]
        [ExpectedException( typeof ( SetupException ) )]
        public void AddOption_Null() {
            var arguments = new Arguments();
            var builder = arguments.AddOption( null );
        }

        [TestMethod]
        public void AddMandatoryOption_Aliases() {
            var arguments = new Arguments();
            var builder = arguments.AddMandatoryOption( "a|aaa" );
            Assert.IsNotNull( builder );
        }

        [TestMethod]
        [ExpectedException( typeof ( SetupException ) )]
        public void AddMandatoryOption_InvalidAlias() {
            var arguments = new Arguments();
            var builder = arguments.AddMandatoryOption( "a+" );
        }

        [TestMethod]
        [ExpectedException( typeof ( SetupException ) )]
        public void AddMandatoryOption_Null() {
            var arguments = new Arguments();
            var builder = arguments.AddMandatoryOption( null );
        }

        [TestMethod]
        public void Parse_PlainArguments() {
            var arguments = new Arguments();
            arguments.Parse( new[] {"arg1", "arg2", "arg3"} );
        }

        [TestMethod]
        public void Parse_Option() {
            var arguments = new Arguments();
            arguments.AddOption( "t|test" );
            arguments.Parse( new[] {"-t"} );
        }

        [TestMethod]
        [ExpectedException( typeof ( ParseException ) )]
        public void Parse_Null() {
            var arguments = new Arguments();
            arguments.Parse( null );
        }

        [TestMethod]
        public void BuildHelpText_OptionsDefinitions() {
            var arguments = new Arguments();
            // Option with extreme amount of aliases
            arguments
                .AddOption( "o|option" )
                .WithAliases( "t|test" )
                .WithDescription( "This is a description\nThis is another line" )
                .WithOptionalArgument( "ARG" );
            // Option with extreme description
            arguments
                .AddOption( "v|verbose" )
                .WithDescription( String.Empty.PadRight( 500, 'v' ) );
            var help = arguments.BuildHelpText();
            Assert.IsTrue( help.Contains( "Options:" ) );
        }

        [TestMethod]
        public void BuildHelpText_ValidUsage() {
            var arguments = new Arguments();
            var help = arguments.BuildHelpText( "myapp [options]" );
            Assert.IsTrue( help.StartsWith( "Usage: myapp [options]" ) );
        }

        [TestMethod]
        public void BuildHelpText_Empty() {
            var arguments = new Arguments();
            var help = arguments.BuildHelpText();
            Assert.IsTrue( help.Length > 0 );
        }

    }

}