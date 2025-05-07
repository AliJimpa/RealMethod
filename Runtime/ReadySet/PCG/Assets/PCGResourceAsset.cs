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
    public enum PCGSourceLayer
    {
        Background,
        Middleground,
        Foreground
    }

    [Serializable]
    public struct PCGSource
    {
        public GameObject Prefabs;
        public PCGSourceLayer Layer;
        public PCGSourceLoadOrder LoadPriority;
        public string Label;
        public int Count;
    }

    [CreateAssetMenu(fileName = "PCG_Resource", menuName = "RealMethod/PCG/Resource", order = 1)]
    public class PCGResourceAsset : DataAsset
    {
        [SerializeField]
        private PCGSource[] Sources;

        public PCGSource GetSource(int index)
        {
            return Sources[index];
        }
        public int GetLength()
        {
            return Sources.Length;
        }

    }
}

