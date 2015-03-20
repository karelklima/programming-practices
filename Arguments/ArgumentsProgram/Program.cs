using System;
using ArgumentsLibrary;

namespace ArgumentsProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            Arguments.AddOption("v")
                .WithAlias("verbose")
                .WithAlias("--v")
                .WithAlias("-verbose")
                .WithDescription("This is a help message")
                .WithRequiredArgument("some")
                .WithOptionalArgument("some")
                .WithArgumentType(ArgumentType.Integer)


        }
    }
}
