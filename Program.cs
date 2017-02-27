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
        static readonly string layoutRegex = ".*layout=\"(.*)\"";
        static readonly string layoutOutletPattern = "@layout-outlet";
        static IEnumerable<string> FilePaths;
        static Dictionary<string, string> Layouts = new Dictionary<string, string>();

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
        static string ReadLayoutPath(string firstLine, string layoutRegex)
        {
            var match = Regex.Match(firstLine, layoutRegex, RegexOptions.IgnoreCase);
            return match?.Groups[1]?.Captures[0]?.Value;

        }
        static string GetLayoutPath(string filePath, string firstLine, string layoutRegex)
        {
            var layoutPath = ReadLayoutPath(firstLine, layoutRegex);
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

        static StreamReader OpenFileForRead(string file)
        {
            var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
            return new StreamReader(stream as FileStream, Encoding.UTF8, true, 512);
        }


        static string CombinedContents(StreamReader reader, string layoutContents)
        {
            var fileContents = reader.ReadToEnd();
            return layoutContents.Replace(layoutOutletPattern, fileContents);
        }

        static void WriteLayoutContents(string combinedContents, string file, Options options, string cwd)
        {

            var outPath = options.OutDir + file.Replace(cwd, "");
            Directory.CreateDirectory(outPath.Replace(Path.GetFileName(outPath), ""));
            var outStream = new FileStream(outPath, FileMode.Create, FileAccess.Write);
            var writer = new StreamWriter(outStream as FileStream, Encoding.UTF8, 512);
            writer.Write(combinedContents);
            writer.Flush();
        }

        static void Main(string[] args)
        {
            var options = ParseOptions(args);

            Directory.SetCurrentDirectory(options.SearchDir);
            var cwd = Directory.GetCurrentDirectory();

            Directory.CreateDirectory(options.OutDir);

            FilePaths = DirSearch(cwd, options);

            foreach (var file in FilePaths)
            {
                var reader = OpenFileForRead(file);
                var line = reader.ReadLine();
                if (Regex.IsMatch(line, layoutRegex, RegexOptions.IgnoreCase))
                {
                    var layoutPath = GetLayoutPath(file, line, layoutRegex);
                    string layoutContents = GetLayoutContents(layoutPath);
                    var combinedContents = CombinedContents(reader, layoutContents);
                    WriteLayoutContents(combinedContents, file, options, cwd);
                }
            }
        }
    }
}
