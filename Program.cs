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
        static Dictionary<string, string> Layouts = new Dictionary<string,string>();

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
            var count = Regex.Matches(Regex.Escape(layoutPath), @"../").Count;
            var relativeFileName = Path.GetFileName(layoutPath);
            var filePathParts = filePath.Split('/');
            return string.Join("/", filePathParts.Take(filePathParts.Count() - count - 1)) + "/" + relativeFileName;
        }

        static string GetLayoutContents(string layoutPath)
        {
            if (Layouts.Keys.Contains(layoutPath))
            {
                return Layouts[layoutPath];
            }
            var stream = new FileStream(layoutPath, FileMode.Open, FileAccess.Read);
            var reader = new StreamReader(stream as FileStream, Encoding.UTF8, true, 512);
            var contents = reader.ReadToEnd();
            Layouts.Add(layoutPath, contents);
            return contents;
        }

        static void Main(string[] args)
        {
            const string layoutRegex = ".*layout=\"(.*)\"";
            var options = ParseOptions(args);
            Console.WriteLine($"Options: searchDir = {options.SearchDir}, pattern = {options.Pattern}, outDir = {options.OutDir}, diagnostics = {options.Diagnostics}");
            //Phase 1: Discover

            Directory.SetCurrentDirectory(options.SearchDir);
            var cwd = Directory.GetCurrentDirectory();

            Directory.CreateDirectory(options.OutDir);

            FilePaths = DirSearch(cwd, options);

            //Phase 2: Link
            foreach (var file in FilePaths)
            {
                var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
                var reader = new StreamReader(stream as FileStream, Encoding.UTF8, true, 512);
                var line = reader.ReadLine();
                if (Regex.IsMatch(line, layoutRegex, RegexOptions.IgnoreCase))
                {
                    var fileContents = reader.ReadToEnd();
                    var match = Regex.Match(line, layoutRegex, RegexOptions.IgnoreCase);
                    var layout = match?.Groups[1]?.Captures[0]?.Value;
                    var layoutPath = GetLayoutPath(file, layout);
                    string layoutContents = GetLayoutContents(layoutPath);
                    var combinedContents = layoutContents.Replace("@layout-outlet", fileContents);
                    var outPath = options.OutDir + file.Replace(cwd, "");
                    Directory.CreateDirectory(outPath.Replace(Path.GetFileName(outPath),""));
                    var outStream = new FileStream(outPath, FileMode.Create, FileAccess.Write);
                    var writer = new StreamWriter(outStream as FileStream, Encoding.UTF8, 512);
                    writer.Write(combinedContents);
                    writer.Flush();
                }
            }

            //Phase 3: Diagnostics
        }
    }
}
