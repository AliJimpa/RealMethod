using UnityEditor;

namespace RealMethod
{
    [CustomEditor(typeof(Game), true)]
    public class GameCompWindow : UnityEditor.Editor
    {
        private Game BaseComponent;

        private void OnEnable()
        {
            BaseComponent = (Game)target;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            if (BaseComponent != null)
            {
                EditorGUILayout.LabelField($"{Game.World.GetType().Name} | {Game.Service.GetType().Name} | {Game.Config.GetType().Name}");
                EditorGUILayout.Space(0.5f);
                string[] Data = BaseComponent.GetAllServiceNames();
                for (int i = 0; i < Data.Length; i++)
                {
                    EditorGUILayout.LabelField($"{i + 1}. {Data[i]}");
                }
            }
        }

    }
}