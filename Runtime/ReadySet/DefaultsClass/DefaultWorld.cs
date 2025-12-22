using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Essential/DefaultWorld")]
    public sealed class DefaultWorld : World
    {
        protected override void WorldBegin()
        {
            Debug.Log("DefaultWorld Begin");
        }
        protected override void WorldEnd()
        {
            Debug.Log("DefaultWorld End");
        }
    }
}