using System;
using System.Linq;
using ArgumentsLibrary;

namespace ArgumentsProgram
{
    class Program
    {
        static void Main(string[] args)
        {

            // UKAZKOVY KOD IMPLEMENTUJICI PRIKLAD ZE ZADANI

            Arguments.AddOption("v|verbose")
                .WithDescription("Verbose option description");
            Arguments.AddOption("s|size")
                .WithDescription("Size option with default value of 42")
                .WithArgument<int>("SIZE")
                .WithDefaultValue(42)
                .WithCondition(v => v > 0);

            Arguments.Parse(args);

            Console.WriteLine("verbose = {0}", Arguments.IsOptionSet("verbose"));
            Console.WriteLine("size = {0}", Arguments.GetOptionValue<int>("size"));
            Console.WriteLine("arguments = {0}", Arguments.GetPlainArguments()
                .Aggregate((i, j) => i + " " + j));

            // KONEC UKAZKOVEHO KODU

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
                .WithCondition( v => v.Contains("x") )
                .WithAction( v => someStringValue = v );

            Arguments.AddOption("i|integer|--i")
                .WithDescription("This is a description of an integer option")
                .WithArgument<int>("INTEGER")
                .WithCondition(i => i > 0 && i <= 100)
                .WithEnumeratedValue(someInts)
                .WithEnumeratedValue(1, 2, 3)
                .WithAction(i => someIntValue = i);

            Arguments.Parse(args);

            var x = Arguments.GetOptionValue("test");
            var y = Arguments.GetOptionValue<int>("test2");
        }
    }
}
