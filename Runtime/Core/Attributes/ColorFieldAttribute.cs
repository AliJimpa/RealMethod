using UnityEngine;
namespace RealMethod
{
    /// <summary>
    /// <code>
    /// [ColorFieldAttribute(R,G,B)]
    /// public GameObject Target;
    /// </code>
    /// </summary>
    public class ColorFieldAttribute : PropertyAttribute
    {
        public Color color = Color.blue;

        public ColorFieldAttribute(float r, float g, float b)
        {
            color = new Color(r, g, b);
        }
    }
}