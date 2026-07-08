namespace RealMethod
{
    /// <summary>
    /// Base configuration asset for game-wide settings and initialization logic.
    /// </summary>
    public abstract class GameConfig : ConfigAsset, IInspectorInfo
    {
#if UNITY_EDITOR
        string IInspectorInfo.GetInfo()
        {
            return GetInspectorInfo();
        }
        protected virtual string GetInspectorInfo()
        {
            return null;
        }
#endif
    }
}