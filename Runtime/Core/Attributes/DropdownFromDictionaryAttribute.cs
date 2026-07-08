using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// <code>
    /// public Dictionary<string, string> options = new Dictionary<string, string>
    /// {
    ///     { "Key1", "Value1" },
    ///     { "Key2", "Value2" },
    ///     { "Key3", "Value3" }
    /// };
    /// 
    /// [DropdownFromDictionary("options")]
    /// public string selectedOption;
    /// </code>
    /// </summary>
    public sealed class DropdownFromDictionaryAttribute : PropertyAttribute
    {
        public string dictionaryFieldName;
        public DropdownFromDictionaryAttribute(string dictionaryFieldName)
        {
            this.dictionaryFieldName = dictionaryFieldName;
        }
    }
}
