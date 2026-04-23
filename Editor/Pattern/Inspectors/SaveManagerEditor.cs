using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomEditor(typeof(SaveManager), true)]
    public class SaveManagerEditor : UnityEditor.Editor
    {
        private SaveManager Component;
        private bool IsShowLoadedFile = true;

        private void OnEnable()
        {
            Component = (SaveManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            IsShowLoadedFile = EditorGUILayout.Foldout(IsShowLoadedFile, "Files", true, EditorStyles.foldoutHeader);
            if (IsShowLoadedFile)
            {
                IFile[] files = Component.GetAllFiles();
                if (Component != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        EditorGUILayout.LabelField($"{i}.[{files[i].Key}]: {files[i].GetObject().GetType()}");
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"Total: {files.Length}");
            }

        }


    }

}