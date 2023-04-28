using PSI_Checker_2p0.FileHandler.FileLoader;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace PSI_Checker_2p0
{
    /// <summary>
    /// Stores the description of an input pattern.
    /// </summary>
    public class Pattern
    {
        /// <summary>
        /// The name of the pattern.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Represents the pattern. Each element is a tuple of voltage-time pairs.
        /// The voltage is [V], the time is [us].
        /// </summary>
        public List<Tuple<double, double>> Values { get; set; }

        /// <summary>
        /// Loads the first pattern found in the <paramref name="file"/>.
        /// </summary>
        /// <param name="file">The file of the pattern descriptor</param>
        /// <returns>True if the load was successful</returns>
        public virtual async Task<bool> PopulatePattern(FileInfo file)
        {
            if (file.Extension != ".txt")
                return false;
            var loader = new PatternFileLoader(file);
            await Task.Run(() =>
            {
                var Values = new List<Tuple<double, double>>();
                foreach (var elem in loader.ReadAllRows())
                {
                    if (elem.StartsWith("END", StringComparison.OrdinalIgnoreCase))
                        break;
                    if (elem.StartsWith("BEGIN", StringComparison.OrdinalIgnoreCase))
                    {
                        Name = elem.Split('\t')[1];
                        continue;
                    }
                    var splittedString = elem.Split('\t');
                    Values.Add(new Tuple<double, double>(
                        Double.Parse(splittedString[1], new CultureInfo("en-US")),
                        Double.Parse(splittedString[3], new CultureInfo("en-US"))));
                }
            });
            return true;
        }

        public static async Task<List<Pattern>> LoadAllPatterns(FileInfo file)
        {
            var result = new List<Pattern>();
            if (file.Extension != ".txt")
                return result;
            var loader = new PatternFileLoader(file);
            await Task.Run(() =>
            {
                Pattern pattern = null;
                foreach (var elem in loader.ReadAllRows())
                {
                    if (elem.StartsWith("BEGIN", StringComparison.OrdinalIgnoreCase))
                    {
                        pattern = new Pattern
                        {
                            Values = new List<Tuple<double, double>>(),
                            Name = elem.Split('\t')[1]
                        };
                        continue;
                    }
                    if (elem.StartsWith("END", StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(pattern);
                        continue;
                    }
                    var splittedString = elem.Split('\t');
                    pattern.Values.Add(new Tuple<double, double>(
                        Double.Parse(splittedString[1], new CultureInfo("en-US")),
                        Double.Parse(splittedString[3], new CultureInfo("en-US"))));
                }
            });
            return result;
        }
    }
}
