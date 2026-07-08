namespace RealMethod
{
    public interface IInspectorInfo
    {
#if UNITY_EDITOR
        string GetTitleInfo() => GetType().Name;
        /// <summary>
        /// Returns a formatted string containing information about any object can show properties
        /// intended for display in the Unity Inspector or debugging interfaces.
        /// This method is for visualization purposes only and must not affect logic.
        /// </summary>
        /// <returns>Formatted display information.</returns>
        string GetInfo();
#endif
    }
}