using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Method/SharedObject")]
    public sealed class SharedObject : MonoBehaviour
    {
        [SerializeField]
        private bool CheckSaftyShare = false;
        private void OnEnable()
        {
            gameObject.Share(CheckSaftyShare);
        }

        private void OnDisable()
        {
            gameObject.Unshare(CheckSaftyShare);
        }
    }
}