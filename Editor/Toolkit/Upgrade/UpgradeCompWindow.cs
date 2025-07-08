using UnityEditor;

namespace RealMethod
{
    [CustomEditor(typeof(Upgrade), true)]
    public class UpgradeCompWindow : UnityEditor.Editor
    {
        private Upgrade BaseComponent;

        private void OnEnable()
        {
            BaseComponent = (Upgrade)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(" ----------------- Items ----------------- ");
            if (BaseComponent != null)
            {
                if (BaseComponent.Count > 0)
                {
                    foreach (var item in BaseComponent.CopyItemsByClass<UpgradeItem>())
                    {
                        //EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"Name: {item.Title}({item.name}) - Status: {item} ");
                        // if (GUILayout.Button("Find"))
                        // {
                        //     Selection.activeObject = item;
                        //     EditorGUIUtility.PingObject(item);
                        // }
                        //EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"TotalAsset: {BaseComponent.Count}");
            }


        }
    }
}