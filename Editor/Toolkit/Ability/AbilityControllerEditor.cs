using UnityEditor;

namespace RealMethod
{
    [CustomEditor(typeof(AbilityController), true)]
    public class AbilityControllerEditor : UnityEditor.Editor
    {
        private AbilityController MyController;
        private void OnEnable()
        {
            MyController = (AbilityController)target;
        }
        private void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            // Draw default inspector
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Debug ({MyController.Count})", EditorStyles.boldLabel);
            for (int i = 0; i < MyController.Count; i++)
            {
                EditorGUILayout.LabelField($"{MyController.GetAbility(i).NameID}: {IsReady(MyController.GetAbility(i))}");
            }

        }


        private string IsReady(IAbility abil)
        {
            return abil.CanUse(MyController.gameObject) ? "Ready" : "Pending";
        }

    }

}