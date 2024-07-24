using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace GMF.Saving
{
    public class BinaryDataSerializer : IDataSerializer
    {
        public async Task SerializeAsync(ISaveData data, IStorageProvider provider, ISaveMetaData metadata)
        {
            data.OnBeforeSave();
            {
                string fullPath = Path.Combine(metadata.Path, "Save");
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    await Task.Run(() => formatter.Serialize(ms, fullPath));
                    await provider.SaveAsync(fullPath, ms.ToArray());
                }
            }

            foreach (var kvp in data.SubData)
            {
                string fullPath = Path.Combine(metadata.Path, "Sub", kvp.Key);
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    await Task.Run(() => formatter.Serialize(ms, kvp.Value));
                    await provider.SaveAsync(fullPath, ms.ToArray());
                }
            }
        }

        public async Task<ISaveData> DeserializeAsync(ISaveData data, IStorageProvider provider, ISaveMetaData metadata)
        {
            ISaveData deserialized = null;
            {
                string fullPath = Path.Combine(metadata.Path, "Save");
                byte[] fileData = await provider.LoadAsync(fullPath);
                if (fileData.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream(fileData))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        deserialized = await Task<ISaveData>.Run(() => formatter.Deserialize(ms) as ISaveData);
                    }
                }
            }
            
            foreach (var kvp in data.SubData)
            {
                string fullPath = Path.Combine(metadata.Path, "Sub", kvp.Key);
                byte[] fileData = await provider.LoadAsync(fullPath);
                using (MemoryStream ms = new MemoryStream(fileData))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    var deserializedData = await Task.Run(() => formatter.Deserialize(ms));
                    data.SubData[kvp.Key] = deserializedData;
                }
            }
            data.OnAfterLoad();

            return deserialized;

        }
    }


}