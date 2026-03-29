using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(ColorFieldAttribute))]
    public class RequiredFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ColorFieldAttribute field = attribute as ColorFieldAttribute;

            GUI.color = field.color; //Set the color of the GUI
            EditorGUI.PropertyField(position, property, label); //Draw the GUI
            GUI.color = Color.white; //Reset the color of the GUI to white

        }
    }
}