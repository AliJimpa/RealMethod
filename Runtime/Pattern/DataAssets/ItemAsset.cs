using UnityEngine;

namespace RealMethod
{
    public abstract class ItemAsset : DataAsset
    {
        [Header("Basic")]
        [SerializeField]
        protected string _name;
        public string Name => _name;
        [SerializeField]
        protected Texture2D _icon;
        public Texture2D Icon => _icon;
    }
}