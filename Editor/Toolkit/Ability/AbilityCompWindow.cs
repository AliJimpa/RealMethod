using UnityEditor;

namespace RealMethod
{
    [CustomEditor(typeof(Ability), true)]
    public class AbilityCompWindow : UnityEditor.Editor
    {
        private Ability BaseComponent;

        private void OnEnable()
        {
            BaseComponent = (Ability)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(" ----------------- Abilites ----------------- ");
            if (BaseComponent != null)
            {
                if (BaseComponent.Count > 0)
                {
                    foreach (var Ability in BaseComponent.CopyAbilities())
                    {
                        EditorGUILayout.LabelField($"Name: {Ability.Name} - Status: {Ability.OnFinished}");
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"TotalAbility: {BaseComponent.Count}");
            }


        }
    }
}