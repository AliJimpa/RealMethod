using System.Linq;
using UnityEditor;

namespace RealMethod
{
    [CustomEditor(typeof(CharacterStat), true)]
    public class RPGStat : UnityEditor.Editor
    {
        private CharacterStat BaseComponent;
        private BaseStatData[] CacheData;
        private BaseStatData[] MyData
        {
            get
            {
                if (CacheData == null)
                {
                    CacheData = BaseComponent.GetAllStatData();
                }
                return CacheData;
            }
        }
        private int totalcount => MyData != null ? MyData.Length : 0;


        private void OnEnable()
        {
            BaseComponent = (CharacterStat)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            
            if (!EditorApplication.isPlaying)
                return;

            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(" ----------------- Items ----------------- ");
            if (BaseComponent != null)
            {
                if (MyData != null && MyData.Length > 0)
                {
                    foreach (var stat in MyData)
                    {
                        EditorGUILayout.LabelField($"{stat.NameID}: {stat.MinValue} . {stat.Value} . {stat.MaxValue}  ({stat.modifiersCount})");
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"TotalAsset: {totalcount}");
            }


        }
    }
}