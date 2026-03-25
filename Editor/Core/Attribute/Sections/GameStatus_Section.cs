
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public class GameStatus_Section : ProjectSettingSection
    {
        private ProjectSettingAsset settings;
        static List<string> names = new List<string>();


        protected override string GetTitle()
        {
            return "GameStatus";
        }
        protected override SectionType GetSectionType()
        {
            return SectionType.Editor;
        }
        protected override void Initialized()
        {

        }
        protected override void BeginRender(ProjectSettingAsset Storage)
        {
            settings = Storage;
            if (settings != null && names.Count == 0)
                names.AddRange(settings.GetStatus());
        }
        protected override void UpdateRender()
        {

            EditorGUI.BeginChangeCheck();

            int newSize = Mathf.Max(0, EditorGUILayout.IntField("Size", names.Count));

            while (names.Count < newSize)
                names.Add("");

            while (names.Count > newSize)
                names.RemoveAt(names.Count - 1);

            for (int i = 0; i < names.Count; i++)
            {
                names[i] = EditorGUILayout.TextField($"Element {i}", names[i]);
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (settings != null)
                {
                    Undo.RecordObject(settings, "Modify Game Status Names");

                    settings.SetStatus(names.ToArray());

                    EditorUtility.SetDirty(settings);
                }
            }
        }

        protected override void Fix(int Id)
        {

        }


    }
}