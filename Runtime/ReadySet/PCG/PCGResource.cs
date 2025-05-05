using System;
using UnityEngine;

namespace RealMethod
{
    public enum PCGSourceLoadOrder
    {
        Low,
        Medium,
        High
    }

    [Serializable]
    public struct PCGSource
    {
        public GameObject Prefabs;
        public PCGSourceLoadOrder LoadPriority;
        public string Label;
    }

    [CreateAssetMenu(fileName = "PCG_Resource", menuName = "RealMethod/PCG/Resource", order = 1)]
    public class PCGResource : DataAsset
    {
        [SerializeField]
        private PCGSource[] Sources;
       
    }
}

