using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public class LibraryViewerWindow : EditorWindow
    {
        private class FieldModule
        {
            //private Vector2 scroll;
            private MonoScript File;


            public Type Class { get; private set; }
            public MethodInfo TargetMethod { get; private set; }
            public string Title
            {
                get
                {
                    return $"{Class.Name}.{TargetMethod.Name}";
                }
            }
            public bool IsExpand { get; private set; } = false;
            public bool isStatic => Class.IsAbstract && Class.IsSealed;

            public FieldModule(MonoScript script, MethodInfo method)
            {
                File = script;

                if (method == null)
                    Print("Method is not valid!");
                TargetMethod = method;

                Type ClassType = script.GetClass();
                if (ClassType == null)
                    Print("Class is not valid!");
                Class = ClassType;
            }

            public void Render()
            {
                EditorGUILayout.BeginHorizontal();
                IsExpand = EditorGUILayout.Foldout(IsExpand, $"{Title}({TargetMethod.GetParameters().Length})", true);
                if (GUILayout.Button("OpenFile  ", EditorStyles.linkLabel))
                {
                    Debug.Log("sdsdsd");
                    AssetDatabase.OpenAsset(File);
                }
                EditorGUILayout.EndHorizontal();
                if (IsExpand)
                {
                    //EditorGUI.BeginDisabledGroup(true);
                    //scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(800));
                    EditorGUILayout.TextArea(GetMethodBody(File, TargetMethod));
                    //EditorGUILayout.EndScrollView();
                    //EditorGUI.EndDisabledGroup();
                }

            }

            private string GetMethodBody(MonoScript script, MethodInfo method)
            {
                string path = AssetDatabase.GetAssetPath(script);
                if (string.IsNullOrEmpty(path))
                {
                    return "Invalid script path.";
                }
                string[] lines = System.IO.File.ReadAllLines(path);

                StringBuilder sb = new StringBuilder();

                int MethodLineNumber = -1;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains(method.Name))
                    {
                        MethodLineNumber = i;
                        break;
                    }
                }

                if (MethodLineNumber == -1)
                {
                    return "Something wrong!";
                }

                for (int i = MethodLineNumber; i < MethodLineNumber + 10; i++)
                {
                    sb.AppendLine(lines[i]);
                }
                return sb.ToString();
            }
            private void Print(string message)
            {
                Debug.Log($"[{Title}]: {message}");
            }
        }
        private class Dropdown : EditorProperty<int>
        {
            private string[] Options;
            public string CurrentOption => Options[GetValue()];

            public Dropdown(string Name, string[] options, UnityEngine.Object other) : base(Name, other)
            {
                Options = options;
            }

            protected override byte UpdateRender()
            {
                CashValue = EditorGUILayout.Popup(string.Empty, CurrentValue, Options, GUI.skin.FindStyle("ToolbarPopup"), GUILayout.Width(120));
                if (CashValue == CurrentValue)
                {
                    return 0; // No change
                }
                else
                {
                    SetValue(CashValue);
                    return 1; // Changed
                }
            }
            protected override void FixError(int Id)
            {
                throw new NotImplementedException();
            }
        }


        private Vector2 scroll;
        private Dropdown NameSpace_Filter;
        private MonoScript[] Scripts;
        private List<FieldModule> Fields = new List<FieldModule>(0);
        private string search = string.Empty;


        [MenuItem("Tools/RealMethod/Viewer/LibraryViewer")]
        public static void Open()
        {
            GetWindow<LibraryViewerWindow>("LibraryViewer");
        }
        private void OnEnable()
        {

            Scripts = LoadScripts();
            CreateNameSpaceList();
            ReSetField();
        }
        private void OnGUI()
        {
            DrawToolbar();
            scroll = EditorGUILayout.BeginScrollView(scroll);
            foreach (var field in Fields)
            {
                if (!PassSearch(field)) continue;
                if (!PassFilter(field)) continue;

                field.Render();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.HelpBox(
            " Note: This inspector uses reflection and can only detect the primary type in this script and its nested types. " +
            "Additional classes, structs, enums, or interfaces declared outside the main type in the same file will not appear.",
            MessageType.Info);
        }


        private void ReSetField()
        {
            Fields.Clear();


            foreach (var script in Scripts)
            {
                Type mainType = script.GetClass();
                if (mainType == null)
                    continue;

                if (mainType.IsAbstract && mainType.IsSealed)
                {
                    DrawField(script,mainType);
                    Type[] nestedTypes = mainType.GetNestedTypes(BindingFlags.Public);
                    foreach (var classtype in nestedTypes)
                    {
                        DrawField(script,classtype);
                    }
                }

            }
        }
        private void DrawField(MonoScript script, Type mainType)
        {
            BindingFlags flags =
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.Static |
            BindingFlags.DeclaredOnly;

            foreach (var method in mainType.GetMethods(flags))
            {
                if (method.IsSpecialName) continue;

                if (!method.IsPublic) continue;

                if (!method.IsStatic) continue;

                if (method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)) continue;

                Fields.Add(new FieldModule(script, method));
            }
        }
        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            search = GUILayout.TextField(search, GUI.skin.FindStyle("ToolbarSearchTextField"), GUILayout.ExpandWidth(true));

            if (GUILayout.Button(GUIContent.none, GUI.skin.FindStyle("ToolbarSearchCancelButton")))
            {
                search = "";
                GUI.FocusControl(null);
            }

            if (NameSpace_Filter.Render() == 1)
                ReSetField();

            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(70)))
            {
                search = "";
                Scripts = LoadScripts();
                CreateNameSpaceList();
                ReSetField();
            }

            EditorGUILayout.EndHorizontal();
        }
        private bool PassSearch(FieldModule Field)
        {
            if (string.IsNullOrEmpty(search)) return true;

            return Field.Title.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        private bool PassFilter(FieldModule Field)
        {
            if (NameSpace_Filter.GetValue() == 0)
                return true;

            if (Field.Class.Namespace == NameSpace_Filter.CurrentOption)
                return true;

            return false;
        }
        private void CreateNameSpaceList()
        {
            int RealMethodIndex = 0;
            HashSet<string> NameSpaceList = new HashSet<string>() { "All" };
            foreach (var script in Scripts)
            {
                if (script == null)
                    continue;

                Type ClassType = script.GetClass();
                if (ClassType != null)
                {
                    if (!string.IsNullOrEmpty(ClassType.Namespace))
                    {
                        if (NameSpaceList.Add(ClassType.Namespace))
                        {
                            if (ClassType.Namespace == "RealMethod")
                            {
                                RealMethodIndex = NameSpaceList.Count - 1;
                            }
                        }
                    }
                }
            }
            // Set NameSpace Lisst to Dropdown box
            NameSpace_Filter = new Dropdown("Filter", NameSpaceList.ToArray(), this);
            NameSpace_Filter.SetValue(RealMethodIndex);
        }
        private MonoScript[] LoadScripts()
        {
            List<MonoScript> scripts = new List<MonoScript>();

            string[] guids = AssetDatabase.FindAssets("t:MonoScript");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                scripts.Add(script);
            }

            scripts.Sort((a, b) => a.name.CompareTo(b.name));

            return scripts.ToArray();
        }

    }
}