using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/General/DestroyAfterDelay")]
    public sealed class DestroyAfterDelay : MonoBehaviour, IInitializableWithArgument<float>
    {
        public float Delay = 5;

        public void Initialize(float argument)
        {
            Delay = argument;
        }

        void Start()
        {
            Destroy(gameObject, Delay);
        }
    }

}
