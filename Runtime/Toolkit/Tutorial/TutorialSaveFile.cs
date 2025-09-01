using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
                RM_PlayerPrefs.SetArray("Tutorial", TutorialMessage.ToArray());
        }
        protected override void OnLoaded()
        {
            if (UsePlayerPrefs)
                TutorialMessage = RM_PlayerPrefs.GetArray<string>("Tutorial").ToHashSet();
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
            TutorialMessage.Add(conf.Label);
        }
        public bool RemoveTutorial(TutorialConfig conf)
        {
            return TutorialMessage.Remove(conf.Label);
        }
        bool ITutorialStorage.IsValidTutorial(TutorialConfig conf)
        {
            return TutorialMessage.Contains(conf.Label);
        }
        void IStorage.StorageClear()
        {
            TutorialMessage.Clear();
        }

#if UNITY_EDITOR
        public override void OnEditorPlay()
        {
            TutorialMessage.Clear();
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