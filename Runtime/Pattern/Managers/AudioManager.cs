using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace RealMethod
{

    public abstract class AudioManager : MixerManager
    {
        [Header("Audio")]
        [SerializeField]
        private AudioMixerGroup DefaultGroup;
        public AudioMixerGroup defaultGroup => DefaultGroup;

        protected override void InitiateManager(bool AlwaysLoaded)
        {
            if (AlwaysLoaded)
            {
                Debug.LogError("You can't use AudioManager in [Game] Scope");
                return;
            }

            if (Game.TryFindService(out Spawn SpawnServ))
            {
                SpawnServ.BringManager(this);
            }
        }
        protected override void InitiateService(Service service)
        {
            if (service is Spawn SpawnServ)
            {
                SpawnServ.BringManager(this);
            }
        }

        // Public Methods
        public AudioSource PlaySound(AudioClip clip, AudioMixerGroup group, Vector3 location, float rolloffDistanceMin = 1f, bool loop = false, float pauseTime = 0, bool autoDestroy = true)
        {
            GameObject SoundObject = new GameObject("Audio3D_" + clip.name, new System.Type[1] { typeof(AudioSource) });
            AudioSource source = SoundObject.GetComponent<AudioSource>();
            SoundObject.transform.position = location;
            source.clip = clip;
            source.outputAudioMixerGroup = group;
            source.spatialBlend = 1;
            source.minDistance = rolloffDistanceMin;
            source.loop = loop;
            SoundObject.transform.SetParent(transform);
            source.Play();
            if (autoDestroy)
            {
                StartCoroutine(DestroyAtEnd(source));
            }
            else
            {
                if (!loop && pauseTime > 0)
                {
                    StartCoroutine(PauseAfterSecond(source, pauseTime));
                }
            }
            return source;
        }
        public AudioSource PlaySound(AudioClip clip, Vector3 location, float rolloffDistanceMin = 1f, bool loop = false, float pauseTime = 0, bool autoDestroy = true)
        {
            return PlaySound(clip, DefaultGroup, location, rolloffDistanceMin, loop, pauseTime, autoDestroy);
        }
        public AudioSource PlaySound(AudioClip clip, AudioMixerGroup group, Vector3 location, bool autoDestroy = true)
        {
            return PlaySound(clip, group, location, autoDestroy);
        }
        public AudioSource PlaySound(AudioClip clip, Vector3 location, bool autoDestroy = true)
        {
            return PlaySound(clip, DefaultGroup, location, autoDestroy);
        }
        public AudioSource PlaySound2D(AudioClip clip, AudioMixerGroup group, float rolloffDistanceMin = 1f, bool loop = false, float pauseTime = 0, bool autoDestroy = true)
        {
            GameObject SoundObject = new GameObject("Audio2D_" + clip.name, new System.Type[1] { typeof(AudioSource) });
            AudioSource source = SoundObject.GetComponent<AudioSource>();
            source.clip = clip;
            source.outputAudioMixerGroup = group;
            source.spatialBlend = 0;
            source.minDistance = rolloffDistanceMin;
            source.loop = loop;
            SoundObject.transform.SetParent(transform);
            source.Play();
            if (autoDestroy)
            {
                StartCoroutine(DestroyAtEnd(source));
            }
            else
            {
                if (!loop && pauseTime > 0)
                {
                    StartCoroutine(PauseAfterSecond(source, pauseTime));
                }
            }
            return source;
        }
        public AudioSource PlaySound2D(AudioClip clip, float rolloffDistanceMin = 1f, bool loop = false, float pauseTime = 0, bool autoDestroy = true)
        {
            return PlaySound2D(clip, DefaultGroup, rolloffDistanceMin, loop, pauseTime, autoDestroy);
        }
        public AudioSource PlaySound2D(AudioClip clip, AudioMixerGroup group, bool autoDestroy = true)
        {
            return PlaySound2D(clip, group, autoDestroy);
        }
        public AudioSource PlaySound2D(AudioClip clip, bool autoDestroy = true)
        {
            return PlaySound2D(clip, DefaultGroup, autoDestroy);
        }

        // Enumerator Methods
        private IEnumerator PauseAfterSecond(AudioSource source, float time)
        {
            yield return new WaitForSeconds(time);
            source.Pause();
        }
        private IEnumerator DestroyAtEnd(AudioSource source)
        {
            yield return new WaitForSeconds(source.clip.length);
            Destroy(source.gameObject);
        }

    }
}