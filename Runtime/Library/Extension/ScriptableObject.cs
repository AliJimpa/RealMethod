using UnityEngine;

namespace RealMethod
{
    public static class ScriptableObject_Extension
    {
        public static ScriptableObject Clone(this ScriptableObject so)
        {
            return ScriptableObject.Instantiate(so);
        }

    }
}