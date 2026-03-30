
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public class GameStatus_Section : ProjectSettingSection
    {
        private ProjectSettingAsset SettingAsset;
        private SerializedObject projectSettings;
        private List<string> MyList
        {
            get
            {
                if (SettingAsset == null)
                {
                    //Error("SettingAsset Can't find");
                    return new List<string>(0);
                }
                return SettingAsset.Status;
            }

            set
            {
                if (SettingAsset != null)
                {
                    SettingAsset.Status = value;
                }
                else
                {
                    //Error("SettingAsset Can't find");
                }
            }
        }


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
            SettingAsset = Storage;
            projectSettings = new SerializedObject(Storage);
        }
        protected override void UpdateRender()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(5);
            int newSize = Mathf.Max(0, EditorGUILayout.IntField("Size", MyList.Count));
            EditorGUILayout.EndHorizontal();

            while (MyList.Count < newSize)
                MyList.Add("");

            while (MyList.Count > newSize)
                MyList.RemoveAt(MyList.Count - 1);

            for (int i = 0; i < MyList.Count; i++)
            {
                MyList[i] = EditorGUILayout.TextField($"Element {i}", MyList[i]);
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (projectSettings != null)
                {
                    Undo.RecordObject(SettingAsset, "Modify Game Status Names");
                    EditorUtility.SetDirty(SettingAsset);
                }
            }

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
            ClearError();
        }


    }
}