using UnityEngine;

namespace RealMethod
{
    public interface IDraw : IIdentifier
    {
        bool Start(IGameManager Manager);
        bool CanDraw();
        void Draw(Vector2 Pivot, int Index);
        void End();
    }
}