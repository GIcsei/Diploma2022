using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PSI_Checker_2p0.FileHandler.FileManagers
{
    public class FileManager : IFileManager
    {
        public string NormalizePath(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return path?.Replace('/', '\\').Trim();
            }
            else
            {
                return path?.Replace('\\', '/').Trim();
            }
        }

        public string ResolvePath(string path) => Path.GetFullPath(path);

        public async Task WriteTextToFileAsync(string text, string path, bool append = false)
        {
            // TODO Add exception handling

            path = NormalizePath(path);
            path = ResolvePath(path);

            await Task.Run(() =>
            {
                using (var fileStream = (TextWriter)new StreamWriter(File.Open(
                    path, append ? FileMode.Append : FileMode.Create)))
                {
                    fileStream.Write(text);
                }
            });
        }
    }
}
