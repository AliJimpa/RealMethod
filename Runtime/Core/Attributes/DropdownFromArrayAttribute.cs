using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// <code>
    /// public string[] options = new string[] { "Option1", "Option2", "Option3" };
    /// 
    /// [StaticDropdown("options")]
    /// public string selectedOption;
    /// </code>
    /// </summary>
    public sealed class DropdownFromArrayAttribute : PropertyAttribute
    {
        public string TargetArray;
        public DropdownFromArrayAttribute(string array)
        {
            TargetArray = array;
        }
    }
}
