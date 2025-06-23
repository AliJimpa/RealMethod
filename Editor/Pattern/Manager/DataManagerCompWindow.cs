using UnityEditor;

namespace RealMethod
{
    [CustomEditor(typeof(DataManager), true)]
    public class DataManagerCompWindow : UnityEditor.Editor
    {
        private DataManager BaseComponent;

        private void OnEnable()
        {
            BaseComponent = (DataManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(" ----------------- History ----------------- ");
            if (BaseComponent != null)
            {
                if (BaseComponent.DataLog != null && BaseComponent.DataLog.Length > 0)
                {
                    foreach (var item in BaseComponent.DataLog)
                    {
                        if (item != string.Empty)
                        {
                            EditorGUILayout.LabelField(item);
                        }
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"TotalLog: {BaseComponent.Logindex}");
            }


        }
    }
}