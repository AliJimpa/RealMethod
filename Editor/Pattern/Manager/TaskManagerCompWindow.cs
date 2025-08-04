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
            int total = 0;
            if (BaseComponent != null)
            {
                if (BaseComponent.Count > 0)
                {
                    ITask[] tasks = BaseComponent.GetAllTasks();
                    foreach (var task in tasks)
                    {
                        EditorGUILayout.LabelField($"{task.NameID}:   {CheckPauseStatus(task)}  ,   {task.RemainingTime}");
                        total++;
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"Total: {total}");
            }


        }



        private string CheckPauseStatus(ITask task)
        {
            return task.IsPaused ? "Paused" : "Running";
        }
    }
}