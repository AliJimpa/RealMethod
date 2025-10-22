using UnityEngine;

namespace RealMethod
{
    public static class MonoBehaviour_Extension
    {
        public static void SpawnEvent(this MonoBehaviour owner, Object spawner)
        {
            owner.SendMessage("OnSpawn", spawner, SendMessageOptions.DontRequireReceiver);
        }
    }
}