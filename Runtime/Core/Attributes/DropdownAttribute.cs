using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// <code>
    /// Exaple_1:
    /// [Dropdown("Option1", "Option2", "Option3")]
    /// public string selectedOption;
    /// Example_2:
    /// [Dropdown("Option1", "Option2", "Option3")]
    /// public int selectedOptionIndex;
    /// </code>
    /// </summary>
    public class DropdownAttribute : PropertyAttribute
    {
        public string[] options;

        public DropdownAttribute(params string[] options)
        {
            this.options = options;
        }
    }
}