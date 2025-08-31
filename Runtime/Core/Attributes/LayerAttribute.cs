#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RealMethod
{
    public class LayerAttribute : PropertyAttribute { }


#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    public class LayerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = EditorGUI.LayerField(position, label, property.intValue);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use [Layer] with int.");
            }
        }
    }
#endif




    // ----Use
    // [Dropdown("Option1", "Option2", "Option3")]
    //     [Layer]
    //   public int layer;
}