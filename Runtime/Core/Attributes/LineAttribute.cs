using UnityEngine;

namespace RealMethod
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class LineAttribute : PropertyAttribute
    {
        public readonly float Height;
        public readonly float Spacing;

        public LineAttribute(float height = 1, float spacing = 10)
        {
            Height = height;
            Spacing = spacing;
        }
    }
}