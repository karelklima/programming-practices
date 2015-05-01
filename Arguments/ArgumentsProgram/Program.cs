using System;
using ArgumentsLibrary;
using ArgumentsLibrary.Exceptions;

namespace ArgumentsProgram {

    internal class Program {

        private static void Main(string[] args) {
            var a = new Arguments();
            a.AddOption("x").WithArgument<int>("Test")
                .SetOptional();
            a.Parse(new string[] {"-x", "--xxx"});
            Console.WriteLine(string.Join(Environment.NewLine, a.BuildHelpText()));

            // UKAZKOVY KOD IMPLEMENTUJICI PRIKLAD ZE ZADANI
            var arguments = new Arguments();
            // Program implicitne rozpozna kratkou a dlouhou volbu
            // AddOption vraci tridu OptionBuilder
            arguments.AddOption("v|verbose")
                // Description se uzivateli zobrazi v ramci zobrazeni napovedy
                .WithDescription("Verbose option description");

            arguments.AddOption("s|size")
                .WithDescription("Size option with default value of 42")
                // Povinny argument typu Integer, nazev SIZE se zobrazi v ramci
                // zobrazeni napovedy
                // OptionBuilder.WithArgument<T> vraci tridu ArgumentBuilder<T>
                .WithArgument<int>("SIZE")
                // Pokud uzivatel volbu vubec nezada, bude mit tuto defaultni
                // hodnotu
                .WithDefaultValue(42)
                // U zadane hodnoty se overi, zda splnuje nasledujici podminku
                .WithCondition(v => v > 0);

            //arguments.AddOption("filenames")
            //    .WithDescription("Filenames to process")
            //    .WithArguments<string>("FILENAMES", 1, int.MaxValue);

            // Program zpracuje argumenty ze vstupu - od teto chvile jiz nelze
            // menit konfiguraci

            try {
                var commandLine =
                    arguments.Parse(new String[]
                    {"-v", "--size=42", "plain1", "plain2"});
                // IsOptionSet testuje, zda volba existuje v ramci argumentu
                Console.WriteLine("verbose = {0}",
                    commandLine.IsOptionSet("verbose"));

                Console.WriteLine("size = {0}",
                    commandLine.GetOptionValue<int>("size"));
                Console.WriteLine("arguments = {0}",
                    string.Join(" ", commandLine.GetPlainArguments()));
                Console.WriteLine(string.Join(Environment.NewLine,
                    arguments.BuildHelpText()));
            }
            catch (ParseException e) {
                Console.WriteLine(e.StackTrace);
            }


            // KONEC UKAZKOVEHO KODU
            //////////////////////////////////////////////////////////////////

            // DALSI PRIKLADY POUZITI

            arguments = new Arguments();

            // Nasledujici priklad ukazuje navaznost API trid
            arguments.AddOption("t") // OptionBuilder API
                .WithDescription("Popis") // stale OptionBuilder API
                .WithArgument<double>("CISLO") // ArgumentBuilder<double> API
                .WithCondition(d => d < 0.5);
                // stale ArgumentBuilder<double> API


            // Vypsani help textu
            arguments.AddOption("h|help")
                .WithDescription("Show help text")
                .WithAction(() =>
                    Console.WriteLine(arguments.BuildHelpText()));

            string someStringValue = null;
            string someActionValue = null;
            int someIntValue = 0;
            var someInts = new[] {1, 2, 3};

            arguments.AddOption("v") // -v
                .WithAlias("verbose") // --verbose
                .WithAlias("--v") // --v
                .WithAlias("-verbose") // -verbose
                .WithDescription("This is a description of this option")
                // Action triggered when the option is present in arguments
                .WithAction(() => someActionValue = "Verbose detected")
                // Optional argument of type string
                .WithOptionalArgument("some")
                // Argument must satisfy this condition
                .WithCondition(v => v.Contains("x"))
                // Action triggered when the option argument is detected
                .WithAction(v => someStringValue = v);

            // Mandatory option must be present among arguments
            // Also notice alias definition
            arguments.AddMandatoryOption("i|integer|--i") // -i, --integer, --i
                .WithDescription("This is a description of an integer option")
                // Mandatory argument of type int
                .WithArgument<int>("INTEGER")
                // Example condition of limiting the number
                .WithCondition(i => i > 0 && i <= 100)
                // Enumerated values constraint
                .WithEnumeratedValue(someInts)
                // This line does the same as the one above
                .WithEnumeratedValue(1, 2, 3)
                // Action triggered when the option argument is detected
                .WithAction(i => someIntValue = i);


            // Custom type processing
            //arguments.RegisterTypeConverter<long>(long.Parse);
            //arguments.AddOption("l|long-list")
            //    // This option takes exactly three arguments, i.e. -l 10 12 14
            //    .WithArguments<long>("LONGS", 3); 

            //arguments.RegisterTypeConverter<byte>(byte.Parse);
            //arguments.AddOption("b|byte-list")
            // This option takes from one to ten arguments
            //   .WithArguments<byte>("BYTES", 1, 10);

            arguments.Parse(new String[] {"--v=text", "-i", "1"});
            Console.WriteLine(string.Join(Environment.NewLine,
                arguments.BuildHelpText()));

            Console.WriteLine("{0} = {1}", "someStringValue", someStringValue);
            Console.WriteLine("{0} = {1}", "someActionValue", someActionValue);
            Console.WriteLine("{0} = {1}", "someIntValue", someIntValue);

            //var x = arguments.GetOptionValue("v");
            //var y = arguments.GetOptionValue<int>("--i");
            //var longList = arguments.GetOptionValues<long>("long-list");
            //var bytesList = arguments.GetOptionValues<byte>("b");
        }

    }

}