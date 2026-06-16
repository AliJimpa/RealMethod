using UnityEngine;

namespace RealMethod
{
    public class AutoDetach : MonoBehaviour
    {
        void Awake()
        {
            // Detach from parent
            transform.SetParent(null);
        }
    }
}