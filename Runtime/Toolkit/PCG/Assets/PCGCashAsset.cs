using UnityEngine;

namespace RealMethod
{
    public class PCGCashAsset : ConfigAsset
    {
        [SerializeField]
        private PCGData[] CashData;

        public bool IsValid => CashData != null;
        public int Length => CashData.Length;
        
        public PCGData this[int index]
        {
            get => CashData[index];
        }

#if UNITY_EDITOR
        public void Set(PCGData[] Target)
        {
            CashData = Target;
        }
#endif

    }
}