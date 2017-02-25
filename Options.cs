using System.Text.RegularExpressions;

namespace Pitch {

    public class Options {
        private static string WildcardToRegex(string wildcard)
        {
            return "^" + Regex.Escape(wildcard).Replace("\\?", ".").Replace("\\*", ".*")+ "$";
        }

        public enum DiagnosticsLevel {
            Silent,
            Errors,
            Warnings,
            Verbose
        }

        public string Pattern {get; private set;}
        public string OutDir {get; private set;}
        public DiagnosticsLevel Diagnostics {get; private set;}

        public static Options ParseOptions(string[] args){
            return new Options {
                Pattern = WildcardToRegex("*pitch.*"),
                OutDir = "",
                Diagnostics = DiagnosticsLevel.Warnings
            };
        }

    }
}