using UnityEngine;

namespace RealMethod
{
    public interface IPCGCashAsset : IIdentifier
    {
#if UNITY_EDITOR
        void SetData(PCGData[] Target);
#endif
    }

    public class PCGCashAsset : ConfigAsset, IPCGCashAsset
    {
        public string NameID => name;
        [SerializeField]
        private PCGData[] CashData;

        public bool IsValid => CashData != null;
        public int Length => CashData.Length;



        public PCGData this[int index]
        {
            get => CashData[index];
        }

#if UNITY_EDITOR
        void IPCGCashAsset.SetData(PCGData[] Target)
        {
            CashData = Target;
        }
#endif
    }
}