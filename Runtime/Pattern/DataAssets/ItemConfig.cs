using UnityEngine;

namespace RealMethod
{
    public abstract class ItemConfig : ConfigAsset, IItem
    {
        [Header("Item")]
        [SerializeField]
        protected string itemName;
        public string NameID => itemName;
        [SerializeField]
        protected Texture2D _icon;
        public Texture2D Icon => _icon;

        // Implement IItemData Interface
        Sprite IItem.GetSpriteIcon()
        {
            return ((IItem)this).GetSpriteIcon(new Rect(0, 0, _icon.width, _icon.height),
                    new Vector2(0.5f, 0.5f)
                );
        }
        Sprite IItem.GetSpriteIcon(Rect rect, Vector2 pivot)
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


#if UNITY_EDITOR
        void IItem.ChangeItemName(string NewName)
        {
            itemName = NewName;
        }
#endif

    }
}