using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Essential/DefaultWorld")]
    public sealed class DefaultWorld : World
    {
        protected override void AwakeWorld()
        {
            Debug.Log("DefaultWorld Awaked");
        }
        protected override void DestroyWorld()
        {
            Debug.Log("DefaultWorld Destroyed");
        }
    }
}