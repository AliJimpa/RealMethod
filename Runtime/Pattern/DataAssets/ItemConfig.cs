using UnityEngine;

namespace RealMethod
{
    public abstract class ItemConfig : ConfigAsset, IItemData
    {
        [Header("Item")]
        [SerializeField]
        protected string itemName;
        public string NameID => itemName;
        [SerializeField]
        protected Texture2D _icon;
        public Texture2D Icon => _icon;

        // Implement IItemData Interface
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
                Debug.LogWarning("ItemAsset: Icon is not assigned for item '" + itemName + "'.");
                return null;
            }
        }
        PrimitiveAsset IItemData.GetAsset()
        {
            return this;
        }


#if UNITY_EDITOR
        public void ChangeName(string NewName)
        {
            itemName = NewName;
        }
#endif

    }
}