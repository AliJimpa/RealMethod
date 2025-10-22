using UnityEngine;

namespace RealMethod
{
    public interface IItem : IIdentifier
    {
        Texture2D Icon { get; }
        Sprite GetSpriteIcon();
        Sprite GetSpriteIcon(Rect rect, Vector2 pivot);
#if UNITY_EDITOR
        void ChangeItemName(string NewName);
#endif
    }

}