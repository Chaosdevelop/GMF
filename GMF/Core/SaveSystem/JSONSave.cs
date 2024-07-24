using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace GMF.Saving
{
    public class UnityJsonDataSerializer : IDataSerializer
    {

        public async Task SerializeAsync(ISaveData data, IStorageProvider provider, ISaveMetaData metadata)
        {

            data.OnBeforeSave();
            {
                string fullPath = Path.Combine(metadata.Path, "Save");

                var bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
                await provider.SaveAsync(fullPath, bytes);
            }

            foreach (var kvp in data.SubData)
            {
                string fullPath = Path.Combine(metadata.Path, "Sub", kvp.Key);
                var bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(kvp.Value));
                await provider.SaveAsync(fullPath, bytes);
            }
            byte[] metaDataBytes = await metadata.SerializeAsync();
            string metaDataPath = Path.Combine(metadata.Path, "metadata.json");
            await provider.SaveAsync(metaDataPath, metaDataBytes);
        }

        public async Task<ISaveData> DeserializeAsync(ISaveData data, IStorageProvider provider, ISaveMetaData metadata)
        {

            ISaveData deserialized = data;
            {
                string fullPath = Path.Combine(metadata.Path, "Save");
                byte[] fileData = await provider.LoadAsync(fullPath);
                if (fileData.Length > 0)
                {
                    var jsonString = Encoding.UTF8.GetString(fileData);
                    JsonUtility.FromJsonOverwrite(jsonString, data);
                    deserialized = data;

                }
            }

            foreach (var kvp in data.SubData)
            {
                string fullPath = Path.Combine(metadata.Path, "Sub", kvp.Key);
                byte[] fileData = await provider.LoadAsync(fullPath);
                if (fileData.Length > 0)
                {
                    var jsonString = Encoding.UTF8.GetString(fileData);
                    JsonUtility.FromJsonOverwrite(jsonString, kvp.Value);
                }
                else
                {
                    data.SubData[kvp.Key] = null;
                }

            }
            data.OnAfterLoad();

            return deserialized;

        }

    }


}