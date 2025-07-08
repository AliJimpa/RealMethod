using UnityEngine;

namespace RealMethod
{
    public abstract class DataAsset : ScriptableObject
    {
        public static bool IsRuntimeClone(ScriptableObject so)
        {
            return so.hideFlags == HideFlags.DontSave || so.name.EndsWith("(Clone)");
        }


#if UNITY_EDITOR
        /// This method runs once at game start, calls OnEditorPlay
        [UnityEditor.InitializeOnEnterPlayMode]
        private static void EditorReset()
        {
            var assets = Resources.FindObjectsOfTypeAll<DataAsset>();

            foreach (var asset in assets)
            {
                if (IsRuntimeClone(asset))
                    continue;

                asset.OnEditorPlay();
            }
        }
        
        /// <summary>
        /// Custom reset method you override in children
        /// (Editor only)
        /// </summary>
        public virtual void OnEditorPlay()
        {
            Debug.Log($"{name} OnEditorPlay called.");
        }
#endif
    }
}
