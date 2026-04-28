using UnityEditor;
using UnityEngine;

namespace RealMethod
{
    // FILE
    public interface IFile : IIdentifier
    {
        string FileName { get; }
        object FileObject { get; }
    }
    public interface ISaveFile : IFile, ISave
    {
        void SetName(string NewName);
    }



    // SAVE
    public interface ISaveSystem
    {
        /// <summary>
        /// Checks whether the specified file exists in the save storage.
        /// </summary>
        bool IsExist(IFile file);
        /// <summary>
        /// Saves the specified file to the save storage.
        /// </summary>
        void Save(IFile file);
        /// <summary>
        /// Loads the specified file from the save storage into memory.
        /// </summary>
        void Load(IFile file);
        /// <summary>
        /// Deletes the specified file from the save storage.
        /// </summary>
        void Delete(IFile file);
        /// <summary>
        /// Save all file or Specific file that implemented
        /// </summary>
        void SaveAll();
        /// <summary>
        /// Load all file or specific file that implemented
        /// </summary>
        void LoadAll();
        /// <summary>
        /// use for adding file to filelist in savemsystem.
        /// if you want to use saveall or geting file with name
        /// </summary>
        /// <param name="file">target file you want adding</param>
        /// <returns>return true if can add</returns>
        bool AddFile(IFile file);
        /// <summary>
        /// use for removing file to filelist in savesystem
        /// </summary>
        /// <param name="file">target file you want removing</param>
        /// <returns>return true if can remove</returns>
        bool RemoveFile(IFile file);
        /// <summary>
        /// Determines whether the specified file already exists in the file list.
        /// Checks by object reference unless IFile implementations override equality.
        /// </summary>
        /// <param name="file">The file instance to check.</param>
        /// <returns>True if the file is found in the list; otherwise false.</returns>
        bool HasFile(IFile file);
        /// <summary>
        /// The file that created by savesystem for merging all file selected in sytem to one file
        /// </summary>
        IFile MainSaveFile { get; }
    }
    public interface ISave
    {
        void OnLoaded();
        void OnSaved();
    }
    /// <summary>
    /// this is UniqueAsset that implement ISaveFile Interface with some Editor Function
    /// for testing save and load
    /// </summary>
    public abstract class SaveAsset : UniqueAsset, ISaveFile
    {
        // Implement IFile Interface
        string IFile.FileName => name;
        object IFile.FileObject => this;
        // Implement ISave Interface
        void ISave.OnLoaded()
        {
            OnLoaded();
        }
        void ISave.OnSaved()
        {
            OnSaved();
        }
        // Implement ISaveFile Interface
        void ISaveFile.SetName(string NewName)
        {
            Debug.LogWarning($"Can't change Name for Asset({name}). the SaveAsset get asset name for file naming");
        }

        // Unity Method
        protected virtual void OnEnable()
        {
#if !UNITY_EDITOR
            ISaveSystem saveSystem = Game.Instance.SaveSystem;
            if (saveSystem != null)
            {
                if (!saveSystem.HasFile(this))
                    saveSystem.AddFile(this);
            }
#endif
        }
        protected virtual void OnDisable()
        {
#if !UNITY_EDITOR
            ISaveSystem saveSystem = Game.Instance.SaveSystem;
            if (saveSystem != null)
            {
                if (saveSystem.HasFile(this))
                    saveSystem.RemoveFile(this);
            }
            else
            {
                Debug.LogError("Can't find SaveSystem");
            }
#endif
        }
        protected virtual void Reset()
        {
#if UNITY_EDITOR
            ISaveSystem saveSystem = Game.Instance.SaveSystem;
            if (saveSystem != null)
            {
                if (!saveSystem.HasFile(this))
                    saveSystem.AddFile(this);
            }
#endif
        }

        protected abstract void OnLoaded();
        protected abstract void OnSaved();


#if UNITY_EDITOR
        public override bool AutoReset(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
                return true;
            return base.AutoReset(state);
        }
        [ContextMenu("Save")]
        private void Editor_SaveSelf()
        {
            var manager = FindFirstObjectByType<SaveManager>();
            if (manager != null)
            {
                manager.Save(this);
            }
            else
            {
                Debug.LogError("No SaveManager found in the scene.");
            }
        }
        [ContextMenu("Load")]
        private void Editor_LoadSelf()
        {
            var manager = FindFirstObjectByType<SaveManager>();
            if (manager != null)
            {
                manager.Load(this);
            }
            else
            {
                Debug.LogError("No SaveManager found in the scene.");
            }
        }
        [ContextMenu("Delete")]
        private void Editor_Delete()
        {
            var manager = FindFirstObjectByType<SaveManager>();
            if (manager != null)
            {
                manager.Delete(this);
            }
            else
            {
                Debug.LogError("No SaveManager found in the scene.");
            }
        }
        [ContextMenu("IsExist")]
        private void Editor_IsExist()
        {
            var manager = FindFirstObjectByType<SaveManager>();
            if (manager != null)
            {
                Debug.Log($"[{name}] IsExist: {manager.IsExist(this)}");
            }
            else
            {
                Debug.LogError("No SaveManager found in the scene.");
            }
        }
#endif


    }

}