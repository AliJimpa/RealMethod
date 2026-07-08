
namespace RealMethod
{
    public enum DrawMode
    {
        GUI,
        Gizmo,
    }

    public interface IDraw : IIdentifier
    {
        bool CanDraw(int Index);
        void Draw(int Index);
    }

    public interface IDrawTask : IDraw, ITask
    {
        int Priority { get; }
        bool IsExpired();
        DrawMode GetDrawMode();
    }


}