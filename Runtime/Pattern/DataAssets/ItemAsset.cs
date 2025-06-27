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

        // Functions
        public virtual Sprite GetSpriteIcon()
        {
            return GetSpriteIcon(new Rect(0, 0, _icon.width, _icon.height),
                    new Vector2(0.5f, 0.5f)
                );
        }
        public virtual Sprite GetSpriteIcon(Rect rect, Vector2 pivot)
        {
            if (_icon != null)
            {
                return Sprite.Create(_icon, rect, pivot);
            }
            else
            {
                Debug.LogWarning("ItemAsset: Icon is not assigned for item '" + _name + "'.");
                return null;
            }
        }
#if UNITY_EDITOR
        protected void ChangeName(string NewName)
        {
            _name = NewName;
        }
#endif

    }
}