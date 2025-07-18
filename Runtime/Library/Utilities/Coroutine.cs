using System.Collections;
using UnityEngine;

namespace RealMethod
{
    public static class RM_Coroutine
    {
        public static IEnumerator Delay(float delaySeconds, System.Action callback)
        {
            yield return new WaitForSeconds(delaySeconds);
            callback?.Invoke();
        }
    }

}