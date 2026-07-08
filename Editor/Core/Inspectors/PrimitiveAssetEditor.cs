using UnityEditor;

namespace RealMethod.Editor
{
    /// <summary>
    /// Required for the fetching of a default editor on ScriptableObject objects.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PrimitiveAsset), true)]
    public class PrimitiveAssetEditor : UnityEditor.Editor { }
}