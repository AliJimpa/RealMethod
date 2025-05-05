using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public class PCGComponent : MonoBehaviour
    {
        public PCGResource Resource;
        public PCGProcess Generation;
        public List<PCGData> MyList = new List<PCGData>();

        private PCGProcess MYGeneration;

        private void Start()
        {
            MYGeneration = ScriptableObject.Instantiate(Generation);
            MYGeneration.Generate(Resource);
            MyList = MYGeneration.GetResult();
        }
    }
}
