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

            // Program implicitne rozpozna kratkou a dlouhou volbu
            // AddOption vraci tridu OptionBuilder
            Arguments.AddOption("v|verbose")
                // Description se uzivateli zobrazi v ramci zobrazeni napovedy
                .WithDescription("Verbose option description");

            Arguments.AddOption("s|size")
                .WithDescription("Size option with default value of 42")
                // Povinny argument typu Integer, nazev SIZE se zobrazi v ramci zobrazeni napovedy
                // OptionBuilder.WithArgument<T> vraci tridu ArgumentBuilder<T>
                .WithArgument<int>("SIZE")
                // Pokud uzivatel volbu vubec nezada, bude mit tuto defaultni hodnotu
                .WithDefaultValue(42)
                // U zadane hodnoty se overi, zda splnuje nasledujici podminku
                .WithCondition(v => v > 0);

            Arguments.AddOption("filenames")
                .WithDescription("Filenames to process")
                .WithArguments<string>("FILENAMES", 1, uint.MaxValue);

            // Program zpracuje argumenty ze vstupu - od teto chvile jiz nelze menit konfiguraci
            Arguments.Parse(args);

            // IsOptionSet testuje, zda volba existuje v ramci argumentu
            Console.WriteLine("verbose = {0}", Arguments.IsOptionSet("verbose"));
            // 
            Console.WriteLine("size = {0}", Arguments.GetOptionValue<int>("size"));
            Console.WriteLine("arguments = {0}", Arguments.GetPlainArguments()
                .Aggregate((i, j) => i + " " + j));

            Arguments.AddOption("?|h|help|--h")
                     .WithDescription("Show help text")
                     .WithAction(() => Console.WriteLine(Arguments.BuildHelpText()));

            // KONEC UKAZKOVEHO KODU

            Arguments.Reset();

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

            Arguments.AddOption("?|h|help|--h")
                     .WithDescription("Show help text")
                     .WithAction(() => Console.WriteLine(Arguments.BuildHelpText(
                         option => "Option: "+option.ToString()
                         )));


            Arguments.Parse(args);

            var x = Arguments.GetOptionValue("test");
            var y = Arguments.GetOptionValue<int>("test2");
        }
    }
}
