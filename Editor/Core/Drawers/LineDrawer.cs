using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(LineAttribute))]
    public class LineDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            // get a reference to the attribute
            LineAttribute separatorAttribute
                = attribute as LineAttribute;
            // define the line to draw
            Rect separatorRect = new Rect(position.xMin,
                position.yMin + separatorAttribute.Spacing,
                position.width,
                separatorAttribute.Height);
            // draw it
            Color lineColor = EditorGUIUtility.isProSkin
                        ? new Color(0.2f, 0.2f, 0.2f, 1)
                        : new Color(0.7f, 0.7f, 0.7f, 1);

            EditorGUI.DrawRect(separatorRect, lineColor);
        }

        public override float GetHeight()
        {
            LineAttribute separatorAttribute
                = attribute as LineAttribute;

            float totalSpacing = separatorAttribute.Spacing
                + separatorAttribute.Height
                + separatorAttribute.Spacing;

            return totalSpacing;
        }
    }
}