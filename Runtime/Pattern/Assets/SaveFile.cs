using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// this is FileAsset that implement ISave Interface with some Editor Function
    /// for testing save and load
    /// </summary>
    public abstract class SaveFile : FileAsset, ISave
    {
        // Implement ISave Interface
        void ISave.OnLoaded()
        {
            OnLoaded();
        }
        void ISave.OnSaved()
        {
            OnSaved();
        }

        protected abstract void OnLoaded();
        protected abstract void OnSaved();

#if UNITY_EDITOR
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
#endif
    }
}