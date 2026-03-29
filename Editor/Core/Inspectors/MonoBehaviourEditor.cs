using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    /// <summary>
    /// Required for the fetching of a default editor on MonoBehaviour objects.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class MonoBehaviourEditor : UnityEditor.Editor
    {

    }
}