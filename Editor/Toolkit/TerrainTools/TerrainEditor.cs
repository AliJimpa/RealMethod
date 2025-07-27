using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace RealMethod.Editor
{
    [CustomEditor(typeof(TerrainCollider), true)]
    public class TerrainEditor : UnityEditor.Editor
    {
        private TerrainCollider BaseComponent;
        private TreeInstance[] trees;
        private int[] treeCountsType;
        private ExportTreeData ExportSection;

        private void OnEnable()
        {
            BaseComponent = (TerrainCollider)target;
            if (ExportSection == null)
            {
                ExportSection = new ExportTreeData(BaseComponent.GetComponent<Terrain>(), this);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("__________________ TerrainTools __________________ ");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh"))
            {
                trees = BaseComponent.terrainData.treeInstances;
                // Count by prototype index
                treeCountsType = new int[BaseComponent.terrainData.treePrototypes.Length];
                foreach (TreeInstance tree in trees)
                {
                    if (tree.prototypeIndex >= 0 && tree.prototypeIndex < treeCountsType.Length)
                    {
                        treeCountsType[tree.prototypeIndex]++;
                    }
                }
            }
            EditorGUILayout.LabelField($"🌲 Total:{GetTreeCount()}");
            EditorGUILayout.EndHorizontal();
            if (treeCountsType != null)
            {
                for (int i = 0; i < treeCountsType.Length; i++)
                {
                    GameObject prefab = BaseComponent.terrainData.treePrototypes[i].prefab;
                    string name = prefab != null ? prefab.name : "Unknown";
                    EditorGUILayout.LabelField($"🌲 - {name}: {treeCountsType[i]} instances");
                }
            }
            EditorGUILayout.Space();
            ExportSection.OnRender();



        }

        private int GetTreeCount()
        {
            return trees != null ? trees.Length : 0;
        }
    }


    // If You Remove PCG Kit Just Remove ExportTreeData from this code.
    public class ExportTreeData : Object
    {
        private EP_ScriptableObject<PCGCashAsset> CashFile;
        private EP_String CashAddress;
        private ProjectSettingAsset ProjectSetting;
        private Terrain OwnerTerrain;

        public ExportTreeData(Terrain owner, UnityEditor.Editor editor)
        {
            OwnerTerrain = owner;
            if (OwnerTerrain == null)
            {
                Debug.LogError("ExportTreeData in Construct need Terrain");
                return;
            }

            ProjectSetting = Resources.Load<ProjectSettingAsset>("RealMethod/RealMethodSetting");
            if (ProjectSetting == null)
            {
                Debug.LogError("ProjectSettingAsset is missing from Resources folder!");
                return;
            }

            CashFile = new EP_ScriptableObject<PCGCashAsset>("Cash", editor);
            CashAddress = new EP_String("Address", editor);
            CashAddress.SetValue(ProjectSetting.FindAddres(ProjectSettingAsset.IdentityAsset.PCG).Path + "/TerrainCash.asset");
        }

        public void OnRender()
        {
            if (CashFile.isvalid)
            {
                CashFile.Render();
                if (GUILayout.Button("UpdateCash"))
                {
                    PCGCashAsset Temporery = CashFile.GetValue();
                    TerrainData Data = OwnerTerrain.terrainData;
                    TreeInstance[] trees = OwnerTerrain.terrainData.treeInstances;
                    PCGData[] CashTarget = new PCGData[trees.Length];
                    int[] treeCountsType = new int[Data.treePrototypes.Length];
                    foreach (TreeInstance tree in trees)
                    {
                        if (tree.prototypeIndex >= 0 && tree.prototypeIndex < treeCountsType.Length)
                        {
                            treeCountsType[tree.prototypeIndex]++;
                        }
                    }
                    for (int i = 0; i < trees.Length; i++)
                    {
                        PCGData TargetData = new PCGData(trees[i].prototypeIndex, treeCountsType[trees[i].prototypeIndex], i);
                        TargetData.Position = Vector3.Scale(trees[i].position, OwnerTerrain.terrainData.size) + OwnerTerrain.transform.position;
                        TargetData.Rotation = new Vector3(0, trees[i].rotation, 0);
                        TargetData.Scale = new Vector3(trees[i].widthScale, trees[i].heightScale, trees[i].widthScale);
                        CashTarget[i] = TargetData;
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
                CashFile.Render();
                if (GUILayout.Button("CreateCash"))
                {
                    string directoryfile = Path.GetDirectoryName(CashAddress.GetValue());
                    PCGResourceConfig ResurcePack = RM_ScriptableObj.CreateAndSaveAsset<PCGResourceConfig>(directoryfile+"/TerrainResource.asset");
                    PCGCashAsset Temporery = RM_ScriptableObj.CreateAndSaveAsset<PCGCashAsset>(CashAddress.GetValue());
                    TerrainData Data = OwnerTerrain.terrainData;
                    PCGSource[] TerrainSource = new PCGSource[Data.treePrototypes.Length];
                    TreeInstance[] trees = OwnerTerrain.terrainData.treeInstances;
                    PCGData[] CashTarget = new PCGData[trees.Length];
                    int[] treeCountsType = new int[Data.treePrototypes.Length];
                    foreach (TreeInstance tree in trees)
                    {
                        if (tree.prototypeIndex >= 0 && tree.prototypeIndex < treeCountsType.Length)
                        {
                            treeCountsType[tree.prototypeIndex]++;
                        }
                    }
                    for (int k = 0; k < Data.treePrototypes.Length; k++)
                    {
                        TerrainSource[k].Prefab = Data.treePrototypes[0].prefab;
                        TerrainSource[k].Layer = PCGSourceLayer.Background;
                        TerrainSource[k].LoadPriority = PCGSourceLoadOrder.High;
                        TerrainSource[k].Label = "Terrain";
                        TerrainSource[k].Count = treeCountsType[k];
                    }
                    for (int i = 0; i < trees.Length; i++)
                    {
                        PCGData TargetData = new PCGData(trees[i].prototypeIndex, treeCountsType[trees[i].prototypeIndex], i);
                        TargetData.Position = Vector3.Scale(trees[i].position, OwnerTerrain.terrainData.size) + OwnerTerrain.transform.position;
                        TargetData.Rotation = new Vector3(0, trees[i].rotation, 0);
                        TargetData.Scale = new Vector3(trees[i].widthScale, trees[i].heightScale, trees[i].widthScale);
                        CashTarget[i] = TargetData;
                    }
                    Temporery.Set(CashTarget);
                    ResurcePack.Set(TerrainSource);
                    EditorUtility.SetDirty(Temporery);
                    EditorUtility.SetDirty(ResurcePack);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("After Create Cash Automaticly Create Resurce");
            }
        }

    }

}