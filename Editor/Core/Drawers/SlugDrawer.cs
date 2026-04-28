#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SlugAttribute))]
public class SlugDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SlugAttribute attr = (SlugAttribute)attribute;

        // Draw label normally
        position = EditorGUI.PrefixLabel(position, label);

        // Shrink width
        position.width *= attr.WidthScale;

        // Draw the text field
        EditorGUI.PropertyField(position, property, GUIContent.none);
    }
}
#endif
