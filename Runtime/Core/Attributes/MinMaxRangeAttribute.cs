using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// <code>
    /// [MinMaxRange(0f, 100f)]
    /// public Vector2 myRange;
    /// </code>
    /// </summary>
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
}