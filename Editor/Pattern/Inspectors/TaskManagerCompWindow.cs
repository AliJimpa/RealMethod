using System;
using UnityEditor;

namespace RealMethod
{
    [CustomEditor(typeof(TaskManager), true)]
    public class TaskManagerCompWindow : UnityEditor.Editor
    {
        private TaskManager BaseComponent;

        private void OnEnable()
        {
            BaseComponent = (TaskManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(" ----------------- Tasks ----------------- ");
            if (BaseComponent != null)
            {
                if (BaseComponent.Count > 0)
                {
                    ITask[] tasks = BaseComponent.GetAllTasks();
                    foreach (var task in tasks)
                    {
                        EditorGUILayout.LabelField($"{task}");
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField($"Total: {tasks.Length}");
                }

            }


        }
    }


    [CustomEditor(typeof(TaskAsset), true)]
    public class TaskAssetCompWindow : UnityEditor.Editor
    {
        private TaskAsset BaseAsset;

        private void OnEnable()
        {
            BaseAsset = (TaskAsset)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug.... ");
            if (BaseAsset != null)
            {
                EditorGUILayout.LabelField($"Status: {CheckStatus(BaseAsset)}");
                if (BaseAsset is TaskBehaviour provider)
                {
                    if (provider.IsInfinit)
                    {
                        EditorGUILayout.LabelField($"Time: Infinit");
                    }
                    else
                    {
                        EditorGUILayout.LabelField($"Time: {Math.Round(provider.ElapsedTime, 2)}  ({Math.Round((1 - provider.NormalizedTime) * 100, 2)}%)");
                    }
                }

            }
        }


        private string CheckStatus(TaskAsset task)
        {
            return task.IsEnable ? "Enable" : "Disable";
        }
    }




}