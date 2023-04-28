using PSI_Checker_2p0.PSI5_Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PSI_Checker_2p0.Checker
{
    public abstract class BaseResultHandler : IResultData<BaseDataContainer>
    {
        protected BaseDataContainer dataContainer;
        protected string FullPath = null;

        private string _directory = null;
        private string savingName = null;
        public string Description => throw new NotImplementedException();
        public string FileName => savingName;
        protected readonly string extension;

        protected const string PATTERN = @"^\d+_S\d_R\d_D\d+_B\d_C\d_ATT\d$";
        public BaseDataContainer Result => dataContainer;
        public string SaveLocation
        {
            get => _directory;
            set => _directory = value;
        }

        public bool AddData(BaseDataContainer newData)
        {
            dataContainer = newData;
            return true;
        }

        public abstract IResultData<BaseDataContainer> LoadFromFile(string filePath);
        public abstract bool SaveToFile();

        public IEnumerable<BaseDataContainer> LoadFromFolder(string directory)
        {
            foreach (string path in GetNextPath(directory))
            {
                LoadFromFile(path);
                yield return dataContainer;
            }
        }

        /// <summary>
        /// Get the full folder name which contains extension.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private IEnumerable<string> GetNextPath(string directory)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(directory);
            Regex re = new Regex(PATTERN);
            foreach (DirectoryInfo dir in dirInfo.GetDirectories().AsParallel().Where((x) => re.IsMatch(x.Name)))
            {
                foreach (FileInfo subdir in dir.GetFiles())
                {
                    if (subdir.Extension.EndsWith(extension, StringComparison.OrdinalIgnoreCase) & subdir.Name.Contains("_BiDir_"))
                    {
                        yield return subdir.FullName;
                    }
                }
            }
        }
    }
}