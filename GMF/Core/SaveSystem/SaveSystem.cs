using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace GMF.Saving
{
    public interface IStorageProvider
    {
        // TODO: переделать на Stream
        Task SaveAsync(string path, byte[] data);
        Task<byte[]> LoadAsync(string path);
        Task DeleteAsync(string path);
        Task<IEnumerable<string>> GetFilesAsync(string directory);
    }

    public interface ISaveLoadManager
    {
        Action<ISaveData, ISaveMetaData> OnSaveLoaded { get; set; }
        Task<IEnumerable<ISaveMetaData>> GetSaves();
        Task SaveAsync(ISaveMetaData saveSlot);
        Task LoadAsync(ISaveMetaData saveSlot);
        Task AutoLoadAsync();
        Task DeleteAsync(ISaveMetaData saveSlot);
        void Initialize();
    }

    public interface ISaveMetaData
    {
        string Name { get; }
        string Path { get; }
        DateTime Date { get; }
        string Version { get; }

        public async Task<byte[]> SerializeAsync()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(ms, this);
                return ms.ToArray();
            }
        }
    }

    // Интерфейс для обеспечения сериализации данных
    public interface IDataSerializer
    {
        Task SerializeAsync(ISaveData data, IStorageProvider provider, ISaveMetaData metadata);
        Task<ISaveData> DeserializeAsync(ISaveData data, IStorageProvider provider, ISaveMetaData metadata);
    }

    // Интерфейс для хранения данных
    public interface ISaveData
    {
        public Dictionary<string, object> SubData { get; }
        void OnBeforeSave();
        void OnAfterLoad();
    }

    [Serializable]
    public struct SimpleSaveMetaData : ISaveMetaData
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime Date { get; set; }
        public string Version { get; set; }
    }

    public static class SaveSystem
    {
        static ISaveData CurrentSaveData { get; set; }
        public static ISaveMetaData CurrentSaveSlot { get; private set; }
        public static ISaveLoadManager SaveLoadManager { get; private set; }

        public static Action<ISaveData, ISaveMetaData> OnSaveLoaded { get; set; }

        public static void Initialize(ISaveLoadManager saveLoadManager)
        {
            SaveLoadManager = saveLoadManager;
            saveLoadManager.OnSaveLoaded += SaveLoaded;
        }

        static void SaveLoaded(ISaveData saveData, ISaveMetaData saveMetaData)
        {
            CurrentSaveData = saveData;
            CurrentSaveSlot = saveMetaData;
            OnSaveLoaded?.Invoke(saveData, saveMetaData);
        }
        public static T GetCurrentSave<T>() where T : class, ISaveData
        {
            return CurrentSaveData as T;
        }

        public static Task SaveCurrentAsync()
        {
            return SaveLoadManager.SaveAsync(CurrentSaveSlot);
        }

        public static void SaveCurrent()
        {
            // new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default).StartNew(SaveCurrentAsync).Unwrap().GetAwaiter().GetResult();
            Task.Run(SaveCurrentAsync).GetAwaiter().GetResult();
        }

        public static Task TryAutoLoadAsync()
        {
            return SaveLoadManager.AutoLoadAsync();
        }
    }
}