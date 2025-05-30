using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RealMethod
{
    public enum FieldColor
    {
        Red,
        Green,
        Blue,
        Yellow,
        Black,
        White,
        Gray
    }
    public class ColorFieldAttribute : PropertyAttribute
    {
        public Color color;

        public ColorFieldAttribute(FieldColor _color = FieldColor.Red)
        {
            switch (_color)
            {
                case FieldColor.Red:
                    color = Color.red;
                    break;
                case FieldColor.Green:
                    color = Color.green;
                    break;
                case FieldColor.Blue:
                    color = Color.blue;
                    break;
                case FieldColor.Yellow:
                    color = Color.yellow;
                    break;
                case FieldColor.Black:
                    color = Color.black;
                    break;
                case FieldColor.White:
                    color = Color.white;
                    break;
                case FieldColor.Gray:
                    color = Color.gray;
                    break;
                default:
                    color = Color.red;
                    break;
            }
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ColorFieldAttribute))]
    public class RequiredFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ColorFieldAttribute field = attribute as ColorFieldAttribute;

            if (property.objectReferenceValue == null)
            {
                GUI.color = field.color; //Set the color of the GUI
                EditorGUI.PropertyField(position, property, label); //Draw the GUI
                GUI.color = Color.white; //Reset the color of the GUI to white
            }
            else
                EditorGUI.PropertyField(position, property, label);
        }
    }
#endif
}

///Use
///[ColorFieldAttribute(FieldColor.Blue)]
///public GameObject Test;