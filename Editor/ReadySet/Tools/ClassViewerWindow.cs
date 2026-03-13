using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    class FieldModule
    {
        private MonoScript MyScript = null;
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
        public bool HasFile => MyScript != null;
        public MonoScript ScriptFile => MyScript;
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
            EditorGUILayout.BeginHorizontal();
            DrawField();
            EditorGUILayout.EndHorizontal();
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
        public void AddChild(FieldModule ChildModule)
        {
            if (ChildModule == null)
            {
                Print("ChildModule is not valid!");
                return;
            }
            MyChilds.Add(ChildModule);
        }
        public void AddChild(MonoScript File)
        {
            Type NewType = File.GetClass();
            if (NewType == null || NewType.Namespace == null)
            {
                Print($"MonoScript does't have have any 'MainClass'or'NameSpace' for Child[{File.name}]");
                return;
            }

            FieldModule NewModule = new FieldModule(File);
            FieldModule ParentField = TraverseHierarchy(File.GetClass(), true);

            if (ParentField.TryFindField(NewModule.Title, out FieldModule result))
            {
                if (!result.HasFile)
                {
                    result.SetFileScript(File);
                }
            }
            else
            {
                ParentField.AddChild(NewModule);
            }

        }
        public void Expand()
        {
            IsExpand = true;
        }

        private void DrawField()
        {
            Color old = GUI.color;
            if (!HasFile)
            {
                GUI.color = new Color(0.9f, 0.9f, 0.9f);
            }

            // Write Name as Label or Foldout
            if (MyChilds.Count == 0)
            {
                EditorGUILayout.LabelField(Title, GUILayout.Width(300));
                IsExpand = false;
            }
            else
            {
                IsExpand = EditorGUILayout.Foldout(IsExpand, Title, true);
            }

            // Add Button
            if (MyScript != null)
            {
                if (GUILayout.Button("OpenFile", EditorStyles.label))
                {
                    AssetDatabase.OpenAsset(MyScript);
                }
            }

            GUI.color = old;
        }
        private FieldModule TraverseHierarchy(Type Target, bool ReturnParent = false)
        {
            FieldModule ParentField = null;
            Type ParentChildType = Target.BaseType;
            if (ParentChildType != null)
            {
                if (ParentChildType.Namespace == Target.Namespace)
                {
                    //Recursive
                    ParentField = TraverseHierarchy(ParentChildType);
                }
            }
            if (ParentField == null)
            {
                ParentField = GetOrCreateField(Target.Namespace);
            }
            return ReturnParent ? ParentField : ParentField.GetOrCreateField(Target.Name);
        }
        private void SetFileScript(MonoScript File)
        {
            MyScript = File;
        }
        private FieldModule GetOrCreateField(string title)
        {
            FieldModule Result = FindField(title);
            if (Result == null)
            {
                Result = new FieldModule(title);
                MyChilds.Add(Result);
            }
            return Result;
        }
        private FieldModule FindField(string title)
        {
            foreach (var child in MyChilds)
            {
                if (child.Title == title)
                { return child; }
            }
            return null;
        }
        private bool TryFindField(string title, out FieldModule module)
        {
            foreach (var child in MyChilds)
            {
                if (child.Title == title)
                {
                    module = child;
                    return true;
                }
            }
            module = null;
            return false;
        }
        private void Print(string message)
        {
            Debug.Log($"[{Title}]: {message}");
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
        private FieldModule Root;
        private string search = string.Empty;
        private ComboBox<FilterType> filter;
        public bool IsSearching
        {
            get
            {
                if (!string.IsNullOrEmpty(search))
                    return true;

                if (filter.GetValue() != FilterType.All)
                    return true;

                return false;
            }
        }


        [MenuItem("Tools/RealMethod/ClassViewer")]
        public static void Open()
        {
            GetWindow<ClassViewerWindow>("Class Viewer");
        }
        private void OnEnable()
        {
            filter = new ComboBox<FilterType>("Filter", this);
            ProjectScripts = LoadScripts();
            Preper();
        }
        private void OnGUI()
        {
            DrawToolbar();
            scroll = EditorGUILayout.BeginScrollView(scroll);
            if (IsSearching)
            {
                DrawScriptList();
            }
            else
            {
                Root.Draw();
            }
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
                filter.SetValue(FilterType.All);
                Root = null;
                ProjectScripts = LoadScripts();
                Preper();
            }

            EditorGUILayout.EndHorizontal();
        }
        private void DrawScriptList()
        {
            foreach (var script in ProjectScripts)
            {
                if (!PassSearch(script)) continue;
                if (!PassFilter(script)) continue;

                DrawScript(script);
            }
        }
        void DrawScript(MonoScript script)
        {
            Type type = script.GetClass();
            string namespaceName = type?.Namespace ?? "No Namespace";

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label($"{script.name}({namespaceName})", GUILayout.Width(300));

            if (GUILayout.Button("OpenFile", EditorStyles.label))
            {
                AssetDatabase.OpenAsset(script);
            }

            EditorGUILayout.EndHorizontal();
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
        private void Preper()
        {
            Root = new FieldModule("Root");
            FieldModule NoneNamespace = new FieldModule("Global (No Namespace)");
            FieldModule NoneClass = new FieldModule("No Type");
            Root.AddChild(NoneNamespace);
            Root.AddChild(NoneClass);

            foreach (var script in ProjectScripts)
            {
                if (script != null)
                {
                    Type ScriptType = script.GetClass();
                    if (ScriptType != null)
                    {
                        if (ScriptType.Namespace != null)
                        {
                            Root.AddChild(script);
                        }
                        else
                        {
                            NoneNamespace.AddChild(new FieldModule(script));
                        }
                    }
                    else
                    {
                        NoneClass.AddChild(new FieldModule(script));
                    }

                }
            }

            Root.Expand();
        }
    }
}
