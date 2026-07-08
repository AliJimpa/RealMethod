using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RealMethod
{
    [Serializable]
    public class SceneAsset : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        public UnityEditor.SceneAsset Asset; // Only in editor
#endif
        public string ScenePath;
        public string ScneName => System.IO.Path.GetFileNameWithoutExtension(ScenePath);


        // Implement ISerializationCallbackReceiver Interface
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            UpdateScenePath();
#endif
        }
        public void OnAfterDeserialize()
        {
        }

        // Implicit conversion to string
        public static implicit operator string(SceneAsset sceneReference)
        {
            return sceneReference?.ScenePath;
        }


#if UNITY_EDITOR
        private void UpdateScenePath()
        {
            if (Asset != null)
            {
                string newPath = AssetDatabase.GetAssetPath(Asset);
                if (ScenePath != newPath)
                {
                    ScenePath = newPath;
                    EditorUtility.SetDirty(Selection.activeObject);
                }
            }
            else
            {
                ScenePath = string.Empty;
            }
        }
#endif


    }
}



