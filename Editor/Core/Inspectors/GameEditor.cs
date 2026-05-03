using UnityEditor;

namespace RealMethod.Editor
{
    [CustomEditor(typeof(Game), true)]
    public class GameEditor : UnityEditor.Editor
    {
        private Game Comp;
        private string WorldName => Game.World != null ? Game.World.GetType().Name : "World Not Valid";
        private string BridgeName => Game.Bridge != null ? Game.Bridge.GetType().Name : "GameBridge Not Valid";
        private string ConfigName => Game.Config != null ? Game.Config.GetType().Name : "GameConfig Not Valid";

        private void OnEnable()
        {
            Comp = (Game)target;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            if (Comp != null)
            {
                EditorGUILayout.LabelField($"{WorldName} | {BridgeName} | {ConfigName}");
                EditorGUILayout.LabelField($"GameStat: {Game.State}");
                EditorGUILayout.Space(0.5f);
                
                EditorGUILayout.LabelField($"----------------------------");
                IInspectorInfo[] Info = Comp.GetAllInfo();
                for (int i = 0; i < Info.Length; i++)
                {
                    EditorGUILayout.LabelField($"{i + 1}.{Info[i].GetType().Name}->      {Info[i].GetInfo()}");
                }
            }
        }


    }



}