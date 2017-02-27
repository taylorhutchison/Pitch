using System;
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

        public string SearchDir {get; private set;}
        public Regex Pattern {get; private set;}
        public string OutDir {get; private set;}
        public bool Help {get; private set;}
        public DiagnosticsLevel Diagnostics {get; private set;}

        public static Options ParseOptions(string[] args){
            var defaultSearchDir = "./";
            var defaultPattern = "*";
            var defaultOutDir = "pitch-output";
            var defaultDiagnostics = DiagnosticsLevel.Warnings;
            var defaultHelp = false;

            for(int i = 0; i < args.Length; i++){
                if(args[i].ToLower() == "--searchdir" && i < args.Length - 1){
                    defaultSearchDir = args[i + 1];
                    i++;
                }
                if(args[i].ToLower() == "--pattern" && i < args.Length - 1){
                    defaultPattern = args[i + 1];
                    i++;
                }

                if(args[i].ToLower() == "--outdir" && i < args.Length - 1){
                    defaultOutDir = args[i + 1];
                    i++;
                }

                if(args[i].ToLower() == "--diagnostics" && i < args.Length - 1){
                    defaultDiagnostics = (DiagnosticsLevel)Int32.Parse(args[i + 1]);
                    i++;
                }

                if(args[i].ToLower() == "--help"){
                    defaultHelp = true;
                }
            }

            return new Options {
                SearchDir = defaultSearchDir,
                Pattern = WildcardToRegex(defaultPattern),
                OutDir = defaultOutDir,
                Diagnostics = defaultDiagnostics,
                Help = defaultHelp
            };
        }

    }
}