using System.Collections;
using UnityEngine;

namespace RealMethod
{
    public static class RM_Audio
    {
        public static IEnumerator FadeIn(AudioSource source, float duration)
        {
            source.Play();
            source.volume = 0f; // Start volume at 0
            float timer = 0f;
            while (timer < duration)
            {
                float t = timer / duration;
                source.volume = Mathf.Lerp(0f, 1f, t);
                timer += Time.deltaTime;
                yield return null;
            }
            source.volume = 1f; // Ensure volume is set to 1 at the end
        }
        public static IEnumerator FadeOut(AudioSource source, float duration)
        {
            source.volume = 1f; // Start volume at 1
            float timer = 0f;
            while (timer < duration)
            {
                float t = timer / duration;
                source.volume = Mathf.Lerp(1f, 0f, t);
                timer += Time.deltaTime;
                yield return null;
            }
            source.volume = 0f; // Ensure volume is set to 0 at the end
            source.Stop();
        }
    }
}