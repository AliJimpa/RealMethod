using System;
using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// <code>
    /// ExampleClass{
    /// public static List<string> options = new List<string> { "Option1", "Option2", "Option3" };
    /// }
    /// 
    /// [ListToPopup(typeof(ExampleClass), "options")]
    /// public string selectedOption;
    /// </code>
    /// </summary>
    public class PopupListAttribute : PropertyAttribute
    {
        public Type myType;
        public string propertyName;

        public PopupListAttribute(Type _myType, string _propertyName)
        {
            myType = _myType;
            propertyName = _propertyName;
        }
    }
}
