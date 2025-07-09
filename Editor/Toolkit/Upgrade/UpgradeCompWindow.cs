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
                    foreach (UpgradeItem item in BaseComponent.CopyItemsByClass<UpgradeItem>())
                    {
                        EditorGUILayout.LabelField($"Name: {item.Title} - Status: {item.IsUnlocked} ");
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"TotalAsset: {BaseComponent.Count}");
            }


        }
    }
}