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
        Background = 0,
        Middleground = 1,
        Foreground = 2
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
        
        public int GetPrefabCount(PCGSourceLayer layer)
        {
            int result = 0;
            foreach (var So in Sources)
            {
                if(So.Layer == layer)
                {
                    result++;
                }
            }
            return result;
        }
        public int GetTotalInstance(PCGSourceLayer layer)
        {
            int result = 0;
            foreach (var So in Sources)
            {
                if(So.Layer == layer)
                {
                    result += So.Count;
                }
            }
            return result;
        }

    }
}

