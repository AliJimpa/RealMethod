using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace RealMethod
{
    public class PCGWindow : EditorWindow
    {
        //Basic Structre Data
        private class GD_Class : EditorProperty
        {
            public string Name = "MyName";
            public EP_Vector3 Position;
            public EP_Vector3 Rotation;
            public EP_Vector3 Scale;

            public GD_Class(string _Name, Object _Owner) : base(_Name, _Owner)
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
        private struct PCGLayerStatus
        {
            public string LayerName;
            public int PrefabCount;
            public int TotalInstance;
        }
        private struct PCGPrefabStatus
        {
            public string PrefabName;
            public int Count;
            public string Label;
        }
        private enum PCGExportType
        {
            PCGCash,
            Prefab
        }

        // Fields
        private EP_ScriptableObject<PCGResourceAsset> Resurce;
        private EP_ScriptableObject<PCGGenerationAsset> Genration;
        private EP_ScriptableObject<PCGCashAsset> Cash;
        private EP_List<GD_Class> SelectedData;
        private EP_String CashAddress;
        private EP_String PrefabAddress;
        private EP_Enum<PCGExportType> ExportType;

        // Generation Data
        private PCGData[] GeneratedObject;
        private GameObject[] InitiateObject;
        private Vector2 RuntimeCount = Vector2.zero;
        private GameObject[] SelectedGameObjects;

        //Status Data (AutoClear)
        private PCGLayerStatus[] LayerStatus;
        private List<PCGPrefabStatus> PrefabStatus;

        // UI Data
        private bool[] PanelVisualize = new bool[3];
        private GameObject[] CashObject;
        private ProjectSettingAsset ProjectSetting;



        // Add the menu item
        [MenuItem("Tools/RealMethod/Kit/PCG")]
        public static void ShowWindow()
        {
            GetWindow<PCGWindow>("PCG Window");
        }
        private void OnEnable()
        {
            ProjectSetting = Resources.Load<ProjectSettingAsset>("RealMethod/RealMethodSetting");
            if (ProjectSetting == null)
            {
                Debug.LogError("ProjectSettingAsset is missing from Resources folder!");
            }

            Resurce = new EP_ScriptableObject<PCGResourceAsset>("Resurce", this);
            Genration = new EP_ScriptableObject<PCGGenerationAsset>("Genration", this);
            Cash = new EP_ScriptableObject<PCGCashAsset>("Cash", this);
            SelectedData = new EP_List<GD_Class>("Data", this);
            CashAddress = new EP_String("Address", this);
            CashAddress.SetValue(ProjectSetting.FindAddres(ProjectSettingAsset.IdentityCategory.PCG).Path + "/PCGCashAsset.asset");
            PrefabAddress = new EP_String("Address", this);
            PrefabAddress.SetValue(ProjectSetting.FindAddres(ProjectSettingAsset.IdentityCategory.Prefab).Path + "/PCG.prefab");
            ExportType = new EP_Enum<PCGExportType>("ExportType", this);

            // Subscribe to the selectionChanged event
            Selection.selectionChanged += OnSelectionChanged;
        }
        private void OnDisable()
        {
            // Unsubscribe from the selectionChanged event to avoid memory leaks
            Selection.selectionChanged -= OnSelectionChanged;
            ClearInstance();
            ClearGeneration();
        }
        private void OnGUI()
        {
            EditorGUILayout.Space(1);
            EditorGUILayout.LabelField("PCG", EditorStyles.boldLabel);
            EditorGUILayout.Space(1);
            EditorGUILayout.LabelField("Assets", EditorStyles.centeredGreyMiniLabel);
            if (Resurce.Render() == 1)
            {
                if (!Resurce.isvalid)
                    return;

                int TPrefabCount = 0;
                int TTotalInstance = 0;
                //Status
                LayerStatus = new PCGLayerStatus[4];
                for (int i = 0; i < LayerStatus.Length; i++)
                {
                    if (i != 3)
                    {
                        LayerStatus[i].LayerName = ((PCGSourceLayer)i).ToString();
                        LayerStatus[i].PrefabCount = Resurce.GetValue().GetPrefabCount((PCGSourceLayer)i);
                        TPrefabCount += LayerStatus[i].PrefabCount;
                        LayerStatus[i].TotalInstance = Resurce.GetValue().GetTotalInstance((PCGSourceLayer)i);
                        TTotalInstance += LayerStatus[i].TotalInstance;
                    }
                    else
                    {
                        LayerStatus[i].LayerName = "Total";
                        LayerStatus[i].PrefabCount = TPrefabCount;
                        LayerStatus[i].TotalInstance = TTotalInstance;
                    }
                }

                TTotalInstance = 0;
                PrefabStatus = new List<PCGPrefabStatus>();
                for (int i = 0; i < Resurce.GetValue().GetLength(); i++)
                {
                    PCGPrefabStatus newindex;
                    newindex.PrefabName = Resurce.GetValue().GetSource(i).Prefab.name;
                    newindex.Count = Resurce.GetValue().GetSource(i).Count;
                    TTotalInstance += newindex.Count;
                    newindex.Label = Resurce.GetValue().GetSource(i).Label;
                    PrefabStatus.Add(newindex);
                }
                PCGPrefabStatus TotalIndex;
                TotalIndex.PrefabName = "Total";
                TotalIndex.Count = TTotalInstance;
                TotalIndex.Label = "---";
                PrefabStatus.Add(TotalIndex);
            }
            Genration.Render();
            EditorGUILayout.Space(5);
            if (Resurce.isvalid && Genration.isvalid)
            {
                ControllPanel();
            }
            if (Resurce.isvalid)
            {
                StatusPanel(ref PanelVisualize[1], ref PanelVisualize[0]);
                EditorGUILayout.Space(5);
                SelcetionPanel();
            }
            if (Resurce.isvalid && Genration.isvalid && GeneratedObject != null)
            {
                ExportPanel();
            }
        }



        // Basic Methdo
        private void OnSelectionChanged()
        {
            // Update the selected GameObjects when the selection changes
            SelectedGameObjects = Selection.gameObjects;
            SelectedData.ClearList();
            if (SelectedGameObjects.Length > 0)
            {
                //EditorGUILayout.LabelField("Selected GameObjects:");
                foreach (var sobj in SelectedGameObjects)
                {
                    //EditorGUILayout.LabelField($"- {obj.name}");
                    GD_Class Gd = new GD_Class(sobj.name, this);
                    Gd.Position.SetValue(sobj.transform.position);
                    Gd.Rotation.SetValue(sobj.transform.rotation.eulerAngles);
                    Gd.Scale.SetValue(sobj.transform.localScale);
                    SelectedData.AddItem(Gd);
                }
            }

            // Optionally, force the window to repaint to reflect the changes immediately
            Repaint();
        }
        private void ControllPanel()
        {
            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("Controll", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate"))
            {
                GeneratedObject = Genration.GetValue().GetFullProcess(Resurce.GetValue());
                RuntimeCount.x = GeneratedObject.Length;
            }
            if (GUILayout.Button("Instantiate"))
            {
                if (GeneratedObject != null)
                {
                    ClearInstance();
                    CashObject = new GameObject[4];
                    InitiateObject = new GameObject[GeneratedObject.Length];

                    GameObject PCG = new GameObject($"{Genration.GetValue().name}({Resurce.GetValue().name})");
                    GameObject BackgroundLayer = new GameObject("Background");
                    GameObject MiddlgroundLayer = new GameObject("Middleground");
                    GameObject ForegroundLayer = new GameObject("Foreground");
                    BackgroundLayer.transform.SetParent(PCG.transform);
                    MiddlgroundLayer.transform.SetParent(PCG.transform);
                    ForegroundLayer.transform.SetParent(PCG.transform);
                    CashObject[0] = PCG;
                    CashObject[1] = BackgroundLayer;
                    CashObject[2] = MiddlgroundLayer;
                    CashObject[3] = ForegroundLayer;
                    for (int i = 0; i < GeneratedObject.Length; i++)
                    {
                        PCGSource Source = Resurce.GetValue().GetSource(GeneratedObject[i].PrefabID);
                        switch (GeneratedObject[i].GetLayer(Resurce.GetValue()))
                        {
                            case PCGSourceLayer.Background:
                                InitiateObject[i] = Instantiate(Source.Prefab, GeneratedObject[i].Position, Quaternion.Euler(GeneratedObject[i].Rotation), BackgroundLayer.transform);
                                break;
                            case PCGSourceLayer.Middleground:
                                InitiateObject[i] = Instantiate(Source.Prefab, GeneratedObject[i].Position, Quaternion.Euler(GeneratedObject[i].Rotation), MiddlgroundLayer.transform);
                                break;
                            case PCGSourceLayer.Foreground:
                                InitiateObject[i] = Instantiate(Source.Prefab, GeneratedObject[i].Position, Quaternion.Euler(GeneratedObject[i].Rotation), ForegroundLayer.transform);
                                break;
                        }
                        InitiateObject[i].transform.localScale = GeneratedObject[i].Scale;
                        InitiateObject[i].name = GeneratedObject[i].CodeName;
                        RuntimeCount.y++;
                    }
                }
            }
            if (GUILayout.Button("Clear"))
            {
                ClearInstance();
                ClearGeneration();
            }
            EditorGUILayout.EndHorizontal();

            // Status
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Generat:", GUILayout.Width(120));
            EditorGUILayout.LabelField(RuntimeCount.x.ToString(), GUILayout.Width(120));
            EditorGUILayout.LabelField("Instantiate:", GUILayout.Width(120));
            EditorGUILayout.LabelField(RuntimeCount.y.ToString(), GUILayout.Width(120));
            EditorGUILayout.EndHorizontal();
        }
        private void StatusPanel(ref bool Mode, ref bool Enable)
        {
            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("Status", EditorStyles.centeredGreyMiniLabel);
            //Title
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Enable ? "-" : ">", GUILayout.Width(20)))
            {
                Enable = !Enable;
            }
            if (!Mode)
            {
                EditorGUILayout.LabelField("Layer", GUILayout.Width(120));
                EditorGUILayout.LabelField("Prefab", GUILayout.Width(100));
                EditorGUILayout.LabelField("Count", GUILayout.Width(100));
            }
            else
            {
                EditorGUILayout.LabelField("PrefabName", GUILayout.Width(120));
                EditorGUILayout.LabelField("Count", GUILayout.Width(100));
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

            if (!Mode)
            {
                foreach (PCGLayerStatus layer in LayerStatus)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space(10);
                    EditorGUILayout.LabelField(layer.LayerName, GUILayout.Width(120));
                    EditorGUILayout.LabelField(layer.PrefabCount.ToString(), GUILayout.Width(100));
                    EditorGUILayout.LabelField(layer.TotalInstance.ToString(), GUILayout.Width(100));
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                foreach (PCGPrefabStatus prefab in PrefabStatus)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space(10);
                    EditorGUILayout.LabelField(prefab.PrefabName, GUILayout.Width(120));
                    EditorGUILayout.LabelField(prefab.Count.ToString(), GUILayout.Width(100));
                    EditorGUILayout.LabelField(prefab.Label, GUILayout.Width(100));
                    EditorGUILayout.EndHorizontal();
                }
            }


        }
        private void SelcetionPanel()
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Selcetion", EditorStyles.centeredGreyMiniLabel);
            if (GUILayout.Button("R", GUILayout.Width(20)))
            {
                OnSelectionChanged();
            }
            EditorGUILayout.EndHorizontal();

            if (SelectedData.GetCount() > 0)
            {
                SelectedData.Render();
            }
            else
            {
                EditorGUILayout.LabelField("Selected GameObjects:", "None");
            }
        }
        private void CashPanel()
        {
            EditorGUILayout.Space(1);
            if (Cash.isvalid)
            {
                Cash.Render();
                if (GUILayout.Button("UpdateCash"))
                {
                    PCGCashAsset Temporery = Cash.GetValue();
                    PCGData[] CashTarget = GeneratedObject;
                    for (int i = 0; i < CashTarget.Length; i++)
                    {
                        if (InitiateObject != null && InitiateObject[i] != null)
                        {
                            CashTarget[i].Position = InitiateObject[i].transform.position;
                            CashTarget[i].Rotation = InitiateObject[i].transform.rotation.eulerAngles;
                            CashTarget[i].Scale = InitiateObject[i].transform.localScale;
                        }
                    }
                    Temporery.Set(CashTarget);
                    EditorUtility.SetDirty(Temporery);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            else
            {
                CashAddress.Render();
                EditorGUILayout.BeginHorizontal();
                Cash.Render();
                if (GUILayout.Button("CreateCash"))
                {
                    Cash.SetValue(ScriptableObj.CreateAndSaveAsset<PCGCashAsset>(CashAddress.GetValue()));
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        private void PrefabPanel()
        {
            EditorGUILayout.Space(1);
            PrefabAddress.Render();
            if (GUILayout.Button("CreatePrefab"))
            {
                string localPath = PrefabAddress.GetValue();
                localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

                GameObject prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(CashObject[0], localPath, InteractionMode.UserAction);
                if (prefab != null)
                {
                    Debug.Log("Prefab created successfully at: " + localPath);
                }
                else
                {
                    Debug.LogError("Failed to create prefab.");
                }
            }
        }
        private void ExportPanel()
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Export", EditorStyles.centeredGreyMiniLabel);
            ExportType.Render();
            switch (ExportType.GetValue())
            {
                case PCGExportType.PCGCash:
                    CashPanel();
                    break;
                case PCGExportType.Prefab:
                    if (InitiateObject != null)
                    {
                        PrefabPanel();
                    }
                    break;
            }
        }
        private void ClearGeneration()
        {
            GeneratedObject = null;
            RuntimeCount.x = 0;
        }
        private void ClearInstance()
        {
            RuntimeCount.y = 0;

            if (InitiateObject != null)
            {
                foreach (var TargetObject in InitiateObject)
                {
                    if (TargetObject)
                        DestroyImmediate(TargetObject);
                }
                InitiateObject = null;
            }

            if (CashObject != null)
            {
                foreach (var obj in CashObject)
                {
                    if (obj)
                        DestroyImmediate(obj);
                }
                CashObject = null;
            }
        }

    }

}