using UnityEditor;
using System.Reflection;

namespace RealMethod.Editor
{
    [CustomEditor(typeof(ResourcAsset<>), true)]
    public class ResourcEditor : UnityEditor.Editor
    {
        private object baseComponent;
        private System.Type targetType;
        private System.Type genericType;
        private FieldInfo pathField;

        private void OnEnable()
        {
            baseComponent = target;
            targetType = baseComponent.GetType();
            genericType = targetType.BaseType.GetGenericArguments()[0];

            pathField = targetType.GetField("ResourcePath", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            //UnityEngine.Debug.Log(pathField != null ? pathField.Name : "kiir");
        }

        public override void OnInspectorGUI()
        {
            // Load asset from path if valid
            string path = pathField?.GetValue(baseComponent) as string;
            UnityEngine.Object currentAsset = null;

            if (!string.IsNullOrEmpty(path))
                currentAsset = AssetDatabase.LoadAssetAtPath(path, genericType);

            // Display object field
            UnityEngine.Object newAsset = EditorGUILayout.ObjectField("Asset", currentAsset, genericType, false);

            // If user changed the object, update fields
            if (newAsset != currentAsset)
            {
                string newPath = AssetDatabase.GetAssetPath(newAsset);

                if (!string.IsNullOrEmpty(newPath))
                {
                    pathField?.SetValue(baseComponent, newPath);
                    EditorUtility.SetDirty((UnityEngine.Object)baseComponent);
                }
                else
                {
                    if (path != string.Empty)
                        pathField?.SetValue(baseComponent, string.Empty);
                    EditorUtility.SetDirty((UnityEngine.Object)baseComponent);
                }
            }

            EditorGUILayout.LabelField("Details");
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }
    }
}
