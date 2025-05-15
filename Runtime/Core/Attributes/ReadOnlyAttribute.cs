using UnityEditor;
using UnityEngine;

namespace RealMethod
{

    public class ReadOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;  // Disable the GUI
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;   // Enable the GUI
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }

#endif
}

// --------Use
// [ReadOnly] public int readOnlyValue = 42;
