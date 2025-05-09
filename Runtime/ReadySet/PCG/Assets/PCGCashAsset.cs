using UnityEngine;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "PCG_Cash", menuName = "RealMethod/PCG/Cash", order = 3)]
    public class PCGCashAsset : DataAsset
    {
        [SerializeField]
        private PCGData[] CashData;

        public PCGData GetData(int Index)
        {
            return CashData[Index];
        }

        

#if UNITY_EDITOR
        public void SetCash(PCGData[] Target)
        {
            CashData = Target;
        }
#endif

    }
}