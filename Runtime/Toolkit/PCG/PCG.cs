using System.Collections;
using UnityEngine;

namespace RealMethod
{
    public enum PCGType
    {
        Static,
        Runtime
    }

    [AddComponentMenu("RealMethod/Toolkit/PCG/PCG")]
    public abstract class PCG : MonoBehaviour
    {
        [Header("Initiate")]
        [SerializeField]
        private PCGType Type;
        [SerializeField]
        private PCGResourceConfig Resource;
        [SerializeField, ShowInInspectorByEnum("Type", 1)]
        private PCGGenerationAsset Generation;
        [SerializeField, ShowInInspectorByEnum("Type", 0)]
        private PCGCashAsset CashFile;
        [Header("Load")]
        [SerializeField]
        private LoadStage LoadType;
        [SerializeField]
        private bool Stackable = false;

        // private Variable
        private Transform[] Slots = new Transform[50];
        private PCGData[] MyCash;

        private void Awake()
        {

            if (Resource == null)
            {
                Debug.LogError("Resource in Not Value");
                return;
            }

            // if (Stackable)
            // {
            //     Slots = new List<GameObject>();
            // }

            switch (Type)
            {
                case PCGType.Static:
                    if (CashFile == null)
                    {
                        Debug.LogError("CashFile in Not Value");
                        return;
                    }
                    Generation = null;
                    break;
                case PCGType.Runtime:
                    if (Generation == null)
                    {
                        Debug.LogError("Generation in Not Value");
                        return;
                    }
                    CashFile = null;
                    break;
            }
        }


        public void PreGenerat()
        {
            if (Type == PCGType.Runtime)
            {
                StartCoroutine(PCG_Process());
            }
            else
            {
                Debug.LogWarning("PreGenerat just work on Runtime Mode");
            }
        }
        public void Generate()
        {
            switch (Type)
            {
                case PCGType.Static:
                    StartCoroutine(PCG_Initiate(Resource, CashFile));
                    break;
                case PCGType.Runtime:
                    if (MyCash == null)
                    {
                        Debug.LogWarning("Befor Generate Call Pregenerate function");
                        return;
                    }
                    StartCoroutine(PCG_Initiate(Resource, MyCash));
                    break;
            }
            Slots[0].SetParent(this.transform);
        }
        public void Generate(int targetindex)
        {
            Generate();
            if (Stackable)
            {
                Slots[targetindex] = Slots[0];
            }
            else
            {
                Debug.LogWarning("For Stack Your Generat Shoud be Stackable");
            }
        }


        public Transform GetSlot(int index)
        {
            return Slots[index];
        }
        public bool RemoveSlot(int index)
        {
            if (Slots[index] != null)
            {
                Destroy(Slots[index]);
                return true;
            }
            else
            {
                return false;
            }
        }
        public Transform GetLastGeneration()
        {
            return Slots[0];
        }



        private void ClearCash()
        {
            MyCash = null;
        }

        private IEnumerator PCG_Process()
        {
            MyCash = Generation.PreProcess(Resource);
            yield return new WaitForEndOfFrame();
            Generation.Process(ref MyCash);
            yield return new WaitForEndOfFrame();
            Generation.PostProcess(ref MyCash);
            yield return new WaitForEndOfFrame();
        }
        private IEnumerator PCG_Initiate(PCGResourceConfig resurce, PCGCashAsset cashfile)
        {
            if (cashfile.IsValid)
            {
                Slots[0] = new GameObject(cashfile.name).transform;
                GameObject BackgroundLayer = new GameObject("Background");
                GameObject MiddlgroundLayer = new GameObject("Middleground");
                GameObject ForegroundLayer = new GameObject("Foreground");
                BackgroundLayer.transform.SetParent(Slots[0]);
                MiddlgroundLayer.transform.SetParent(Slots[0]);
                ForegroundLayer.transform.SetParent(Slots[0]);
                for (int i = 0; i < cashfile.Length; i++)
                {
                    PCGSource Source = resurce.GetSource(cashfile[i].PrefabID);
                    GameObject TargetSpawned = null;
                    switch (cashfile[i].GetLayer(resurce))
                    {
                        case PCGSourceLayer.Background:
                            TargetSpawned = Instantiate(Source.Prefab, cashfile[i].Position, Quaternion.Euler(cashfile[i].Rotation), BackgroundLayer.transform);
                            break;
                        case PCGSourceLayer.Middleground:
                            TargetSpawned = Instantiate(Source.Prefab, cashfile[i].Position, Quaternion.Euler(cashfile[i].Rotation), MiddlgroundLayer.transform);
                            break;
                        case PCGSourceLayer.Foreground:
                            TargetSpawned = Instantiate(Source.Prefab, cashfile[i].Position, Quaternion.Euler(cashfile[i].Rotation), ForegroundLayer.transform);
                            break;
                    }
                    TargetSpawned.transform.localScale = cashfile[i].Scale;
                    TargetSpawned.name = cashfile[i].CodeName;
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                Debug.LogWarning($"PCGCashAsset {cashfile.name} is not valid");
            }
        }
        private IEnumerator PCG_Initiate(PCGResourceConfig resurce, PCGData[] cashdata)
        {
            Slots[0] = new GameObject(Generation.name).transform;
            GameObject BackgroundLayer = new GameObject("Background");
            GameObject MiddlgroundLayer = new GameObject("Middleground");
            GameObject ForegroundLayer = new GameObject("Foreground");
            BackgroundLayer.transform.SetParent(Slots[0]);
            MiddlgroundLayer.transform.SetParent(Slots[0]);
            ForegroundLayer.transform.SetParent(Slots[0]);
            for (int i = 0; i < cashdata.Length; i++)
            {
                PCGSource Source = resurce.GetSource(cashdata[i].PrefabID);
                GameObject TargetSpawned = null;
                switch (cashdata[i].GetLayer(resurce))
                {
                    case PCGSourceLayer.Background:
                        TargetSpawned = Instantiate(Source.Prefab, cashdata[i].Position, Quaternion.Euler(cashdata[i].Rotation), BackgroundLayer.transform);
                        break;
                    case PCGSourceLayer.Middleground:
                        TargetSpawned = Instantiate(Source.Prefab, cashdata[i].Position, Quaternion.Euler(cashdata[i].Rotation), MiddlgroundLayer.transform);
                        break;
                    case PCGSourceLayer.Foreground:
                        TargetSpawned = Instantiate(Source.Prefab, cashdata[i].Position, Quaternion.Euler(cashdata[i].Rotation), ForegroundLayer.transform);
                        break;
                }
                TargetSpawned.transform.localScale = cashdata[i].Scale;
                TargetSpawned.name = cashdata[i].CodeName;
                yield return new WaitForEndOfFrame();
            }
            ClearCash();
        }
    }
}
