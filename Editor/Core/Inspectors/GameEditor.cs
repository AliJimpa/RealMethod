using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomEditor(typeof(Game), true)]
    public class GameEditor : UnityEditor.Editor
    {
        private Game Comp;
        private string WorldName => Game.World != null ? Game.World.GetType().Name : "World Not Valid";
        private string BridgeName => Game.Bridge != null ? Game.Bridge.GetType().Name : "GameBridge Not Valid";
        private string ConfigName => Game.Config != null ? Game.Config.GetType().Name : "GameConfig Not Valid";
        private IInspectorInfo[] InfoList;
        private bool[] FoldoutList;

        private void OnEnable()
        {
            Comp = (Game)target;
            InfoList = Comp.GetAllInfo();
            FoldoutList = new bool[InfoList.Length];
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            if (Comp != null)
            {
                EditorGUILayout.LabelField($"{WorldName} | {BridgeName} | {ConfigName}");
                EditorGUILayout.LabelField($"GameStat: {Game.State}");
                EditorGUILayout.LabelField($"----------------------------------------({InfoList.Length})----------------------------------------");
                if (InfoList != null)
                {
                    for (int i = 0; i < InfoList.Length; i++)
                    {
                        IInspectorInfo info = InfoList[i];
                        if (info == null)
                            continue;

                        FoldoutList[i] = EditorGUILayout.Foldout(FoldoutList[i], $"{i + 1}. {info.GetTitleInfo()}", true, EditorStyles.foldoutHeader);
                        if (FoldoutList[i])
                        {
                            GUIStyle style = new GUIStyle(EditorStyles.label);
                            style.wordWrap = true;
                            EditorGUILayout.LabelField(info.GetInfo(), style);
                        }
                    }
                }

            }
        }


    }



}