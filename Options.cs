using System.Text.RegularExpressions;

namespace Pitch {

    public class Options {
        private static Regex WildcardToRegex(string wildcard)
        {
            var pattern = "^" + Regex.Escape(wildcard).Replace("\\?", ".").Replace("\\*", ".*")+ "$";
            return new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public enum DiagnosticsLevel {
            Silent,
            Errors,
            Warnings,
            Verbose
        }

        public Regex Pattern {get; private set;}
        public string OutDir {get; private set;}
        public DiagnosticsLevel Diagnostics {get; private set;}

        public static Options ParseOptions(string[] args){
            return new Options {
                Pattern = WildcardToRegex("*.html"),
                OutDir = "",
                Diagnostics = DiagnosticsLevel.Warnings
            };
        }

    }
}