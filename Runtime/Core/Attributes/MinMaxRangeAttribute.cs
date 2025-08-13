#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace RealMethod
{
    public class MinMaxRangeAttribute : PropertyAttribute
    {
        public float minLimit;
        public float maxLimit;

        public MinMaxRangeAttribute(float minLimit, float maxLimit)
        {
            this.minLimit = minLimit;
            this.maxLimit = maxLimit;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
    public class MinMaxRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                MinMaxRangeAttribute range = (MinMaxRangeAttribute)attribute;

                Vector2 value = property.vector2Value;

                EditorGUI.BeginProperty(position, label, property);

                // Draw label
                position = EditorGUI.PrefixLabel(position, label);

                // Draw min-max slider
                float min = value.x;
                float max = value.y;

                float sliderWidth = position.width * 0.6f;
                float fieldWidth = (position.width - sliderWidth) / 2f;

                Rect minFieldRect = new Rect(position.x, position.y, fieldWidth - 2, position.height);
                Rect sliderRect = new Rect(position.x + fieldWidth, position.y, sliderWidth, position.height);
                Rect maxFieldRect = new Rect(position.x + fieldWidth + sliderWidth + 2, position.y, fieldWidth - 2, position.height);

                min = EditorGUI.FloatField(minFieldRect, min);
                EditorGUI.MinMaxSlider(sliderRect, ref min, ref max, range.minLimit, range.maxLimit);
                max = EditorGUI.FloatField(maxFieldRect, max);

                // Clamp values
                min = Mathf.Clamp(min, range.minLimit, max);
                max = Mathf.Clamp(max, min, range.maxLimit);

                property.vector2Value = new Vector2(min, max);

                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use MinMaxRange with Vector2");
            }
        }
    }
#endif

}


//  [MinMaxRange(0f, 100f)]
// public MinMaxFloat myRange;