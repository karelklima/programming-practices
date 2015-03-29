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
            Arguments arguments = new Arguments();
            // Program implicitne rozpozna kratkou a dlouhou volbu
            // AddOption vraci tridu OptionBuilder
            arguments.AddOption("v|verbose")
                // Description se uzivateli zobrazi v ramci zobrazeni napovedy
                .WithDescription("Verbose option description");

            arguments.AddOption("s|size")
                .WithDescription("Size option with default value of 42")
                // Povinny argument typu Integer, nazev SIZE se zobrazi v ramci zobrazeni napovedy
                // OptionBuilder.WithArgument<T> vraci tridu ArgumentBuilder<T>
                .WithArgument<int>("SIZE")
                // Pokud uzivatel volbu vubec nezada, bude mit tuto defaultni hodnotu
                .WithDefaultValue(42)
                // U zadane hodnoty se overi, zda splnuje nasledujici podminku
                .WithCondition(v => v > 0);

            arguments.AddOption("filenames")
                .WithDescription("Filenames to process")
                .WithArguments<string>("FILENAMES", 1, uint.MaxValue);

            // Program zpracuje argumenty ze vstupu - od teto chvile jiz nelze menit konfiguraci
            arguments.Parse(args);

            // IsOptionSet testuje, zda volba existuje v ramci argumentu
            Console.WriteLine("verbose = {0}", arguments.IsOptionSet("verbose"));
            // 
            Console.WriteLine("size = {0}", arguments.GetOptionValue<int>("size"));
            Console.WriteLine("arguments = {0}", arguments.GetPlainArguments()
                .Aggregate((i, j) => i + " " + j));

            arguments.AddOption("?|h|help|--h")
                     .WithDescription("Show help text")
                     .WithAction(() => Console.WriteLine(arguments.BuildHelpText()));

            // KONEC UKAZKOVEHO KODU

            arguments = new Arguments();

            string someStringValue;
            string someActionValue;
            int someIntValue;
            var someInts = new[] {1, 2, 3};

            arguments.AddOption("v")
                .WithAlias("verbose")
                .WithAlias("--v")
                .WithAlias("-verbose")
                .WithDescription("This is a description of this option")
                .WithAction(() => someActionValue = "Verbose option was detected")
                .WithOptionalArgument("some")
                .WithCondition( v => v.Contains("x") )
                .WithAction( v => someStringValue = v );

            arguments.AddOption("i|integer|--i")
                .WithDescription("This is a description of an integer option")
                .WithArgument<int>("INTEGER")
                .WithCondition(i => i > 0 && i <= 100)
                .WithEnumeratedValue(someInts)
                .WithEnumeratedValue(1, 2, 3)
                .WithAction(i => someIntValue = i);


            arguments.Parse(args);

            var x = arguments.GetOptionValue("test");
            var y = arguments.GetOptionValue<int>("test2");
        }
    }
}
