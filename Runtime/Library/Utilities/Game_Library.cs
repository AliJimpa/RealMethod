#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RealMethod
{
    public static class RM_Game
    {
        public static bool IsPlaying
        {
            get
            {
#if UNITY_EDITOR
                return EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode == false;
#else
        return Application.isPlaying; // Runtime fallback
#endif
            }
        }
    }
}