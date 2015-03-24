using System;
using ArgumentsLibrary;

namespace ArgumentsProgram
{
    class Program
    {
        static void Main(string[] args)
        {

            string someStringValue;
            string someActionValue;
            int someIntValue;
            var someInts = new[] {1, 2, 3};

            Arguments.AddOption("v")
                .WithAlias("verbose")
                .WithAlias("--v")
                .WithAlias("-verbose")
                .WithDescription("This is a description of this option")
                .WithAction(() => someActionValue = "Verbose option was detected")
                .WithOptionalArgument("some")
                .WithPredicate( v => v.Contains("x") )
                .WithAction( v => someStringValue = v );

            Arguments.AddOption("i|integer|--i")
                .WithDescription("This is a description of an integer option")
                .WithArgument<int>("INTEGER")
                .WithPredicate(i => i > 0 && i <= 100)
                .WithEnumeratedValue(someInts)
                .WithEnumeratedValue(1, 2, 3)
                .WithAction(i => someIntValue = i);

            Arguments.Parse(args);

            var x = Arguments.GetOptionValue("test");
            var y = Arguments.GetOptionValue<int>("test2");
        }
    }
}
