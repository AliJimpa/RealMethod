using UnityEditor;

namespace RealMethod
{
    [CustomEditor(typeof(Ability), true)]
    public class AbilityEditor : UnityEditor.Editor
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
                    foreach (var power in BaseComponent.CopyAbilities())
                    {
                        EditorGUILayout.LabelField($"Name: {power.Label} - Status: {GetState(power)}  [{GetlifeTime(power)}]");
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"TotalAbility: {BaseComponent.Count}");
            }


        }

        private string GetState(Power targetpower)
        {
            if (targetpower.IsFinished)
            {
                return "Deactive";
            }
            else
            {
                if (targetpower.IsPaused)
                {
                    return "Paused";
                }
                else
                {
                    return "Active";
                }
            }
        }
        private string GetlifeTime(Power targetpower)
        {
            if (targetpower.IsFinished)
            {
                return "-";
            }
            else
            {
                if (targetpower.RemainingTime == 0)
                {
                    return "Infinit";
                }
                else
                {
                    return targetpower.RemainingTime.ToString();
                }
            }

        }
    }
}