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
                EditorGUILayout.LabelField($"{GetWorld()} | {GetSetvice()} | {GetConfig()}");
                EditorGUILayout.Space(0.5f);
                string[] Data = BaseComponent.GetAllServiceNames();
                for (int i = 0; i < Data.Length; i++)
                {
                    EditorGUILayout.LabelField($"{i + 1}. {Data[i]}");
                }
            }
        }

        private string GetWorld()
        {
            return Game.World != null ? Game.World.GetType().Name : "World Not Valid";
        }
        private string GetSetvice()
        {
            return Game.Service != null ? Game.Service.GetType().Name : "GameService Not Valid";
        }
        private string GetConfig()
        {
            return Game.Config != null ? Game.Config.GetType().Name : "GameConfig Not Valid";
        }

    }



}