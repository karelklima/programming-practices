using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentsLibrary {

    internal class HelpTextGenerator {

        internal static string Generate(Dictionary<OptionAlias, Option> options) {
            // TODO refactor into several methods
            var result = new List<string>();
            StringBuilder sb = new StringBuilder("Help:");
            result.Add(sb.ToString());
            sb.Clear();
            foreach (Option opt in options.Values.Distinct()) {
                var shorts = new List<OptionAlias>();
                var longs = new List<OptionAlias>();
                foreach (OptionAlias alias in opt.Aliases) {
                    if (alias.Type == OptionType.Short) {
                        shorts.Add(alias);
                    }
                    else {
                        longs.Add(alias);
                    }
                }

                string argument = null;
                if (opt.Argument != null) {
                    if (opt.Argument.Optional) {
                        dynamic defaultValue = opt.Argument.DefaultValue;
                        string dValue = "";
                        if (defaultValue != null) {
                            dValue = defaultValue.ToString();
                            if (dValue.Length == 0) {
                                dValue = "\"\"";
                            }
                        }
                        else {
                            dValue = "null";
                        }
                        argument = String.Format("<{0}:{1}={2}>",
                            opt.Argument.Name, opt.Argument.GetValueType().Name,
                            dValue);
                    }
                    else {
                        argument = String.Format("<{0}:{1}>", opt.Argument.Name,
                            opt.Argument.GetValueType().Name);
                    }
                }

                if (shorts.Count > 0) {
                    sb.Append("\t");
                    sb.Append(string.Join("|", shorts));
                    if (argument != null) {
                        sb.AppendFormat(" {0}", argument);
                    }
                    if (longs.Count > 0) {
                        result.Add(sb.ToString());
                        sb.Clear();
                    }
                }

                if (longs.Count > 0) {
                    sb.Append("\t");
                    sb.Append(string.Join("|", longs));
                    if (argument != null) {
                        sb.AppendFormat("={0}", argument);
                    }
                }

                sb.Append(":");
                if (opt.Mandatory) {
                    sb.Append(" Mandatory");
                }
                else {
                    sb.Append(" Optional");
                }
                result.Add(sb.ToString());
                sb.Clear();
                sb.AppendFormat("\t\t{0}", opt.Description);
                result.Add(sb.ToString());
                sb.Clear();
                result.Add(sb.ToString());
                sb.Clear();
            }
            result.Add(sb.ToString());
            sb.Clear();
            return String.Join("\n", result);
        }

    }

}