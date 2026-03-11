using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    class FieldModule
    {
        private MonoScript MyScript;
        private List<FieldModule> MyChilds = new List<FieldModule>();
        private string MyName = string.Empty;



        public string Title
        {
            get
            {
                if (MyName == string.Empty)
                {
                    return MyScript.name;
                }
                else
                {
                    return MyName;
                }
            }
        }
        public MonoScript ScriptFile => MyScript;
        public Type Class => MyScript.GetClass();
        public bool IsExpand { get; private set; } = false;



        public FieldModule(string Name)
        {
            MyName = Name;
        }
        public FieldModule(MonoScript script)
        {
            MyScript = script;
        }


        public void Draw()
        {
            if (MyChilds.Count == 0)
            {
                EditorGUILayout.LabelField(Title);
            }
            else
            {
                IsExpand = EditorGUILayout.Foldout(IsExpand, Title, true);
                if (IsExpand)
                {
                    EditorGUI.indentLevel++;
                    foreach (var child in MyChilds)
                    {
                        child.Draw();
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }
        public void AddChild(MonoScript script)
        {
            FieldModule NewChildField = new FieldModule(script);
            Type NewType = script.GetClass();
            if (NewType == null)
            {
                MyChilds.Add(NewChildField);
                return;
            }
            Type NewParentType = NewType.BaseType;
            if (NewParentType == null)
            {
                MyChilds.Add(NewChildField);
                return;
            }



            // find parent from this child
            foreach (var child in MyChilds)
            {
                Type ChildType = child.Class;
                if (ChildType != null)
                {
                    if (NewParentType == ChildType)
                    {
                        child.AddChild(NewChildField.ScriptFile);
                        return;
                    }
                }
            }

            // Find Child from This Field Child
            List<int> RemoveIndex = new List<int>();
            for (int i = 0; i < MyChilds.Count; i++)
            {
                FieldModule child = MyChilds[i];
                Type ChildType = child.Class;
                if (ChildType != null)
                {
                    Type ChildParentType = ChildType.BaseType;
                    if (ChildParentType != null)
                    {
                        if (NewType == ChildParentType)
                        {
                            RemoveIndex.Add(i);
                            NewChildField.AddChild(child.ScriptFile);
                        }
                    }
                }
            }


            MyChilds.Add(NewChildField);

            foreach (var Index in RemoveIndex)
            {
                if (MyChilds.IsValidIndex(Index))
                {
                    MyChilds.RemoveAt(Index);
                }
                else
                {
                    Debug.LogWarning($"[{Title}] Remove Index is not valid, TargetIndex={Index} , Count={MyChilds.Count}");
                }
            }
        }
    }


    public class ClassViewerWindow : EditorWindow
    {
        private class ComboBox<T> : EditorProperty<T> where T : Enum
        {
            public ComboBox(string Name, UnityEngine.Object other) : base(Name, other)
            {
            }

            protected override byte UpdateRender()
            {
                CashValue = (T)EditorGUILayout.EnumPopup(CurrentValue, GUI.skin.FindStyle("ToolbarPopup"), GUILayout.Width(120));
                if (EqualityComparer<T>.Default.Equals(CashValue, CurrentValue))
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

            }

        }
        private enum FilterType
        {
            All,
            MonoBehaviour,
            ScriptableObject,
            Editor
        }

        private Vector2 scroll;
        private MonoScript[] ProjectScripts;
        private Dictionary<string, FieldModule> NameSpaces = new Dictionary<string, FieldModule>();

        private string search = "";
        private ComboBox<FilterType> filter;
        private bool TEST;


        [MenuItem("Tools/RealMethod/ClassViewer")]
        public static void Open()
        {

            GetWindow<ClassViewerWindow>("Class Viewer");
        }
        private void OnEnable()
        {
            filter = new ComboBox<FilterType>("Filter", this);
            ProjectScripts = LoadScripts();
            foreach (var script in ProjectScripts)
            {
                if (script != null)
                {
                    Type ScriptType = script.GetClass();
                    if (ScriptType != null)
                    {
                        if (ScriptType.Namespace != null)
                        {
                            if (NameSpaces.TryGetValue(ScriptType.Namespace, out FieldModule Field))
                            {
                                Field.AddChild(script);
                            }
                            else
                            {
                                FieldModule NewField = new FieldModule(ScriptType.Namespace);
                                NameSpaces.Add(ScriptType.Namespace, NewField);
                                NewField.AddChild(script);
                            }
                        }
                        else
                        {
                            if (NameSpaces.TryGetValue("NonSpace", out FieldModule Field))
                            {
                                Field.AddChild(script);
                            }
                            else
                            {
                                FieldModule NewField = new FieldModule("NonSpace");
                                NameSpaces.Add("NonSpace", NewField);
                                NewField.AddChild(script);
                            }
                        }
                    }
                    else
                    {
                        if (NameSpaces.TryGetValue("NoType", out FieldModule Field))
                        {
                            Field.AddChild(script);
                        }
                        else
                        {
                            FieldModule NewField = new FieldModule("NoType");
                            NameSpaces.Add("NoType", NewField);
                            NewField.AddChild(script);
                        }
                    }

                }
            }
        }
        private void OnGUI()
        {
            DrawToolbar();
            scroll = EditorGUILayout.BeginScrollView(scroll);
            DrawList();
            EditorGUILayout.EndScrollView();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            search = GUILayout.TextField(
                search,
                GUI.skin.FindStyle("ToolbarSearchTextField"),
                GUILayout.ExpandWidth(true)
            );

            if (GUILayout.Button(
                GUIContent.none,
                GUI.skin.FindStyle("ToolbarSearchCancelButton")))
            {
                search = "";
                GUI.FocusControl(null);
            }
            filter.Render();
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(70)))
            {
                search = "";
                LoadScripts();
            }

            EditorGUILayout.EndHorizontal();
        }
        private void DrawList()
        {
            foreach (var item in NameSpaces)
            {
                item.Value.Draw();
            }
        }
        private void DrawScriptList(ref MonoScript[] Scripts)
        {
            foreach (var script in Scripts)
            {
                if (!PassSearch(script)) continue;
                if (!PassFilter(script)) continue;

                DrawScript(script);
            }
        }



        private bool PassSearch(MonoScript script)
        {
            if (string.IsNullOrEmpty(search)) return true;

            return script.name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        private bool PassFilter(MonoScript script)
        {
            Type type = script.GetClass();
            if (type == null) return false;

            return filter.GetValue() switch
            {
                FilterType.MonoBehaviour => type.IsSubclassOf(typeof(MonoBehaviour)),
                FilterType.ScriptableObject => type.IsSubclassOf(typeof(ScriptableObject)),
                FilterType.Editor => type.IsSubclassOf(typeof(UnityEditor.Editor)),
                _ => true
            };
        }
        private void DrawScript(MonoScript script)
        {
            Type type = script.GetClass();
            string namespaceName = type?.Namespace ?? "No Namespace";

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(script.name, EditorStyles.label, GUILayout.Width(250)))
            {
                AssetDatabase.OpenAsset(script);
            }

            GUILayout.Label(namespaceName);

            EditorGUILayout.EndHorizontal();
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
