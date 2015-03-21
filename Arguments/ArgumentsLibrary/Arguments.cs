using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArgumentsLibrary.Builders;

namespace ArgumentsLibrary
{
    public sealed class Arguments
    {
        private static readonly List<Option> _options = new List<Option>();  

        private Arguments()
        {
            
        }

        public static OptionBuilder AddOption(string alias)
        {
            var option = new Option(alias);
            _options.Add(option);
            return new OptionBuilder(option);
        }

        public static void Parse(string[] args)
        {
            // TODO implement
        }
    }
}
