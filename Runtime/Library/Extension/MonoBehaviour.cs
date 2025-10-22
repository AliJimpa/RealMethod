using UnityEngine;

namespace RealMethod
{
    public static class MonoBehaviour_Extension
    {
        public static void Event_Spawn(this MonoBehaviour owner, Object spawner)
        {
            owner.SendMessage("OnSpawn", spawner, SendMessageOptions.DontRequireReceiver);
        }
    }
}