using System;
using UnityEngine;

namespace RealMethod
{
    [Serializable]
    public class PCGData
    {
        public int ID;
        public Vector3 Transform;
        public Vector3 Rotation;
        public Vector3 Scale;
    }


    [CreateAssetMenu(fileName = "PCG_Cash", menuName = "RealMethod/PCG/Cash", order = 2)]
    public class PCGCash : DataAsset
    {
        [SerializeField]
        private PCGData[] CashData;

    }
}