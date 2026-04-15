using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RealMethod
{
    [CreateAssetMenu(fileName = "TutorialSaveFile", menuName = "RealMethod/Tutorial/SaveFile", order = 1)]
    public class TutorialSaveFile : SaveFile, ITutorialStorage
    {
        [Header("Tutorial")]
        [SerializeField, ReadOnly, TextArea]
        protected string Description = "This Save file include ITutorialStorage for store data by TutorialWidget for saving tutorial label";
        [SerializeField]
        private bool UsePlayerPrefs = true;
        [Header("Storage")]
        public HashSet<string> TutorialMessage = new HashSet<string>();

        // SaveFile Method
        protected override void OnStable(DataManager manager)
        {
        }
        protected override void OnSaved()
        {
            if (UsePlayerPrefs)
                RM_Save.SetArray("Tutorial", TutorialMessage.ToArray());
        }
        protected override void OnLoaded()
        {
            if (UsePlayerPrefs)
                TutorialMessage = RM_Save.GetArray<string>("Tutorial").ToHashSet();
        }
        protected override void OnDeleted()
        {
            if (UsePlayerPrefs)
                TutorialMessage.Clear();
        }

        // IMplement ITutorialStorage Interface
        void IStorage.StorageCreated(Object author)
        {

        }
        void IStorage.StorageLoaded(Object author)
        {

        }
        void ITutorialStorage.AddNewTutorial(TutorialConfig conf)
        {
            TutorialMessage.Add(conf.NameID);
        }
        public bool RemoveTutorial(TutorialConfig conf)
        {
            return TutorialMessage.Remove(conf.NameID);
        }
        bool ITutorialStorage.IsValidTutorial(TutorialConfig conf)
        {
            return TutorialMessage.Contains(conf.NameID);
        }
        void IStorage.StorageClear()
        {
            TutorialMessage.Clear();
        }


        private void Reset()
        {
            TutorialMessage.Clear();
        }

#if UNITY_EDITOR
        public override bool AutoReset(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
                return true;
            return base.AutoReset(state);
        }
#endif

    }



#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(TutorialSaveFile))]
    internal class TutorialSaveFileEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            using (new UnityEditor.EditorGUI.DisabledScope(true))
            {
                UnityEditor.EditorGUILayout.Space();
                UnityEditor.EditorGUILayout.LabelField("Debug");
                UnityEditor.EditorGUILayout.Space();

                using (new UnityEditor.EditorGUI.IndentLevelScope())
                {
                    foreach (var item in ((TutorialSaveFile)target).TutorialMessage)
                    {
                        UnityEditor.EditorGUILayout.LabelField("Message", item);
                    }
                }
            }
        }
    }
#endif
}