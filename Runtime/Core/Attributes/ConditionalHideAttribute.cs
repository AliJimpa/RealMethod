using UnityEngine;
using UnityEditor;

namespace RealMethod
{
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

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
    public class ConditionalHidePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
            bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

            bool wasEnabled = GUI.enabled;
            GUI.enabled = enabled;

            if (!condHAtt.HideInInspector || enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }

            GUI.enabled = wasEnabled;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
            bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

            if (!condHAtt.HideInInspector || enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
        {
            bool enabled = true;

            string propertyPath = property.propertyPath; // returns the property path of the property we want to apply the attribute to
            string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField); // changes the path to the conditionalsource property path
            SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

            if (sourcePropertyValue != null)
            {
                if (!condHAtt.ReverceCondition)
                {
                    enabled = sourcePropertyValue.boolValue;
                }
                else
                {
                    enabled = !sourcePropertyValue.boolValue;
                }

            }
            else
            {
                Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
            }

            return enabled;
        }
    }

#endif

}

//Use
// [ConditionalHide("IsPlayerInScene", true, false)]
// First variable : Your boolean variable Name 
// Second variable : if true= Show/Hide false= Visible/Invisible
//  Third variable : if true= revert functionality
