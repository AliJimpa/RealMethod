using UnityEditor;

namespace RealMethod
{
    [CustomEditor(typeof(Inventory), true)]
    public class InventoryCompWindow : UnityEditor.Editor
    {
        private Inventory BaseComponent;

        private void OnEnable()
        {
            BaseComponent = (Inventory)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(" ----------------- Items ----------------- ");
            int total = 0;
            if (BaseComponent != null)
            {
                if (BaseComponent.Count > 0)
                {
                    foreach (var item in BaseComponent.CopyItemsByClass<InventoryItemAsset>())
                    {
                        //EditorGUILayout.BeginHorizontal();
                        int ItemQuantity = BaseComponent.GetQuantity(item);
                        EditorGUILayout.LabelField($"Name: {item.Title}({item.name}) - Quantity: {ItemQuantity} ");
                        total += ItemQuantity;
                        // if (GUILayout.Button("Find"))
                        // {
                        //     Selection.activeObject = item;
                        //     EditorGUIUtility.PingObject(item);
                        // }
                        //EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"TotalAsset: {BaseComponent.Count} - TotalQuantity: {total}");
            }


        }
    }
}