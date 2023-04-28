using PSI_Checker_2p0.Enums;
using PSI_Checker_2p0.FileHandler.FileLoader;
using PSI_Checker_2p0.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PSI_Checker_2p0.FileHandler.FileSaver
{
    public abstract class BaseScopeFileHandler
    {
        private readonly string directoryPath;
        public string DirectoryPath
        {
            get => directoryPath;
        }
        public string FileName { get; private set; }

        private string folderName;
        public string FolderName
        {
            get => folderName;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException();
                }
                folderName = value;
                CreateSaveFolder();
            }
        }

        protected string Channel
        {
            get;
            private set;
        }
        private int ChannelNum = 0;

        protected int index = 0;
        protected FileInfo filePath;
        protected readonly long MaxSize = GigabyteToByteConverter.GBToByte(2); // Maximum filesize in bytes. by default: 2GB
        protected bool MaxSizeReached = false;
        protected readonly FileSavePolicy Policy;
        protected const int MaxNumberOfFiles = 5;

        // List of saved FilePaths, so the load back can be simplifed
        protected List<FileInfo> filePaths = new List<FileInfo>();

        #region Constructor
        public BaseScopeFileHandler(string folderName = @"NotDefined", string dirPath = @"C:\waveform")
        {
            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentException();
            }
            if (string.IsNullOrEmpty(dirPath))
            {
                throw new ArgumentNullException(nameof(dirPath));
            }
            directoryPath = dirPath;
            FileName = $"{index}";
            Channel = $"Channel_{ChannelNum}";
            FolderName = folderName;
            UpdateFilePath();
        }
        #endregion

        #region Internal functions
        private void CreateSaveFolder()
        {
            var fullpath = Path.Combine(DirectoryPath, FolderName);
            if (!Directory.Exists(fullpath))
            {
                Directory.CreateDirectory(fullpath);
            }
        }

        protected abstract void AddSaveFile();

        /// <summary>
        /// Updates the <seealso cref="filePath"/>. If the newfile doesn't exist,
        /// creates it.
        /// The new file path created according to the <seealso cref="Policy"/>.
        /// </summary>
        protected void UpdateFilePath()
        {

            FileName = $@"{index}";
            filePath = new FileInfo(
                Path.Combine(DirectoryPath,
                FolderName, FileName, Channel + ".txt"));
            Directory.CreateDirectory(
                Path.Combine(DirectoryPath,
                FolderName, FileName));
            if (!filePath.Exists)
            {
                filePath.Create().Close();
            }
        }

        /// <summary>
        /// Checks if the file size exceeds the <see cref="MaxSize"/>.
        /// </summary>
        /// <returns>Reference to the object</returns>
        protected BaseScopeFileHandler CheckFileLoad()
        {
            filePath.Refresh();
            MaxSizeReached = filePath.Length > MaxSize;
            return this;
        }

        /// <summary>
        /// Check if the channel is equals with the current value.
        /// If not, set the <see cref="MaxSizeReached"/> to false.
        /// </summary>
        /// <param name="channel">The new channel number.</param>
        private void CheckChannel(int channel)
        {
            if (channel == ChannelNum)
                return;
            ChannelNum = channel;
            Channel = $"Channel_{ChannelNum}";
        }

        private List<FileInfo> PopulateFilePaths(DirectoryInfo folderName)
        {
            var Loader = new TxtFileLoader();
            return Loader.LoadAllFiles(folderName).Select((x) => new FileInfo(x)).ToList();
        }
        #endregion


        /// <summary>
        /// Saves the data to the destination path.
        /// </summary>
        /// <param name="data">Data to be saved</param>
        /// <param name="channel">Identifier of the channel</param>
        public void SaveFile(double[] data, int channel = 0)
        {
            CheckChannel(channel);
            UpdateFilePath();
            CheckFileLoad();
            AddSaveFile();
            var fileMode = MaxSizeReached ? FileMode.Truncate : FileMode.Append;
            using (var fileStream = new BinaryWriter(filePath.Open(fileMode, FileAccess.Write)))
            {
                foreach (var elem in data)
                {
                    fileStream.Write(elem);
                }
            }
        }

        /// <summary>
        /// Loads the requested number of data from the given file.
        /// </summary>
        /// <param name="path">The path of the data file.</param>
        /// <param name="elemNum">The requested number of data from the file.</param>
        /// <param name="offset">Offset value where the read starts from.</param>
        /// <returns></returns>
        public double[] LoadFile(FileInfo path,
                                 long elemNum,
                                 int offset = 0)
        {
            double[] data = new double[elemNum];
            bool success = false;
            while (!success)
            {
                try
                {
                    using (var fileStream = new BinaryReader(path.Open(FileMode.Open, FileAccess.Read)))
                    {
                        fileStream.BaseStream.Seek(offset * sizeof(double), SeekOrigin.Begin);
                        for (var i = 0; i < elemNum; i++)
                        {
                            data[i] = fileStream.ReadDouble();
                        }
                        success = true;
                    }
                }
                catch { }
            }
            return data;
        }

        /// <summary>
        /// Loads back all the data from the files.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double[]> LoadAllData()
        {
            if (filePaths is null)
                yield break;
            foreach (var path in filePaths)
            {
                path.Refresh();
                var elemNum = path.Length / sizeof(double);
                yield return LoadFile(path, elemNum);
            }
        }

        /// <summary>
        /// Loads back all the data for the given channel.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public IEnumerable<double[]> LoadAllDataForChannel(int channel)
        {
            foreach (var path in filePaths.Where(x => x.FullName.Contains($"Channel_{channel}")))
            {
                var elemNum = path.Length / sizeof(double);
                yield return LoadFile(path, elemNum);
            }
        }

        /// <summary>
        /// Clears the directory where the data was saved.
        /// </summary>
        public void ClearAll() => filePaths.AsParallel().ForAll(x => x.Delete());

        /// <summary>
        /// Moves all file to the given destination.
        /// </summary>
        /// <param name="directoryInfo"></param>
        public void MoveTo(DirectoryInfo directoryInfo)
        {
            Directory.Move(DirectoryPath, directoryInfo.FullName);
        }

        /// <summary>
        /// Load saved raw data back from the given folder recursievly.
        /// This function overrides the <see cref="filePaths"/> value!
        /// </summary>
        /// <param name="folderName">The path to the folder where the results can be found.</param>
        /// <param name="channel">Specify the channel to load back. If value is not positive, all channels will be loaded</param>
        /// <returns></returns>
        public List<double[]> LoadChannelsFromFolder(string folderName, int channel = -1)
        {
            filePaths = PopulateFilePaths(new DirectoryInfo(folderName));
            List<double[]> result;
            if (channel < 0)
                result = LoadAllData().ToList();
            else
                result = LoadAllDataForChannel(channel).ToList();
            return result;
        }
    }
}
