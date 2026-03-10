using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(FName))]
    public class FName_Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty idProp = property.FindPropertyRelative("id");

            if (idProp == null)
            {
                EditorGUI.LabelField(position, label.text, "id field not found");
                return;
            }

            string current = FName.GetString(idProp.intValue);

            EditorGUI.BeginProperty(position, label, property);

            string newValue = EditorGUI.DelayedTextField(position, label, current);

            if (newValue != current)
            {
                idProp.intValue = FName.GetOrCreateId(newValue);
            }



            // --- Set GUI color to blue to indicate change ---
            // Color prevColor = GUI.color;
            // GUI.color = Color.gray; // light blue
            // EditorGUI.TextField(position, label, current);
            // GUI.color = prevColor;
            // EditorGUI.EndProperty();
        }
    }

}