using System.Linq;
using UnityEditor;

namespace RealMethod.Editor
{
    [CustomEditor(typeof(Game), true)]
    public class GameEditor : UnityEditor.Editor
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
                EditorGUILayout.LabelField($"GameStat: {Game.State}");
                EditorGUILayout.LabelField($"{GetWorld()} | {GetBrgidge()} | {GetConfig()}");
                EditorGUILayout.Space(0.5f);
                string[] Services = BaseComponent.GetAllServiceNames();
                string[] Managers = BaseComponent.gameObject.GetComponents<IGameManager>().Select(c => c.GetManagerClass().name).ToArray();
                string[] WorldManagers = Game.World ? Game.World.gameObject.GetComponents<IGameManager>().Select(c => c.GetManagerClass().name).ToArray() : new string[0];
                for (int i = 0; i < Services.Length; i++)
                {
                    EditorGUILayout.LabelField($"{i + 1}. {Services[i]}");
                }
            }
        }

        private string GetWorld()
        {
            return Game.World != null ? Game.World.GetType().Name : "World Not Valid";
        }
        private string GetBrgidge()
        {
            return Game.Bridge != null ? Game.Bridge.GetType().Name : "GameBridge Not Valid";
        }
        private string GetConfig()
        {
            return Game.Config != null ? Game.Config.GetType().Name : "GameConfig Not Valid";
        }

    }



}