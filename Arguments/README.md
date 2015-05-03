README.md
============

Requirements
============
- .NET

Compilation
============

### Windows ###
Open Visual Studio and Build it

### Mono based ###
Project contains Makefile.

- Build: **make all**
- Build lib: **make lib**
- Build debug lib: **make debug**
- Generate docs: **make doxygen**
- Test: **make test**
- Clean: **make clean**

How to Use
============

### Simple example ###

~~~
var a = new Arguments();
a.AddOption("xxx");
a.Parse(new string[] {"--xxx"});
Console.WriteLine(string.Join(Environment.NewLine, a.BuildHelpText()));
~~~

### Example from homework definition ###

~~~
var arguments = new Arguments();
// Implicit recognition of short/long option name
// AddOption returns OptionBuilder instane
arguments.AddOption("v|verbose")
    // Description is used in help text generation
    .WithDescription("Verbose option description");

arguments.AddOption("s|size")
    .WithDescription("Size option with default value of 42")
    // Mandatory argument of Integer type.
    // Name SIZE is used in help text generation
    // OptionBuilder.WithArgument<T> returns ArgumentBuilder<T> instance
    .WithArgument<int>("SIZE")
    // hodnotu
    // Set default value. (It is ignored, when argument is mandatory)
    .WithDefaultValue(42)
    // Set value conditions. It is not used to check default value
    .WithCondition(v => v > 0);

try {
    //Parse arguments
    var commandLine = arguments.Parse(new String[] {"-v", "--size=42", "plain1", "plain2"});

    Console.WriteLine("verbose = {0}", commandLine.IsOptionSet("verbose"));

    Console.WriteLine("size = {0}", commandLine.GetOptionValue<int>("size"));
    Console.WriteLine("arguments = {0}", string.Join(" ", commandLine.GetPlainArguments()));
    Console.WriteLine(string.Join(Environment.NewLine, arguments.BuildHelpText()));
}
catch (ParseException e) {
    Console.WriteLine(e.StackTrace);
}

~~~

### Next example ###
Some other API methods
~~~
arguments = new Arguments();

// API class hierarchy:
arguments.AddOption("t") // OptionBuilder API
    .WithDescription("Popis") // OptionBuilder API
    .WithArgument<double>("CISLO") // ArgumentBuilder<double> API
    .WithCondition(d => d < 0.5); // ArgumentBuilder<double> API


// Help text
arguments.AddOption("h|help")
    .WithDescription("Show help text")
    .WithAction(() => Console.WriteLine(arguments.BuildHelpText()));


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

arguments.Parse(new String[] {"--v=text", "-i", "1"});
Console.WriteLine(string.Join(Environment.NewLine,
    arguments.BuildHelpText()));

Console.WriteLine("{0} = {1}", "someStringValue", someStringValue);
Console.WriteLine("{0} = {1}", "someActionValue", someActionValue);
Console.WriteLine("{0} = {1}", "someIntValue", someIntValue);

var x = arguments.GetOptionValue("v");
var y = arguments.GetOptionValue<int>("--i");
~~~




### Custom type processing ###
~~~
arguments.RegisterTypeConverter<long>(long.Parse);
arguments.AddOption("l|long-list")
    // This option takes exactly three arguments, i.e. -l 10 12 14
    .WithArguments<long>("LONGS", 3); 

arguments.RegisterTypeConverter<byte>(byte.Parse);
arguments.AddOption("b|byte-list")
 This option takes from one to ten arguments
   .WithArguments<byte>("BYTES", 1, 10);
~~~