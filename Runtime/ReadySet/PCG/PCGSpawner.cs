using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public class PCGSpawner : MonoBehaviour
    {
        public PCGResourceAsset Resource;
        public PCGGenerationAsset Generation;
        public List<PCGData> MyList = new List<PCGData>();

        private PCGGenerationAsset MYGeneration;

        private void Start()
        {
            MYGeneration = ScriptableObject.Instantiate(Generation);
            MYGeneration.GetFullProcess(Resource);
            MyList = MYGeneration.GetResult();
        }
    }
}
