using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomEditor(typeof(TaskManager), true)]
    public class TaskManagerEditor : UnityEditor.Editor
    {
        private TaskManager BaseComponent;
        FieldInfo tasksField;

        private void OnEnable()
        {
            BaseComponent = (TaskManager)target;
            tasksField = target.GetType().GetField(
            "Tasks",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public

        );

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            if (BaseComponent != null)
            {
                if (BaseComponent.Count > 0)
                {
                    IList tasks = tasksField.GetValue(target) as IList;
                    for (int i = 0; i < tasks.Count; i++)
                    {
                        object task = tasks[i];
                        if (task == null)
                            continue;
                        EditorGUILayout.LabelField($"{i}.{task.GetType().Name}", EditorStyles.boldLabel);
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField($"Total: {BaseComponent.Count}");
                }

            }


        }
    }



}