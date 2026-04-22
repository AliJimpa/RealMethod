using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomEditor(typeof(SaveManager), true)]
    public class SaveManagerEditor : UnityEditor.Editor
    {
        private SaveManager Component; private bool IsShowLoadedFile = false;

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
                    foreach (var file in files)
                    {
                        EditorGUILayout.LabelField($"{file.Key}({file.GetObject().GetType()})");
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"Total: {files.Length}");
            }

        }


    }

}