using UnityEngine;

namespace RealMethod
{
    public interface IDataFile
    {
        void InitiateFile(DataManager manager);
        void FileLoaded();
        void FileSaved();
        void FileRemoved();
    }
    public interface IStorage
    {
        void StorageCreated(Object author);
        void StorageLoaded(Object author);
        void StorageClear();
    }
    [System.Serializable]
    public struct StorageFile<T, J> where T : IStorage where J : SaveFile
    {
        [SerializeField]
        private bool UseCustomFile;
        [SerializeField, ConditionalHide("UseCustomFile", true, false)]
        private SaveFile _SaveFile;
        public SaveFile file
        {
            get
            {
                if (_SaveFile == null)
                {
                    if (!TryGetStorage(out cacheProvider))
                    {
                        Debug.LogWarning($"{this}: Can't get Storage Interface Something is wrong in your 'CustomSavefile' or 'SaveFileClass'");
                        return default;
                    }
                }
                return _SaveFile;
            }
        }
        private T cacheProvider;
        public T provider
        {
            get
            {
                if (cacheProvider == null)
                {
                    if (!TryGetStorage(out cacheProvider))
                    {
                        Debug.LogWarning($"{this}: Can't get Storage Interface Something is wrong in your 'CustomSavefile' or 'SaveFileClass'");
                        return default;
                    }
                }
                return cacheProvider;
            }
        }


        // Public Functions
        public bool Load(Object author)
        {
            return Load(author, Game.FindManager<DataManager>());
        }
        public bool Load(Object author, DataManager saveManager)
        {
            if (saveManager == null)
            {
                Debug.LogWarning($"{this}: Initiate faield we need DataManager");
                return false;
            }

            if (saveManager.IsExistFile(_SaveFile))
            {
                saveManager.LoadFile(_SaveFile);
                provider.StorageLoaded(author);
                return true;
            }
            else
            {
                provider.StorageCreated(author);
                return false;
            }
        }
        public void Clear()
        {
            provider.StorageClear();
        }

        // Private Functions
        private bool TryGetStorage(out T provider)
        {
            if (UseCustomFile)
            {
                if (_SaveFile is T customProvider)
                {
                    provider = customProvider;
                    return true;
                }
                else
                {
                    Debug.LogWarning("Storage Interface not implemented in Customfile.");
                    UseCustomFile = false;
                }
            }
            _SaveFile = ScriptableObject.CreateInstance<J>();
            if (_SaveFile is T autoProvider)
            {
                _SaveFile.name = $"RM{typeof(J)}";
                provider = autoProvider;
                return true;
            }
            else
            {
                Debug.LogError($"Storage Interface not implemented in SaveFileType {typeof(J)}");
                provider = default;
                return false;
            }
        }
    }


    // DataMaanger Class 
    public abstract class DataManager : MonoBehaviour, IGameManager
    {

        [Header("Basic")]
        [SerializeField]
        private bool LoadOnInitiate = true;
        [SerializeField]
        private SaveFile[] StableFiles;

        // Actions
        public System.Action<SaveFile> OnFileLoaded;
        public System.Action<SaveFile> OnFileSaved;

        public byte Logindex { get; private set; }
        public string[] DataLog { get; private set; }

        // Operators
        public SaveFile this[int Index]
        {
            get => StableFiles[Index];
            protected set => StableFiles[Index] = value;
        }



        // Implement IGameManager Interface
        MonoBehaviour IGameManager.GetManagerClass()
        {
            return this;
        }
        void IGameManager.InitiateManager(bool AlwaysLoaded)
        {
            Logindex = 0;
            DataLog = new string[5];

            foreach (var file in StableFiles)
            {
                file.provider.InitiateFile(this);
                if (LoadOnInitiate)
                {
                    if (IsExistFile(file))
                        LoadFile(file);
                }
            }
        }
        void IGameManager.InitiateService(Service service)
        {
            InitiateService(service);
        }


