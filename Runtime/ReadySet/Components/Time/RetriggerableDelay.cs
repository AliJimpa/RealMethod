using System;
using System.Collections;
using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Time/RetriggerableDelay")]
    public sealed class RetriggerableDelay : MonoBehaviour
    {
        public Action DelayPerfrom;
        public Action<float> DelayUpdate;
        private Coroutine delayCoroutine;
        private float delayTime = 2f;
        private float elapsedTime = 0f;

        public RetriggerableDelay(float Duration)
        {
            delayTime = Duration;
        }
        public RetriggerableDelay(float Duration, out Action action)
        {
            delayTime = Duration;
            action = DelayPerfrom;
        }

        private void Update()
        {
            if (delayCoroutine != null)
            {
                elapsedTime += Time.deltaTime;
                DelayUpdate?.Invoke(Mathf.Clamp(elapsedTime, 0, delayTime));
            }
        }

        // This is the action to call after the delay
        private void ActionToPerform()
        {
            delayCoroutine = null;
            DelayPerfrom?.Invoke();
        }

        // Call this method to trigger (or retrigger) the delay
        public void TriggerDelay()
        {
            if (delayCoroutine != null)
            {
                // If a delay is already running, stop it
                StopCoroutine(delayCoroutine);
            }

            // Start a new delay
            delayCoroutine = StartCoroutine(DelayedAction(delayTime));
        }
        public void TriggerDelay(float Duration)
        {
            delayTime = Duration;
            if (delayCoroutine != null)
            {
                // If a delay is already running, stop it
                StopCoroutine(delayCoroutine);
            }

            // Start a new delay
            delayCoroutine = StartCoroutine(DelayedAction(delayTime));
        }


        // The coroutine that handles the delay
        private IEnumerator DelayedAction(float delay)
        {
            elapsedTime = 0;
            // Wait for the specified delay time
            yield return new WaitForSeconds(delay);
            // Perform the action after the delay
            ActionToPerform();
        }
    }


}