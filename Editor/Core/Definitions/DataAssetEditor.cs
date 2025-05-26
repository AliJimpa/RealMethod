#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RealMethod
{
    public class DataAssetEditor : Editor
    {
        public static void DrawCompleteScriptableObjectEditor<T>(string name, ref T settings, ref bool foldout, ref Editor editor) where T : ScriptableObject
        {
            GUILayout.BeginVertical("GroupBox");
            settings = (T) EditorGUILayout.ObjectField(name, settings, typeof(T), true);
            DrawSubEditor(settings,ref foldout,ref editor);
            GUILayout.EndVertical();
        }
        public static void DrawSubEditor (Object settings, ref bool foldout, ref Editor editor) {
            if (settings != null) {
                foldout = EditorGUILayout.InspectorTitlebar (foldout, settings);
                if (foldout) {
                    CreateCachedEditor (settings, null, ref editor);
                    editor.OnInspectorGUI ();
                }
            }
        }
    }
}
#endif