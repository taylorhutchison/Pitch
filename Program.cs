using System;
using System.IO;
using System.Collections.Generic;
using static Pitch.Options;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pitch
{
    class Program
    {
        static IEnumerable<string> FilePaths;
        //static Dictionary<string, string> Layouts;

        static IEnumerable<string> DirSearch(string directory, Options options)
        {
            var files = new List<string>();
            foreach (string d in Directory.GetDirectories(directory))
            {
                foreach (string f in Directory.GetFiles(d))
                {
                    if (options.Pattern.IsMatch(f))
                    {
                        files.Add(f);
                    }
                }
                files.AddRange(DirSearch(d, options));
            }
            return files;
        }

        static string GetLayoutPath(string filePath, string layoutPath)
        {
            var count = Regex.Matches(Regex.Escape(layoutPath),  @"../").Count;
            var relativeFileName = Path.GetFileName(layoutPath);
            var filePathParts = filePath.Split('/');
            return string.Join("/", filePathParts.Take(filePathParts.Count() - count - 1)) + "/" + relativeFileName;
        }

        static void Main(string[] args)
        {
            const string layoutRegex = ".*layout=\"(.*)\"";
            var options = ParseOptions(args);
            //Phase 1: Discover
            Directory.SetCurrentDirectory("test");
            var cwd = Directory.GetCurrentDirectory();
            Console.WriteLine($"CWD: {cwd}");
            FilePaths = DirSearch(cwd, options);

            //Phase 2: Link
            foreach (var file in FilePaths)
            {
                var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
                var reader = new StreamReader(stream as FileStream, Encoding.UTF8, true, 512);
                var line = reader.ReadLine();
                if (Regex.IsMatch(line, layoutRegex, RegexOptions.IgnoreCase))
                {
                    var match = Regex.Match(line, layoutRegex, RegexOptions.IgnoreCase)?.Groups[1]?.Captures[0]?.Value;
                    var layoutPath = GetLayoutPath(file, match);
                    Console.WriteLine(layoutPath);
                }
            }

            //Phase 3: Output

            //Phase 4: Diagnostics
        }
    }
}
