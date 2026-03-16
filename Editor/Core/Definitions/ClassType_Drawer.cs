using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(ClassType), true)]
    public class ClassTypeDrawer : PropertyDrawer
    {
        private SerializedProperty MyProperty;
        private Type TargetType;
        private MonoScript[] scripts;
        private string[] ClassList;
        private int CurrentIndex;
        private int NewIndex = -1;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MyProperty = property.FindPropertyRelative("ClassName");

            if (scripts == null)
            {
                scripts = LoadScripts();
                OnActive();
            }

            if (TargetType == null)
            {
                EditorGUI.LabelField(position, label.text, "Invalid ClassType");
                return;
            }

            NewIndex = EditorGUI.Popup(position, label.text, CurrentIndex, ClassList);

            if (NewIndex != CurrentIndex)
            {
                MyProperty.stringValue = ClassList[NewIndex];
                CurrentIndex = NewIndex;
            }
        }
        private void OnActive()
        {
            string TargetClass = GetTargetType().FullName;
            TargetType = GetClassType(TargetClass);

            ClassList = AppDomain.CurrentDomain.GetAssemblies()
               .SelectMany(a => a.GetTypes())
               .Where(t => TargetType.IsAssignableFrom(t) && !t.IsAbstract)
               .Select(t => t.Name)
               .OrderBy(n => n)
               .ToArray();
            
            if (string.IsNullOrEmpty(MyProperty.stringValue))
            {
                CurrentIndex = ClassList.FindIndex(TargetClass);
            }
            else
            {
                CurrentIndex = ClassList.FindIndex(MyProperty.stringValue);
                NewIndex = CurrentIndex;
            }
            
        }

        private Type GetTargetType()
        {
            if (fieldInfo.FieldType.IsGenericType)
            {
                return fieldInfo.FieldType.GetGenericArguments()[0];
            }

            return null;
        }
        private Type GetClassType(string className)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(className);
                if (type != null)
                    return type;
            }
            return null;
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