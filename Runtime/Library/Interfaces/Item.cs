using UnityEngine;

namespace RealMethod
{

    public interface IItem : IIdentifier
    {
        Texture2D Icon { get; }
        Sprite GetSpriteIcon();
        Sprite GetSpriteIcon(Rect rect, Vector2 pivot);
    }

    public interface IItemData : IItem
    {
        PrimitiveAsset GetAsset();
#if UNITY_EDITOR
        void ChangeName(string NewName);
#endif
    }


}