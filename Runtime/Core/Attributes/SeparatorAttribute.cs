using UnityEngine;
using UnityEditor;


namespace Name
{

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class SeparatorAttribute : PropertyAttribute
    {
        public readonly float Height;
        public readonly float Spacing;

        public SeparatorAttribute(float height = 1, float spacing = 10)
        {
            Height = height;
            Spacing = spacing;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SeparatorAttribute))]
    public class SeparatorDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            // get a reference to the attribute
            SeparatorAttribute separatorAttribute
                = attribute as SeparatorAttribute;
            // define the line to draw
            Rect separatorRect = new Rect(position.xMin,
                position.yMin + separatorAttribute.Spacing,
                position.width,
                separatorAttribute.Height);
            // draw it
            EditorGUI.DrawRect(separatorRect, Color.white);
        }

        public override float GetHeight()
        {
            SeparatorAttribute separatorAttribute
                = attribute as SeparatorAttribute;

            float totalSpacing = separatorAttribute.Spacing
                + separatorAttribute.Height
                + separatorAttribute.Spacing;

            return totalSpacing;
        }
    }
#endif
}