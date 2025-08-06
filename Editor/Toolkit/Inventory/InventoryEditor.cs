using UnityEditor;

namespace RealMethod
{
    [CustomEditor(typeof(Inventory), true)]
    public class InventoryEditor : UnityEditor.Editor
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
                    foreach (var item in BaseComponent.CopyItemsByClass<IInventoryItem>())
                    {
                        //EditorGUILayout.BeginHorizontal();
                        int ItemQuantity = BaseComponent.GetQuantity(item);
                        EditorGUILayout.LabelField($"Name: {item.NameID}({item.GetItem().name}) - Quantity: {ItemQuantity} ");
                        total += ItemQuantity;
                        //EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"TotalAsset: {BaseComponent.Count} - TotalQuantity: {total}");
            }


        }
    }
}