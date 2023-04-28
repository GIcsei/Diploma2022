using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PSI_Checker_2p0.FileHandler.FileManager.FileManagers
{
    public sealed class Serializer
    {
        public static async Task SaveJson<T>(T serializable, string name, string path = null)
        {
            string fileName = name;
            if (!fileName.EndsWith(".json"))
            {
                fileName = name + ".json";
            }
            if (!(path is null))
                fileName = Path.Combine(path, fileName);
            using (FileStream createStream = File.Create(fileName))
            {
                await JsonSerializer.SerializeAsync(createStream, serializable);
                await createStream.FlushAsync();
                createStream.Dispose();
            };
        }

        public static async Task<T> LoadJson<T>(string path) where T : class, new()
        {
            string fileName = path;
            T result = new T();
            if (fileName is null)
                throw new NullReferenceException();
            using (FileStream openStream = File.OpenRead(fileName))
            {
                result = await JsonSerializer.DeserializeAsync<T>(openStream);
            };
            return result;
        }
    }
}
