using System;
using System.Collections.Generic;
using System.IO;

namespace PSI_Checker_2p0.FileHandler.FileLoader
{
    public class PatternFileLoader : BaseFileLoader
    {
        public PatternFileLoader(FileInfo file) : base()
        {
            filePath = file.FullName;
            name = file.Name;
            Extension = file.Extension;
        }

        public PatternFileLoader() : base()
        {
            Extension = ".txt";
        }

        public IEnumerable<string> ReadAllRows()
        {
            if (String.IsNullOrEmpty(FilePath))
            {
                yield break;
            }
            using (StreamReader sr = new StreamReader(FilePath))
            {
                while (sr.Peek() != -1)
                {
                    var line = sr.ReadLine();
                    if (line == null)
                        yield break;
                    if (String.IsNullOrEmpty(line))
                        continue;
                    yield return line;
                }
            }
        }
    }
}
