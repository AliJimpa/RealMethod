using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RealMethod
{
    public abstract class FileManager : MonoBehaviour, IGameManager
    {
        protected class FileData
        {
            public FieldInfo[] Fields { get; private set; }
            public PropertyInfo[] Properties { get; private set; }

            public FileData(object Instance, BindingFlags flags)
            {
                System.Type type = Instance.GetType();
                Fields = type.GetFields(flags);
                Properties = type.GetProperties(flags);
            }
        }
        protected Dictionary<IFile, FileData> FileList = new Dictionary<IFile, FileData>();

        public event System.Action<IFile> OnFileAdded;
        public event System.Action<IFile> OnFileRemoved;

        // Implement IGameManager Interface
        MonoBehaviour IGameManager.GetManagerClass()
        {
            return this;
        }
        public virtual void InitiateManager(bool AlwaysLoaded)
        {

        }
        public virtual void ResolveService(Service service, bool active)
        {

        }

        // Functions
        public bool AddFile(IFile file)
        {
            if (!FileList.ContainsKey(file))
            {
                if (CanAddFile(file))
                {
                    FileList.Add(file, CreateFileData(file));
                    OnFileAdded?.Invoke(file);
                    return true;
                }
                else
                {
                    Debug.LogWarning($"Can't Add file with name: {file.Name}");
                    return false;
                }
            }
            else
            {
                Debug.LogWarning($"There is a file with this name: {file.Name}");
                return false;
            }
        }
        public bool RemoveFile(IFile file)
        {
            if (FileList.ContainsKey(file))
            {
                FileList.Remove(file);
                OnFileRemoved?.Invoke(file);
                return true;
            }
            else
            {
                Debug.LogWarning($"Can't find any file with this name: {file.Name}");
                return false;
            }
        }
        public bool IsValidFile(IFile file)
        {
            return FileList.ContainsKey(file);
        }
        protected T GetFileData<T>(IFile file) where T : FileData
        {
            return (T)FileList[file];
        }

        // Methods
        protected virtual BindingFlags GetFlagSetting()
        {
            return BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        }
        protected virtual FileData CreateFileData(IFile file)
        {
            return new FileData(file.GetObject(), GetFlagSetting());
        }

        // Abstraction Methods
        protected abstract bool CanAddFile(IFile file);
    }

    public abstract class FileManager<T> : FileManager where T : FileAsset
    {
        [Header("Files")]
        [SerializeField]
        private T[] Files;


        // IGameManager
        public override void InitiateManager(bool AlwaysLoaded)
        {
            base.InitiateManager(AlwaysLoaded);
            foreach (var file in Files)
            {
                AddFile(file);
            }
            Files = null;
        }
    }
}