using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentsLibrary {

    internal class Option {

        internal const bool DEFAULT_MANDATORY = false;
        internal const string DEFAULT_DESCRIPTION = "";

        internal List<OptionAlias> Aliases { get; set; }

        internal bool Mandatory { get; set; }

        internal string Description { get; set; }

        internal dynamic Argument { get; set; }

        internal List<Action> Actions { get; set; }

        internal Option() {
            Aliases = new List<OptionAlias>();
            Mandatory = DEFAULT_MANDATORY;
            Description = DEFAULT_DESCRIPTION;
            Argument = null;
            Actions = new List<Action>();
        }

        internal void InvokeActions() {
            Actions.ForEach(action => action.Invoke());
        }

    }

}