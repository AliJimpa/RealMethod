using UnityEngine;

namespace RealMethod
{
    public interface IItem : IIdentifier
    {
        Texture2D Icon { get; }
        Sprite GetSpriteIcon();
        Sprite GetSpriteIcon(Rect rect, Vector2 pivot);
        T GetClass<T>() where T : DataAsset;

#if UNITY_EDITOR
        void ChangeName(string NewName);
#endif
    }


}