using System;
using UnityEngine;

namespace RealMethod
{
    [Serializable]
    public struct PCGData
    {
        public string CodeName;
        public int SourceID { get; private set; }
        public int PrefabIndex { get; private set; }
        public int DataIndex { get; private set; }
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;

        public PCGData(int sourceindex, int prefabindex, int dataindex)
        {
            SourceID = sourceindex;
            PrefabIndex = prefabindex;
            DataIndex = dataindex;
            CodeName = $"{dataindex} {sourceindex}-{prefabindex}";
            Position = Vector3.zero;
            Rotation = Vector3.zero;
            Scale = Vector3.one;
        }

        public PCGSourceLayer GetLayer(PCGResourceAsset Resource)
        {
            return Resource.GetSource(SourceID).Layer;
        }

    }


    public class PCGCashAsset : DataAsset
    {
        [SerializeField]
        private PCGData[] CashData;

        public PCGData GetData(int Index)
        {
            return CashData[Index];
        }

        public GameObject Initiate(PCGResourceAsset Resurce)
        {
            if (CashData != null)
            {
                GameObject PCG = new GameObject(name);
                GameObject BackgroundLayer = new GameObject("Background");
                GameObject MiddlgroundLayer = new GameObject("Middleground");
                GameObject ForegroundLayer = new GameObject("Foreground");
                BackgroundLayer.transform.SetParent(PCG.transform);
                MiddlgroundLayer.transform.SetParent(PCG.transform);
                ForegroundLayer.transform.SetParent(PCG.transform);
                for (int i = 0; i < CashData.Length; i++)
                {
                    PCGSource Source = Resurce.GetSource(CashData[i].SourceID);
                    GameObject TargetSpawned = null;
                    switch (CashData[i].GetLayer(Resurce))
                    {
                        case PCGSourceLayer.Background:
                            TargetSpawned = Instantiate(Source.Prefabs, CashData[i].Position, Quaternion.Euler(CashData[i].Rotation), BackgroundLayer.transform);
                            break;
                        case PCGSourceLayer.Middleground:
                            TargetSpawned = Instantiate(Source.Prefabs, CashData[i].Position, Quaternion.Euler(CashData[i].Rotation), MiddlgroundLayer.transform);
                            break;
                        case PCGSourceLayer.Foreground:
                            TargetSpawned = Instantiate(Source.Prefabs, CashData[i].Position, Quaternion.Euler(CashData[i].Rotation), ForegroundLayer.transform);
                            break;
                    }
                    TargetSpawned.transform.localScale = CashData[i].Scale;
                    TargetSpawned.name = CashData[i].CodeName;
                }
                return PCG;
            }
            else
            {
                Debug.LogWarning($"PCGCashAsset {this.name} is not valid");
                return null;
            }
        }



#if UNITY_EDITOR
        public void SetCash(PCGData[] Target)
        {
            CashData = Target;
        }
#endif

    }


}