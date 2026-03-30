using System;
using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// Example:
    /// <code>
    /// public enum MyEnumWithDescriptions
    /// {
    ///     [EnumDescription("This is the first option.")]
    ///     Option1,
    ///     [EnumDescription("This is the second option.")]
    ///     Option2,
    ///     [EnumDescription("This is the third option.")]
    ///     Option3
    /// }
    /// </code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DescriptionEnumAttribute : PropertyAttribute
    {
        public string Description { get; }
        public DescriptionEnumAttribute(string description)
        {
            Description = description;
        }
    }
}