        // Public Functions
        public void Save()
        {
            if (StableFiles == null)
            {
                Debug.LogError("StableFiles is not valid");
                return;
            }
            foreach (var file in StableFiles)
            {
                if (file == null)
                {
                    Debug.LogError("File is not valid");
                    continue;
                }
                OnSaveFile(file);
                file.provider.FileSaved();
                OnFileSaved?.Invoke(file);
            }
        }
        public void SaveFile(int Index)
        {
            if (StableFiles.IsValidIndex(Index))
            {
                SaveFile(StableFiles[Index]);
            }
            else
            {
                Debug.LogError("Invalid index passed to SaveFile: " + Index);
            }
        }
        public void SaveFile(SaveFile file)
        {
            if (file == null)
            {
                Debug.LogError("File is not valid");
                return;
            }
            OnSaveFile(file);
            file.provider.FileSaved();
            OnFileSaved?.Invoke(file);
        }
        public void Load()
        {
            if (StableFiles == null)
            {
                Debug.LogError("StableFiles is not valid");
                return;
            }
            foreach (var file in StableFiles)
            {
                if (file == null)
                {
                    Debug.LogError("File Does not valid");
                    continue;
                }
                OnLoadFile(file);
                file.provider.FileLoaded();
                OnFileLoaded?.Invoke(file);
            }
        }
        public void LoadFile(int Index)
        {
            if (StableFiles.IsValidIndex(Index))
            {
                LoadFile(StableFiles[Index]);
            }
            else
            {
                Debug.LogError("Invalid index passed to LoadFile: " + Index);
            }
        }
        public void LoadFile(SaveFile file)
        {
            if (file == null)
            {
                Debug.LogError("File Does not valid");
                return;
            }

            OnLoadFile(file);
            file.provider.FileLoaded();
            OnFileLoaded?.Invoke(file);
        }
        public void DeleteFile(int Index)
        {
            if (StableFiles.IsValidIndex(Index))
            {
                DeleteFile(StableFiles[Index]);
            }
            else
            {
                Debug.LogError("Invalid index passed to DeleteFile: " + Index);
            }
        }
        public void DeleteFile(SaveFile file)
        {
            if (file == null)
            {
                Debug.LogError("File Does not valid");
                return;
            }

            OnDeleteFile(file);
            file.provider.FileRemoved();
        }
        public bool IsExistFile(int Index)
        {
            if (StableFiles.IsValidIndex(Index))
            {
                return IsExistFile(StableFiles[Index]);
            }
            else
            {
                Debug.LogError("Invalid index passed to IsExistFile: " + Index);
                return false;
            }
        }
        public bool IsExistFile(SaveFile file)
        {
            if (file == null)
            {
                Debug.LogError("File Does not valid");
                return false;
            }
            return IsExist(file);
        }


        // Protected Function
        protected void WriteLog(string message, SaveFile file)
        {
            if (Application.isPlaying && DataLog != null)
            {
                if (Logindex == 0)
                {
                    DataLog[0] = $"{System.DateTime.Now} -- {file.name} -- {message}";
                    Logindex++;
                }
                else
                {
                    DataLog[Logindex % DataLog.Length] = $"{System.DateTime.Now} -- {file.name} -- {message}";
                    Logindex++;
                }
            }
        }


        // Abstract Mehtod
        protected abstract void InitiateService(Service service);
        protected abstract bool IsExist(SaveFile targetfile);
        protected abstract void OnDeleteFile(SaveFile targetfile);
        protected abstract void OnSaveFile(SaveFile targetfile);
        protected abstract void OnLoadFile(SaveFile targetfile);
    }

    // Save File Class
    public abstract class SaveFile : DataAsset, IDataFile
    {
        public IDataFile provider => this;

        // Implement IDataPersistence Interface
        void IDataFile.InitiateFile(DataManager manager)
        {
            OnStable(manager);
        }
        void IDataFile.FileSaved()
        {
            OnSaved();
        }
        void IDataFile.FileLoaded()
        {
            OnLoaded();
        }
        void IDataFile.FileRemoved()
        {
            OnDeleted();
        }


        protected abstract void OnStable(DataManager manager);
        protected abstract void OnLoaded();
        protected abstract void OnSaved();
        protected abstract void OnDeleted();


#if UNITY_EDITOR
        [ContextMenu("ResetToDefault")]
        private void Editor_BacktoDefault()
        {
            OnEditorPlay();
        }
        [ContextMenu("Save")]
        private void Editor_SaveSelf()
        {
            var manager = FindFirstObjectByType<DataManager>();
            if (manager != null)
            {
                manager.SaveFile(this);
            }
            else
            {
                Debug.LogError("No DataManager found in the scene.");
            }
        }
        [ContextMenu("Load")]
        private void Editor_LoadSelf()
        {
            var manager = FindFirstObjectByType<DataManager>();
            if (manager != null)
            {
                manager.LoadFile(this);
            }
            else
            {
                Debug.LogError("No DataManager found in the scene.");
            }
        }
        [ContextMenu("Delete")]
        private void Editor_Delete()
        {
            var manager = FindFirstObjectByType<DataManager>();
            if (manager != null)
            {
                manager.DeleteFile(this);
            }
            else
            {
                Debug.LogError("No DataManager found in the scene.");
            }
        }
#endif



    }

}