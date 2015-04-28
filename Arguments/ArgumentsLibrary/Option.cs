using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentsLibrary
{
    internal class Option
    {
        internal const bool DEFAULT_MANDATORY = false;

        internal bool Mandatory { get; set; }

        internal string Description { get; set; }

        internal object Argument { get; set; }

        internal List<Action> Actions { get; set; }

        internal bool IsSet { get; set; }

    internal Option()
        {
            // TODO
            Mandatory = DEFAULT_MANDATORY;
            Actions = new List<Action>();
            IsSet = false;
        }

        

    }
}
