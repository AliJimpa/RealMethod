using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// <code>
    /// public bool Variable;
    /// [ConditionalHide("Variable", true, false)]
    /// public String TargetName;
    /// </code>
    /// conditionalSourceField,hideInInspector, reverceCondition
    /// </summary>
    public class ConditionalHideAttribute : PropertyAttribute
    {
        public string ConditionalSourceField;
        public bool HideInInspector;
        public bool ReverceCondition;

        public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector = false, bool reverceCondition = false)
        {
            ConditionalSourceField = conditionalSourceField;
            HideInInspector = hideInInspector;
            ReverceCondition = reverceCondition;
        }
    }
}