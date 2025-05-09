using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public enum PCGType
    {
        Static,
        Runtime
    }
    public class PCGSpawner : MonoBehaviour
    {
        [Header("Initiate")]
        [SerializeField]
        private PCGType Type;
        [SerializeField]
        private PCGResourceAsset Resource;
        [SerializeField, ShowInInspectorByEnum("Type", 1)]
        private PCGGenerationAsset Generation;
        [SerializeField, ShowInInspectorByEnum("Type", 0)]
        private PCGCashAsset CashFile;
        [SerializeField]
        private bool InstantiateOnAwake = false;
        [Header("Load")]
        [SerializeField]
        private LoadStage LoadType;
        [SerializeField]
        private bool Stackable = false;


        public List<GameObject> Slots;
        public PCGData[] MyList;
        private PCGGenerationAsset MYGeneration;

        private void Awake()
        {

            if (Resource == null)
            {
                Debug.LogError("Resource in Not Value");
                return;
            }

            if (Stackable)
            {
                Slots = new List<GameObject>();
            }

            switch (Type)
            {
                case PCGType.Static:
                    if (Generation == null)
                    {
                        Debug.LogError("Generation in Not Value");
                        return;
                    }
                    CashFile.Initiate(Resource).transform.SetParent(transform);
                    break;
                case PCGType.Runtime:
                    if (CashFile == null)
                    {
                        Debug.LogError("CashFile in Not Value");
                        return;
                    }
                    break;
            }
        }

        private void Start()
        {
            // MYGeneration = ScriptableObject.Instantiate(Generation);
            // MYGeneration.GetFullProcess(Resource);
            // MyList = MYGeneration.GetFullProcess(Resource);
            // Instantiate
        }
    }
}
