using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Text;

public class ScriptInspectorWindow : EditorWindow
{
    private MonoScript script;
    private Vector2 scroll;
    private string output;

    [MenuItem("Tools/Script Inspector")]
    static void Open()
    {
        GetWindow<ScriptInspectorWindow>("Script Inspector");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Select MonoScript", EditorStyles.boldLabel);

        script = (MonoScript)EditorGUILayout.ObjectField(
            "Script",
            script,
            typeof(MonoScript),
            false);

        if (GUILayout.Button("Analyze Script"))
        {
            Analyze();
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);

        EditorGUILayout.TextArea(output, GUILayout.ExpandHeight(true));

        EditorGUILayout.EndScrollView();
    }

    void Analyze()
    {
        if (script == null)
        {
            output = "No script selected.";
            return;
        }

        Type type = script.GetClass();

        if (type == null)
        {
            output = "Could not find class in script.";
            return;
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

        output = sb.ToString();
    }
    string GetAccess(FieldInfo field)
    {
        if (field.IsPublic) return "public";
        if (field.IsPrivate) return "private";
        if (field.IsFamily) return "protected";
        if (field.IsAssembly) return "internal";
        return "";
    }
    string GetAccess(MethodInfo method)
    {
        if (method.IsPublic) return "public";
        if (method.IsPrivate) return "private";
        if (method.IsFamily) return "protected";
        if (method.IsAssembly) return "internal";
        return "";
    }
}
