
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public class CompileRule_Section : ProjectSettingSection
    {
        private class RuleSelector<T> : ArrayTypeSelector<T>
        {

            public RuleSelector(SerializedProperty TargetStringProperty, string displayName) : base(TargetStringProperty, displayName)
            {
            }

            protected override int GetEnforceDisableIndex()
            {
                return CompileGuard.DefaultRuls.Length;
            }
            protected override void DefaultInitiation()
            {
                foreach (var ruletype in CompileGuard.DefaultRuls)
                {
                    int Index = myProperty.arraySize;
                    myProperty.InsertArrayElementAtIndex(Index);
                    SerializedProperty element = myProperty.GetArrayElementAtIndex(Index);
                    element.stringValue = ruletype.AssemblyQualifiedName;
                }
            }
        }

        private ProjectSettingAsset SettingAsset;
        private SerializedObject projectSettings;
        private RuleSelector<CompileRule> Rules;


        // Implement ProjectSettingSection Methods
        protected override void Initialized()
        {

        }
        protected override string GetTitle()
        {
            return "CompileRules";
        }
        protected override SectionType GetSectionType()
        {
            return SectionType.Editor;
        }
        protected override void BeginRender(ProjectSettingAsset Storage)
        {
            SettingAsset = Storage;
            projectSettings = new SerializedObject(Storage);
            Rules = new RuleSelector<CompileRule>(projectSettings.FindProperty("Rules"), "Rules");
        }
        protected override void UpdateRender()
        {
            if (projectSettings == null) return;

            projectSettings.Update();
            Rules.Draw();

            if (GUI.changed)
            {
                projectSettings.ApplyModifiedProperties();
                EditorUtility.SetDirty(SettingAsset); // Mark ScriptableObject dirty
                AssetDatabase.SaveAssets();     // Optional: saves to disk immediately
                AssetDatabase.Refresh();
            }

            projectSettings.ApplyModifiedProperties();

        }


        protected override void Fix(int Id)
        {

        }

    }
}