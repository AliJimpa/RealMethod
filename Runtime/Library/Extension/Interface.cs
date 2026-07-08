using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// Extension methods for the Interface to add common functionalities.
    /// </summary>
    public static class Interface_Extension
    {
        public static bool IsSame(this IIdentifier a, IIdentifier b)
        {
            // 1. Exactly same reference
            if (ReferenceEquals(a, b))
                return true;

            // 2. If one is null
            if (a is null || b is null)
                return false;

            return Equals(a.Self, b.Self);
        }
        public static bool HasNameID(this INameIdentifier target, string Name)
        {
            if (Name == string.Empty)
            {
                Debug.LogWarning("HasNameID is false [Name is empty]");
                return false;
            }
            if (target.SelfName == string.Empty)
            {
                Debug.LogWarning("HasNameID is false [NameID is empty]");
                return false;
            }
            return target.SelfName == Name;
        }
    }
}