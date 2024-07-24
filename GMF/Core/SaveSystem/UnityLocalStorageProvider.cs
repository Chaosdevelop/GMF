using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace GMF.Saving
{
    public class UnityLocalStorageProvider : IStorageProvider
    {
        public static string PersonalPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "SaveData";

        public async Task SaveAsync(string path, byte[] data)
        {
            string fullPath = Path.Combine(PersonalPath, path);

            string directoryPath = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {

                await fs.WriteAsync(data, 0, data.Length);
            }
        }

        public async Task<byte[]> LoadAsync(string path)
        {
            string fullPath = Path.Combine(PersonalPath, path);
            string directoryPath = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directoryPath) || !File.Exists(fullPath))
            {
                return new byte[0];
            }
            using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                byte[] data = new byte[fs.Length];
                await fs.ReadAsync(data, 0, (int) fs.Length);
                return data;
            }
        }

        public Task DeleteAsync(string path)
        {
            File.Delete(path);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<string>> GetFilesAsync(string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath);
            return Task.FromResult<IEnumerable<string>>(files);
        }
    }
}
