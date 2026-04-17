using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public class CompileRuleSection : ProjectSettingSection
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
                    Debug.LogWarning($"====>    {ruletype}");
                }
            }
        }

        private SerializedProperty CompilingSerialized;
        private ProjectSettingAsset SettingAsset;
        private SerializedObject projectSettings;
        private RuleSelector<CompileRule> Rules;


        // Implement ProjectSettingSection Methods
        protected override void Initialized()
        {
        }
        protected override string GetTitle()
        {
            return "CompileGuard";
        }
        protected override SectionType GetSectionType()
        {
            return SectionType.Editor;
        }
        protected override void BeginRender(ProjectSettingAsset Storage)
        {
            SettingAsset = Storage;
            projectSettings = new SerializedObject(Storage);
            CompilingSerialized = projectSettings.FindProperty("Compiling");
            Rules = new RuleSelector<CompileRule>(projectSettings.FindProperty("Rules"), "Rules");
        }
        protected override void UpdateRender()
        {
            if (projectSettings == null) return;

            projectSettings.Update();
            CompilingSerialized.boolValue = EditorGUILayout.Toggle("Enable", CompilingSerialized.boolValue, GUILayout.Width(100));

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