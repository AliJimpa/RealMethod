using System;
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
    /// [ConditionalShowByEnum("exampleEnum", ExampleEnum.Option1, ExampleEnum.Option3)]
    /// public string visibleOnlyForOption1And3;
    /// [ConditionalShowByEnum("exampleEnum", ExampleEnum.Option2)]
    /// public int visibleOnlyForOption2;
    /// </code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ConditionalShowByEnumAttribute : PropertyAttribute
    {
        public string EnumFieldName { get; }
        public object[] ShowValues { get; }

        public ConditionalShowByEnumAttribute(string enumFieldName, params object[] showValues)
        {
            EnumFieldName = enumFieldName;
            ShowValues = showValues;
        }
    }
}
