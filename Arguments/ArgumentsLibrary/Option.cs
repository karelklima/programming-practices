﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentsLibrary {

    /// <summary>
    /// Arguments option representation
    /// See <see cref="OptionBuilder"/> to access it.
    /// <example>
    /// <code>
    /// var arguments = new Arguments();
    /// arguments.AddOption("v|verbose")
    ///     .WithDescription("Verbose option description");
    /// </code>
    /// </example>
    /// </summary>
    internal class Option {
        
        /// <summary>
        /// Default behaviour: Mandatory or Optional.
        /// </summary>
        internal const bool DEFAULT_MANDATORY = false;

        /// <summary>
        /// Default description of option.
        /// </summary>
        internal const string DEFAULT_DESCRIPTION = "";


        /// <summary>
        /// List of aliases.
        /// </summary>
        /// <value>The option aliases.</value>
        internal List<OptionAlias> Aliases { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ArgumentsLibrary.Option"/> is mandatory.
        /// </summary>
        /// <value><c>true</c> if mandatory; otherwise, <c>false</c>.</value>
        internal bool Mandatory { get; set; }

        /// <summary>
        /// Gets or sets the option description.
        /// See <see cref="Option.DEFAULT_DESCRIPTION"/> for default value
        /// </summary>
        /// <value>The option description.</value>
        internal string Description { get; set; }

        /// <summary>
        /// Gets or sets the option argument.
        /// </summary>
        /// <value>The option argument.</value>
        internal dynamic Argument { get; set; }

        /// <summary>
        /// List of actions, which are invoked when option appears in args string
        /// It will be invoked after option name is parsed.
        /// Action signature: void function(){ ... }
        /// </summary>
        internal List<Action> Actions { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentsLibrary.Option"/> class.
        /// </summary>
        internal Option() {
            Aliases = new List<OptionAlias>();
            Mandatory = DEFAULT_MANDATORY;
            Description = DEFAULT_DESCRIPTION;
            Argument = null;
            Actions = new List<Action>();
        }

        /// <summary>
        /// Invokes the actions.
        /// </summary>
        internal void InvokeActions() {
            Actions.ForEach(action => action.Invoke());
        }

    }

}