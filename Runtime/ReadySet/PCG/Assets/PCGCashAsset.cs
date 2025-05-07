using System;
using UnityEngine;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "PCG_Cash", menuName = "RealMethod/PCG/Cash", order = 3)]
    public class PCGCashAsset : DataAsset
    {
        [SerializeField]
        private PCGData[] CashData;

    }
}