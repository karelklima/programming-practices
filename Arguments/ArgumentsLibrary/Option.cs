using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentsLibrary {

    internal class Option {

        internal const bool DEFAULT_MANDATORY = false;

        internal List<OptionAlias> Aliases { get; set; }

        internal bool Mandatory { get; set; }

        internal string Description { get; set; }

        internal Type ArgumentType { get; set; }

        internal dynamic Argument { get; set; }

        internal List<Action> Actions { get; set; }

        internal bool IsSet { get; set; }

        internal Option() {
            // TODO
            Aliases = new List<OptionAlias>();
            Mandatory = DEFAULT_MANDATORY;

            Actions = new List<Action>();
            IsSet = false;
        }

        internal void InvokeActions() {
            Actions.ForEach(action => action.Invoke());
        }

    }

}