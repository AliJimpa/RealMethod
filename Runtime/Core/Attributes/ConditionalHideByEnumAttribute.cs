using UnityEngine;


namespace RealMethod
{
    /// <summary>
    /// Example:
    /// public enum DisplayMode
    /// {
    ///     Mode1,
    ///     Mode2,
    ///     Mode3
    /// }
    /// 
    /// public DisplayMode displayMode;
    /// 
    /// [HideInInspectorByEnum("displayMode", 0)]
    /// public int DisplaySize_Mode_1;
    /// </summary>
    public class ConditionalHideByEnumAttribute : PropertyAttribute
    {
        public string EnumFieldName { get; private set; }
        public int EnumValue { get; private set; }

        public ConditionalHideByEnumAttribute(string enumFieldName, int enumValue)
        {
            EnumFieldName = enumFieldName;
            EnumValue = enumValue;
        }
    }
}