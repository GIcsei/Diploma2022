using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PSI_Checker_2p0.FileHandler.FileLoader
{
    public abstract class BaseFileLoader
    {
        private string extension;
        protected string filePath;
        protected string name;

        protected string Extension
        {
            get => extension;
            set => extension = value;
        }
        public string FilePath
        {
            get => filePath;
        }

        public string Name
        {
            get => name;
        }

        public IEnumerable<string> LoadAllFiles(string Folder)
        {
            foreach (var path in LoadAllFiles(new DirectoryInfo(Folder)))
            {
                yield return path;
            }
        }

        /// <summary>
        /// Recursievly search for all files with the <see cref="Extension"/>
        /// and returns with their full patternFilePath.
        /// </summary>
        /// <param name="dirInfo">The root directory where the search starts.</param>
        /// <returns></returns>
        public IEnumerable<string> LoadAllFiles(DirectoryInfo dirInfo)
        {
            if (dirInfo.GetDirectories().Length == 0)
            {
                foreach (var directory in dirInfo.GetFiles())
                {
                    if (directory.Extension == Extension)
                        yield return directory.FullName;
                }
            }
            foreach (DirectoryInfo path in dirInfo.GetDirectories())
            {
                foreach (string fullPath in LoadAllFiles(path))
                {
                    yield return fullPath;
                }
            }
        }

        public int NumberOfFiles(string path) => NumberOfFiles(new DirectoryInfo(path));

        public int NumberOfFiles(DirectoryInfo dirInfo, int iterator = 0)
        {
            if (dirInfo.GetDirectories().Length == 0)
            {
                int count = dirInfo.GetFiles().AsParallel().Where(x => x.Extension == Extension).Count();
                return count;
            }
            foreach (DirectoryInfo path in dirInfo.GetDirectories())
            {
                iterator += NumberOfFiles(path);
            }
            return iterator;
        }
    }
}