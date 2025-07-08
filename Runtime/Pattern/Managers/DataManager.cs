using UnityEngine;
using System;

namespace RealMethod
{
    public interface IDataPersistence
    {
        void Initiate(DataManager manager);
        void Load();
        void Save();
        void Remove();
    }


    public abstract class DataManager : MonoBehaviour, IGameManager
    {

        [Header("Basic")]
        [SerializeField]
        private bool LoadOnAwake = true;
        [SerializeField]
        private SaveFile[] StableFiles;

        // Actions
        public Action<SaveFile> OnFileChanged;
        public Action<SaveFile> OnFileLoaded;
        public Action<SaveFile> OnFileSaved;

        public byte Logindex { get; private set; }
        public string[] DataLog { get; private set; }

        // Operators
        public SaveFile this[int Index]
        {
            get => StableFiles[Index];
            protected set => StableFiles[Index] = value;
        }



        // Implement IGameManager Interface
        public MonoBehaviour GetManagerClass()
        {
            return this;
        }
        public void InitiateManager(bool AlwaysLoaded)
        {
            Logindex = 0;
            DataLog = new string[5];

            foreach (var file in StableFiles)
            {
                IDataPersistence DataFile = file;
                DataFile.Initiate(this);
                if (LoadOnAwake)
                {
                    if (IsExistFile(file))
                        LoadFile(file);
                }
            }
        }
        public abstract void InitiateService(Service service);


        // Public Functions
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
            IDataPersistence DataFile = file;
            DataFile.Save();
            OnFileSaved?.Invoke(file);
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
            IDataPersistence DataFile = file;
            DataFile.Load();
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

            OnDelete(file);
            IDataPersistence DataFile = file;
            DataFile.Remove();
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
                    DataLog[0] = $"{DateTime.Now} -- {file.name} -- {message}";
                    Logindex++;
                }
                else
                {
                    DataLog[Logindex % DataLog.Length] = $"{DateTime.Now} -- {file.name} -- {message}";
                    Logindex++;
                }
            }
        }


        // Abstract Mehtod
        protected abstract bool IsExist(SaveFile targetfile);
        protected abstract void OnDelete(SaveFile targetfile);
        protected abstract void OnSaveFile(SaveFile targetfile);
        protected abstract void OnLoadFile(SaveFile targetfile);

    }

    public abstract class SaveFile : DataAsset, IDataPersistence
    {

        // Implement IDataPersistence Interface
        void IDataPersistence.Initiate(DataManager manager)
        {
            OnStable(manager);
        }
        void IDataPersistence.Save()
        {
            OnSaved();
        }
        void IDataPersistence.Load()
        {
            OnLoaded();
        }
        void IDataPersistence.Remove()
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