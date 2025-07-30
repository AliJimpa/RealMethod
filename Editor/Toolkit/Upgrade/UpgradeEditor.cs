using UnityEditor;

namespace RealMethod
{
    [CustomEditor(typeof(Upgrade), true)]
    public class UpgradeEditor : UnityEditor.Editor
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
                if (BaseComponent.AvailableCount > 0)
                {
                    foreach (var item in BaseComponent.GetItems())
                    {
                        EditorGUILayout.LabelField($"Name: {item.Label} - Status: {CheckStatus(item)} - Available: {BaseComponent.IsAvailable(item)} ");
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"TotalAsset: {BaseComponent.ItemCount}");
            }


        }

        private string CheckStatus(IUpgradeItem item)
        {
            return item.IsUnlocked ? "Unlock" : "Lock";
        }
    }
}