using UnityEngine;
using System;

namespace RealMethod
{
    public abstract class DataManager : MonoBehaviour, IGameManager
    {

        [Header("SaveFile")]
        [SerializeField]
        private bool AutoLoad = true;
        [SerializeField] private SaveFile file;
        public SaveFile File => file;


        public Action<SaveFile> OnFileChanged;
        public Action<SaveFile> OnFileLoaded;
        public Action<SaveFile> OnFileSaved;


        public MonoBehaviour GetManagerClass()
        {
            return this;
        }
        public void InitiateManager(bool AlwaysLoaded)
        {
            if (File)
            {
                File.OnActiveted(this);
                if (AutoLoad)
                {
                    if (IsExistFile())
                        LoadFile();
                }
            }
        }
        public abstract void InitiateService(Service service);



        [ContextMenu("SaveFile")]
        public void SaveFile()
        {
            if (File == null)
            {
                Debug.LogError("File is not valid");
                return;
            }
            OnSaveFile(File);
            File.OnSaved();
            OnFileSaved?.Invoke(File);
        }
        [ContextMenu("LoadFile")]
        public void LoadFile()
        {
            if (File == null)
            {
                Debug.LogError("File Does not valid");
                return;
            }

            OnLoadFile(File);
            File.OnLoaded();
            OnFileLoaded?.Invoke(File);
        }
        [ContextMenu("DeleteFilew")]
        public void DeleteFile()
        {
            if (File == null)
            {
                Debug.LogError("File Does not valid");
                return;
            }

            OnDelete(File);
            File.OnDeleted();
        }
        public bool IsExistFile()
        {
            if (File == null)
            {
                Debug.LogError("File Does not valid");
                return false;
            }
            return IsExist(File);
        }
        public bool SetFile(SaveFile newfile)
        {
            if (newfile)
            {
                if (File != newfile)
                {
                    File?.OnDiactiveted(this);
                    file = newfile;
                    File.OnActiveted(this);
                    OnFileChanged?.Invoke(File);
                }
                return true;
            }
            else
            {
                Debug.LogError("File Does not valid");
                return false;
            }
        }


        // Abstract Mehtod
        protected abstract bool IsExist(SaveFile targetfile);
        protected abstract void OnDelete(SaveFile targetfile);
        protected abstract void OnSaveFile(SaveFile targetfile);
        protected abstract void OnLoadFile(SaveFile targetfile);

    }

    public abstract class DataManager<T> : DataManager where T : Enum
    {
        [Header("FileSystem")]
        [SerializeField] private T method;
        public T Method => method;
    }

    public abstract class SaveFile : ScriptableObject
    {
        public Action<SaveFile> OnModify;
        public abstract void OnActiveted(DataManager manager);
        public abstract void OnLoaded();
        public abstract void OnSaved();
        public abstract void OnDeleted();
        public abstract void Default();
        public abstract void OnDiactiveted(DataManager manager);

        [ContextMenu("ResetToDefault")]
        private void BacktoDefault()
        {
            Default();
        }

    }

}