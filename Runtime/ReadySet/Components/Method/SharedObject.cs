using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Method/SharedObject")]
    public sealed class SharedObject : MonoBehaviour
    {
        private void OnEnable()
        {
            gameObject.Share();
        }

        private void OnDisable()
        {
            gameObject.Unshare();
        }
    }
}