using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// Use this property on a ScriptableObject type to allow the editors drawing the field to draw an expandable
    /// area that allows for changing the values on the object without having to change editor.
    /// <code>
    /// [Expandable]
    /// public ScriptableObject TEST;
    /// </code>
    /// </summary>
    public sealed class AssetInlinAttribute : PropertyAttribute
    {
        public AssetInlinAttribute()
        {
        }
    }
}