using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Time/DestroyAfterDelay")]
    public sealed class DestroyAfterDelay : MonoBehaviour, IInitializableWithArgument<float>
    {
        public float Delay = 5;

        // Implement IInitializableWithArgument Interface
        public void Initialize(float argument)
        {
            Delay = argument;
        }

        // Unity Methods
        private void Start()
        {
            Destroy(gameObject, Delay);
        }
    }

}
