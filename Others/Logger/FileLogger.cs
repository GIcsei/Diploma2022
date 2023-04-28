using System;

namespace PSI_Checker_2p0.Logger
{
    public class FileLogger : ILogger
    {
        public string FilePath { get; set; }

        public bool LogTime { get; set; } = true;

        public FileLogger(string filePath)
        {
            FilePath = filePath;
        }

        public void Log(string message, LogLevel logLevel)
        {
            var currentTime = DateTimeOffset.Now.ToString("yyyy-MM-dd hh:mm:ss");

            var timeLogString = LogTime ? $"[{currentTime}]" : "";

            //DIService.File.WriteTextToFileAsync($"{timeLogString}{message}{Environment.NewLine}", FilePath, append: true);
        }
    }
}
