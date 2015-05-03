using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentsLibrary {

    /// <summary>
    /// Help text generator.
    /// </summary>
    internal class HelpTextGenerator {

        /// <summary>
        /// Generates the header.
        /// </summary>
        /// <param name="sb">String builder.</param>
        private static void GenerateHeader(StringBuilder sb) {
            sb.AppendLine ("Help:");
        }

        /// <summary>
        /// Generates the footer.
        /// </summary>
        /// <param name="sb">String builder.</param>
        private static void GenerateFooter(StringBuilder sb) {
            sb.AppendLine ();
        }
            
        /// <summary>
        /// Divide aliases into groups for better formating
        /// </summary>
        /// <param name="opt">Option</param>
        /// <param name="shorts">List of short options</param>
        /// <param name="longs">List of long options</param>
        private static void DivideAliases(Option opt, List<OptionAlias> shorts, List<OptionAlias> longs){
            foreach (OptionAlias alias in opt.Aliases) {
                if (alias.Type == OptionType.Short) {
                    shorts.Add(alias);
                }
                else {
                    longs.Add(alias);
                }
            }
        }

        /// <summary>
        /// Formats the argument description.
        /// </summary>
        /// <returns>Argument representation as <Name:Type=DefaultValue></returns>
        /// <param name="opt">Option.</param>
        private static string FormatArgumentDescription(Option opt){
            if (opt.Argument == null)
                return null;

            string argumentTypeName = opt.Argument.GetType().GetGenericArguments ()[0].Name;

            //Mandatory, default value is not required
            if (!opt.Argument.Optional) {
                return String.Format("<{0}:{1}>", opt.Argument.Name,
                    argumentTypeName);
            }

            //Optional
            string defaultValue = "";
            if (opt.Argument.DefaultValue != null) {
                defaultValue = opt.Argument.DefaultValue.ToString();
                if (defaultValue.Length == 0) {
                    defaultValue = "\"\"";
                }
            }
            else {
                defaultValue = "null";
            }
            return String.Format("<{0}:{1}={2}>",
                opt.Argument.Name, argumentTypeName,
                defaultValue);
        }


        /// <summary>
        /// Generates the option description.
        /// </summary>
        /// <param name="sb">String builder.</param>
        /// <param name="opt">Option</param>
        /// Returns:
        /// <example>
        /// -ShortAlias1, -ShortAlias2, ... <ArgumentInfo>
        /// --LongAlias1, --LongAlias2, --LongAliasN=<ArgumentInfo>: <Optional|Mandatory>
        ///         Option Description
        /// </example>
        private static void GenerateOptionDescription(StringBuilder sb, Option opt) {
            var shorts = new List<OptionAlias>();
            var longs = new List<OptionAlias>();
            DivideAliases (opt, shorts, longs);
            string argument = FormatArgumentDescription(opt);

            if (shorts.Count > 0) {
                sb.Append("\t").Append(string.Join("|", shorts));
                if (argument != null) {
                    sb.AppendFormat(" {0}", argument);
                }
                if (longs.Count > 0) {
                    sb.AppendLine ();
                }
            }

            if (longs.Count > 0) {
                sb.Append("\t").Append(string.Join("|", longs));
                if (argument != null) {
                    sb.AppendFormat("={0}", argument);
                }
            }

            sb.Append(": ").AppendLine(opt.Mandatory ? "Mandatory" : "Optional");
            sb.AppendFormat("\t\t{0}", opt.Description);
            sb.AppendLine ().AppendLine ();
        }

        /// <summary>
        /// Generate help text for all options
        /// </summary>
        /// <param name="options">Option-Alias dictionary</param>
        internal static string Generate(Dictionary<OptionAlias, Option> options) {
            StringBuilder sb = new StringBuilder();
            GenerateHeader (sb);
            foreach (Option opt in options.Values.Distinct()) {
                GenerateOptionDescription (sb, opt);
            }
            GenerateFooter (sb);
            return sb.ToString ();
        }

    }

}