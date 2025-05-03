using UnityEngine;
using UnityEngine.Audio;

namespace RealMethod
{
    static class Spawn
    {
        public static void SFX(AudioClip clip, Vector3 position, AudioMixerGroup audioGroup, float spatialBlend, float rolloffDistanceMin = 1f)
        {
            GameObject impactSfxInstance = new GameObject();
            impactSfxInstance.name = "Audio_" + clip.name;
            impactSfxInstance.transform.position = position;
            AudioSource source = impactSfxInstance.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = spatialBlend;
            source.minDistance = rolloffDistanceMin;
            source.outputAudioMixerGroup = audioGroup;
            source.Play();

            DestroyAfterDelay timedSelfDestruct = impactSfxInstance.AddComponent<DestroyAfterDelay, float>(clip.length);
        }
        public static T Class<T>() where T : MonoBehaviour
        {
            GameObject gameobject = new GameObject();
            return gameobject.AddComponent<T>();
        }
    }
}