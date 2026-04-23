using UnityEngine;


namespace RealMethod
{
    /// <summary>
    /// <code>
    /// public enum ExampleEnum
    /// {
    ///     Option1,
    ///     Option2,
    ///     Option3
    /// }
    /// 
    /// public ExampleEnum exampleEnum;
    /// [ConditionalHideByEnum("exampleEnum", ExampleEnum.Option1, ExampleEnum.Option3)]
    /// public string hideOnlyForOption1And3;
    /// [ConditionalHideByEnum("exampleEnum", ExampleEnum.Option2)]
    /// public int hideOnlyForOption2;
    /// </code>
    /// </summary>
    public class ConditionalHideByEnumAttribute : PropertyAttribute
    {
        public string EnumFieldName;
        public object[] HideValues;

        public ConditionalHideByEnumAttribute(string enumFieldName, params object[] hideValues)
        {
            EnumFieldName = enumFieldName;
            HideValues = hideValues;
        }
    }
}