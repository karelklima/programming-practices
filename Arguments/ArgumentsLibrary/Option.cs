using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentsLibrary
{
    internal class Option
    {

        internal bool Mandatory { get; set; }

        internal string Description { get; set; }

        internal object Argument { get; set; }

        internal List<Action> Actions { get; set; } 

        internal Option()
        {
            // TODO
            Mandatory = false;
            Actions = new List<Action>();
        }

        

    }
}
