using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public class ClassViewerWindow : EditorWindow
    {
        private class FieldModule
        {
            private ClassViewerWindow MyOwner;
            private MonoScript MyScript = null;
            private List<FieldModule> MyChilds = new List<FieldModule>();
            private string MyName = string.Empty;


            public string Title
            {
                get
                {
                    if (MyScript != null)
                    {
                        return MyScript.GetClass().Name;
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


            public FieldModule(ClassViewerWindow owner, string Name)
            {
                MyOwner = owner;
                MyName = Name;
            }
            public FieldModule(ClassViewerWindow owner, MonoScript script)
            {
                MyOwner = owner;
                MyScript = script;
            }


            public void Draw()
            {
                if (MyScript != null)
                {
                    if (MyScript.GetClass() == null)
                        return;
                }

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

                FieldModule NewModule = new FieldModule(MyOwner, File);
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
                if (MyScript)
                {
                    if (GUILayout.Button("INFO  ", EditorStyles.linkLabel))
                    {
                        MyOwner.SelectedScript = MyScript;
                    }
                    if (MyScript != null)
                    {
                        if (GUILayout.Button("OpenFile  ", EditorStyles.linkLabel))
                        {
                            AssetDatabase.OpenAsset(MyScript);
                        }
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
                    ParentField = GetOrCreateField(Target.Namespace, true);
                    if (Target.IsInterface)
                    {
                        ParentField = ParentField.GetOrCreateField("<Interface>");
                    }
                    if (Target.IsValueType && !Target.IsEnum)
                    {
                        ParentField = ParentField.GetOrCreateField("<Struct>");
                    }
                }
                return ReturnParent ? ParentField : ParentField.GetOrCreateField(Target.Name);
            }
            private void SetFileScript(MonoScript File)
            {
                MyScript = File;
            }
            private FieldModule GetOrCreateField(string title, bool IsNameSpace = false)
            {
                FieldModule Result = FindField(title);
                if (Result == null)
                {
                    Result = new FieldModule(MyOwner, title);
                    MyChilds.Add(Result);
                    if (IsNameSpace)
                    {
                        Result.AddChild(new FieldModule(MyOwner, "<Interface>"));
                        Result.AddChild(new FieldModule(MyOwner, "<Struct>"));
                    }
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
        public class Ceckbox : EditorProperty<bool>
        {
            public Ceckbox(string Name, UnityEngine.Object other) : base(Name, other)
            {
            }


            protected override byte UpdateRender()
            {
                CashValue = GUILayout.Toggle(CurrentValue, GUIContent.none, GUILayout.Width(18));
                if (CashValue == CurrentValue)
                {
                    return 0;
                }
                else
                {
                    SetValue(CashValue);
                    return 1;
                }
            }
            protected override void FixError(int Id)
            {
            }

        }
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
        private Ceckbox AdvanceSerach;
        private MonoScript SelectedScript;

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
            AdvanceSerach = new Ceckbox("Checkbox", this);
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
            if (SelectedScript != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"{SelectedScript.name}.cs");
                if (GUILayout.Button(GUIContent.none, GUI.skin.FindStyle("ToolbarSearchCancelButton")))
                {
                    SelectedScript = null;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.TextArea(Analyze(SelectedScript), GUILayout.ExpandHeight(true));
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.HelpBox(
            " Note: This inspector uses reflection and can only detect the primary type in this script and its nested types. " +
            "Additional classes, structs, enums, or interfaces declared outside the main type in the same file will not appear.",
            MessageType.Info);
        }



        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            AdvanceSerach.Render();

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
                SelectedScript = null;
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
        private void DrawScript(MonoScript script)
        {
            Vector2 scroller = Vector2.zero;
            Type type = script.GetClass();
            string namespaceName = type?.Namespace ?? "No Namespace";

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label($"{script.name}({namespaceName})", GUILayout.Width(400));

            if (GUILayout.Button("INFO  ", EditorStyles.linkLabel))
            {
                SelectedScript = script;
            }

            if (GUILayout.Button("OpenFile  ", EditorStyles.linkLabel))
            {
                AssetDatabase.OpenAsset(script);
            }

            EditorGUILayout.EndHorizontal();
        }
        private bool PassSearch(MonoScript script)
        {
            if (string.IsNullOrEmpty(search)) return true;

            string Result = string.Empty;
            if (AdvanceSerach.GetValue())
            {
                Result = GetScriptInfo(script);
            }
            else
            {
                string type = script.GetClass() != null ? script.GetClass().Name : string.Empty;
                Result = $"{script.name},{type}";
            }

            return Result.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

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
        private string GetScriptInfo(MonoScript script)
        {
            if (script == null) return "";

            Type type = script.GetClass();
            if (type == null) return script.name;

            string result =
                script.name + "," +
                type.Name + "," +
                type.Namespace + "," +
                (type.BaseType != null ? type.BaseType.Name : "");

            var nestedTypes = type.GetNestedTypes(
                BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var nested in nestedTypes)
            {
                result += "," + nested.Name;
            }

            return result;
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
        private string Analyze(MonoScript script)
        {
            if (script == null)
            {
                return "No script selected.";
            }

            Type type = script.GetClass();

            if (type == null)
            {
                return "Could not find class in script.";
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("=== SCRIPT INFO ===");
            sb.AppendLine($"Namespace: {type.Namespace}");
            sb.AppendLine($"Class: {type.Name}");
            sb.AppendLine($"Abstract: {type.IsAbstract}");
            sb.AppendLine($"Sealed: {type.IsSealed}");
            sb.AppendLine($"Static: {type.IsAbstract && type.IsSealed}");
            sb.AppendLine("");

            BindingFlags flags =
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.DeclaredOnly;

            // Fields
            sb.AppendLine("=== FIELDS ===");
            foreach (var field in type.GetFields(flags))
            {
                sb.AppendLine($"{GetAccess(field)} {field.FieldType.Name} {field.Name}");
            }

            sb.AppendLine("");

            // Properties
            sb.AppendLine("=== PROPERTIES ===");
            foreach (var prop in type.GetProperties(flags))
            {
                sb.AppendLine($"{prop.PropertyType.Name} {prop.Name}");
            }

            sb.AppendLine("");

            // Methods
            sb.AppendLine("=== METHODS ===");
            foreach (var method in type.GetMethods(flags))
            {
                if (method.IsSpecialName) continue;

                sb.Append($"{GetAccess(method)} {method.ReturnType.Name} {method.Name}(");

                var parameters = method.GetParameters();

                for (int i = 0; i < parameters.Length; i++)
                {
                    sb.Append($"{parameters[i].ParameterType.Name} {parameters[i].Name}");

                    if (i < parameters.Length - 1)
                        sb.Append(", ");
                }

                sb.AppendLine(")");
            }

            return sb.ToString();
        }
        private string GetAccess(FieldInfo field)
        {
            if (field.IsPublic) return "public";
            if (field.IsPrivate) return "private";
            if (field.IsFamily) return "protected";
            if (field.IsAssembly) return "internal";
            return "";
        }
        private string GetAccess(MethodInfo method)
        {
            if (method.IsPublic) return "public";
            if (method.IsPrivate) return "private";
            if (method.IsFamily) return "protected";
            if (method.IsAssembly) return "internal";
            return "";
        }
        private void Preper()
        {
            Root = new FieldModule(this, "Root");
            FieldModule NoneNamespace = new FieldModule(this, "<Global> (No Namespace)");
            FieldModule NoneClass = new FieldModule(this, "<Core> (No Type)");
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
                            NoneNamespace.AddChild(new FieldModule(this, script));
                        }
                    }
                    else
                    {
                        NoneClass.AddChild(new FieldModule(this, script));
                    }

                }
            }

            Root.Expand();
        }
    }
}
