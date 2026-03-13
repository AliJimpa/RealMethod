using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TESTWindow : EditorWindow
{
    private Vector2 scroll;
    private MonoScript script;
    private List<string> Info = new List<string>();



    [MenuItem("Tools/RealMethod/TEST")]
    public static void Open()
    {
        GetWindow<TESTWindow>("TEST");
    }
    private void OnEnable()
    {

    }
    private void OnGUI()
    {
        if (GUILayout.Button("Refresh"))
        {
            Info.Clear();
            if (script == null)
                return;

            AddInfo("name", script.name);
            //AddInfo("text", script.text);
            Type Scripttype = script.GetClass();
            AddInfo("Class", Scripttype.ToString());
            AddInfo("ClassName", Scripttype.Name);
            AddInfo("FullName", Scripttype.FullName);
            AddInfo("Namespace", Scripttype.Namespace);
            Type Parenttype = Scripttype.BaseType;
            AddInfo("Parent", Parenttype.Name);
            AddInfo("IsClass", Scripttype.IsClass.ToString());
            AddInfo("IsAbstact", Scripttype.IsAbstract.ToString());
            AddInfo("IsInterface", Scripttype.IsInterface.ToString());
            AddInfo("IsEnum", Scripttype.IsEnum.ToString());
            AddInfo("IsStruct", Scripttype.IsStruct().ToString());
            AddInfo("IsSealed", Scripttype.IsSealed.ToString());
            Type[] Scriptinterfaces = Scripttype.GetInterfaces();
            AddInfo("Interfaces", Scriptinterfaces.Length.ToString());
            for (int i = 0; i < Scriptinterfaces.Length; i++)
            {
                AddInfo($"{i}", Scriptinterfaces[i].Name);
            }
            FieldInfo[] fields = Scripttype.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            AddInfo("Fields", fields.Length.ToString());
            for (int i = 0; i < fields.Length; i++)
            {
                AddInfo($"{i}.{fields[i].Name}", $"{fields[i].FieldType} - public[{fields[i].IsPublic}], private[{fields[i].IsPrivate}], static[{fields[i].IsStatic}] ");
            }
            PropertyInfo[] properties = Scripttype.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            AddInfo("Properties", properties.Length.ToString());
            for (int i = 0; i < properties.Length; i++)
            {
                AddInfo($"{i}.{properties[i].Name}", $"{properties[i].PropertyType} - Canread[{properties[i].CanRead}], CanWrite[{properties[i].CanWrite}]");
            }
            MethodInfo[] methods = Scripttype.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            AddInfo("Methods", methods.Length.ToString());
            for (int i = 0; i < methods.Length; i++)
            {
                AddInfo($"{i}.{methods[i].Name}", $"{methods[i].ReturnType} - public[{methods[i].IsPublic}], virtual[{methods[i].IsVirtual}], static[{methods[i].IsStatic}]");
            }
            ConstructorInfo[] constructors = Scripttype.GetConstructors();
            for (int i = 0; i < constructors.Length; i++)
            {
                AddInfo($"{i}.{constructors[i].Name}", $"Attributes = {constructors[i].Attributes}");
            }
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);

        script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
        if (Info != null)
        {
            foreach (var item in Info)
            {
                EditorGUILayout.LabelField(item);
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void AddInfo(string Type, string data)
    {
        Info.Add($"{Type}:   {data}");
    }

}