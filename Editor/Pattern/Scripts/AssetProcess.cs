using UnityEngine;
using UnityEditor;


namespace RealMethod.Editor
{
    public abstract class AssetProcess
    {
        public AssetProcess()
        {
            Initialized();
        }

        protected abstract void Initialized();
        public abstract void OnAssetImported(string AssetPath);
        public abstract void OnAssetDeleted(string AssetPath);
        public abstract void OnAssetMoved(string AssetPath, string FromPath);
        public abstract void OnAssetClick(string AssetPath, Event e);
        protected abstract string GetIconPath();
    }
    public abstract class AssetProcess<T, J> : AssetProcess where T : Object
    {
        protected abstract void DoubleClick(T asset);

        public override void OnAssetImported(string AssetPath)
        {
            T loadedAsset;
            if (TryLoadAsset(AssetPath, out loadedAsset))
            {
                if (loadedAsset.GetType() == typeof(J))
                {
                    // Load your icon from Resources (adjust the path as needed)
                    Texture2D icon = Resources.Load<Texture2D>(GetIconPath());
                    if (icon != null)
                    {
                        EditorGUIUtility.SetIconForObject(loadedAsset, icon);
                    }
                    else
                    {
                        Debug.LogWarning("Custom icon not found. Please ensure the path and file are correct.");
                    }
                }
            }
            else
            {
                Debug.LogWarning("Cant Load ");
            }
        }
        public override void OnAssetClick(string AssetPath, Event e)
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(AssetPath);

            if (asset != null)
            {
                DoubleClick(asset);
                e.Use(); // Consume the event
            }
        }

        protected bool TryLoadAsset<K>(string assetPath, out K asset) where K : Object
        {
            asset = AssetDatabase.LoadAssetAtPath<K>(assetPath);
            return asset != null;
        }
    }

}