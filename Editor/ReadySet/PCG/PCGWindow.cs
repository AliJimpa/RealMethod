using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace RealMethod
{
    public class PCGWindow : EditorWindow
    {
        private class DataClass : EditorProperty
        {
            public string Name = "MyName";
            public EP_Vector3 Position;
            public EP_Vector3 Rotation;
            public EP_Vector3 Scale;

            public DataClass(string _Name, Object _Owner) : base(_Name, _Owner)
            {
                Name = _Name;
                Position = new EP_Vector3("Position", _Owner);
                Rotation = new EP_Vector3("Rotation", _Owner);
                Scale = new EP_Vector3("Scale", _Owner);
            }

            protected override byte UpdateRender()
            {
                // Create a centered style
                GUIStyle centeredStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter
                };

                // Render the centered label
                EditorGUILayout.LabelField(Name, centeredStyle);

                Position.Render();
                Rotation.Render();
                Scale.Render();
                return 0;
            }
            protected override void FixError(int Id)
            {
                throw new System.NotImplementedException();
            }
        }

        private EP_ScriptableObject<PCGResourceAsset> Resurce;
        private EP_ScriptableObject<PCGGenerationAsset> Genration;
        private EP_ScriptableObject<PCGCashAsset> Cash;
        private EP_List<DataClass> SelectionData;
        private PCGData[] GeneratedData;
        private GameObject[] ResultObject;

        private bool[] PanelVisualize = new bool[3];



        // Add the menu item
        [MenuItem("Window/RealMethod/PCG")]
        public static void ShowWindow()
        {
            GetWindow<PCGWindow>("PCG Window");
        }


        private void OnEnable()
        {
            Resurce = new EP_ScriptableObject<PCGResourceAsset>("Resurce", this);
            Genration = new EP_ScriptableObject<PCGGenerationAsset>("Genration", this);
            Cash = new EP_ScriptableObject<PCGCashAsset>("Cash", this);
            SelectionData = new EP_List<DataClass>("Data", this);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(1);
            EditorGUILayout.LabelField("PCG", EditorStyles.boldLabel);
            EditorGUILayout.Space(1);
            EditorGUILayout.LabelField("Assets", EditorStyles.centeredGreyMiniLabel);
            Resurce.Render();
            Genration.Render();
            if (!Resurce.isvalid || !Genration.isvalid)
            {
                GUIUtility.ExitGUI();
            }
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate"))
            {
                GeneratedData = Genration.GetValue().GetFullProcess(Resurce.GetValue());
                ResultObject = new GameObject[GeneratedData.Length];
                SelectionData.ClearList();
                foreach (var item in GeneratedData)
                {
                    DataClass NewData = new DataClass(item.CodeName, this);
                    NewData.Position.SetValue(item.Position);
                    NewData.Rotation.SetValue(item.Rotation);
                    NewData.Scale.SetValue(item.Scale);
                    SelectionData.AddItem(NewData);
                }
            }
            if (GUILayout.Button("Instantiate"))
            {
                for (int i = 0; i < GeneratedData.Length; i++)
                {
                    PCGSource Source = Resurce.GetValue().GetSource(GeneratedData[i].SourceID);
                    ResultObject[i] = Instantiate(Source.Prefabs, GeneratedData[i].Position, Quaternion.Euler(GeneratedData[i].Rotation));
                }
            }
            if (GUILayout.Button("Clear"))
            {
                SelectionData.ClearList();
                GeneratedData = null;
                foreach (var TargetObject in ResultObject)
                {
                    if (TargetObject)
                        DestroyImmediate(TargetObject);
                }
                ResultObject = null;
            }
            EditorGUILayout.EndHorizontal();
            StatusPanel(ref PanelVisualize[1],ref PanelVisualize[0]);

            EditorGUILayout.Space(5);
            SelectionData.Render();
        }



        private void StatusPanel(ref bool Mode , ref bool Enable)
        {
            EditorGUILayout.Space(1);
            EditorGUILayout.LabelField("Status", EditorStyles.centeredGreyMiniLabel);
            //Title
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("()", GUILayout.Width(20)))
            {
                Enable = !Enable;
            }
            if (Mode)
            {
                EditorGUILayout.LabelField("Layer", GUILayout.Width(120));
                EditorGUILayout.LabelField("Prefab", GUILayout.Width(100));
                EditorGUILayout.LabelField("Instance", GUILayout.Width(100));
            }
            else
            {
                EditorGUILayout.LabelField("PrefabName", GUILayout.Width(120));
                EditorGUILayout.LabelField("Instance", GUILayout.Width(100));
                EditorGUILayout.LabelField("Label", GUILayout.Width(100));
            }
            if (GUILayout.Button("Mode", GUILayout.Width(40)))
            {
                Mode = !Mode;
            }
            EditorGUILayout.EndHorizontal();
            if (!Enable)
            {
                return;
            }

            if(Mode)
            {
                // foreach (var item in collection)
                // {
                    
                // }
            }


        }



    }

}