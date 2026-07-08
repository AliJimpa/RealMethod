using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RealMethod
{
    public interface ISaveFile : IFile, ISaveable
    {
        void SetName(string NewName);
    }

    public abstract class SaveFileAsset : FileAsset, ISaveFile
    {
        // Implement ISave Interface
        void ISaveable.OnLoaded()
        {
            OnLoaded();
        }
        void ISaveable.OnSaved()
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
            IStorageService saveSystem = Game.GetService<IStorageService>();
            if (saveSystem != null)
            {
                if (!saveSystem.HasFile(this))
                    saveSystem.AddFile(this);
            }else
            {
                Debug.LogError("Can't find SaveSystem");
            }
#endif
        }
        protected virtual void OnDisable()
        {
#if !UNITY_EDITOR
            IStorageService saveSystem = Game.GetService<IStorageService>();
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
            IStorageService saveSystem = Game.GetService<IStorageService>();
            if (saveSystem != null)
            {
                if (!saveSystem.HasFile(this))
                    saveSystem.AddFile(this);
            }
            else
            {
                Debug.LogError("Can't find SaveSystem");
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
            var manager = Game.GetService<IStorageService>();
            if (manager != null)
            {
                manager.Save(this);
            }
            else
            {
                Debug.LogError($"No {typeof(IStorageService)} found in the scene.");
            }
        }
        [ContextMenu("Load")]
        private void Editor_LoadSelf()
        {
            var manager = Game.GetService<IStorageService>();
            if (manager != null)
            {
                manager.Load(this);
            }
            else
            {
                Debug.LogError($"No {typeof(IStorageService)} found in the scene.");
            }
        }
        [ContextMenu("Delete")]
        private void Editor_Delete()
        {
            var manager = Game.GetService<IStorageService>();
            if (manager != null)
            {
                manager.Delete(this);
            }
            else
            {
                Debug.LogError($"No {typeof(IStorageService)} found in the scene.");
            }
        }
        [ContextMenu("IsExist")]
        private void Editor_IsExist()
        {
            var manager = Game.GetService<IStorageService>();
            if (manager != null)
            {
                Debug.Log($"[{name}] IsExist: {manager.IsExist(this)}");
            }
            else
            {
                Debug.LogError($"No {typeof(IStorageService)} found in the scene.");
            }
        }
#endif
    }
}