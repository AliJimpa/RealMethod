#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RealMethod
{
    public class ShowOnlyAttribute : PropertyAttribute
    {
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
    public class ShowOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            string valueStr;

            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    valueStr = prop.intValue.ToString();
                    break;
                case SerializedPropertyType.Boolean:
                    valueStr = prop.boolValue.ToString();
                    break;
                case SerializedPropertyType.Float:
                    valueStr = prop.floatValue.ToString("0.00000");
                    break;
                case SerializedPropertyType.String:
                    valueStr = prop.stringValue;
                    break;
                default:
                    valueStr = "(not supported)";
                    break;
            }

            EditorGUI.LabelField(position, label.text, valueStr);
        }
    }
#endif
}

// Use
// public class MyClass : MonoBehaviour
// {
//     [ShowOnly] public float aaa = 123.45678f;
//     [ShowOnly] public int bbb = 234;
//     [ShowOnly] public bool ccc = false;
//     [ShowOnly] [SerializeField] bool ddd = true;
// }