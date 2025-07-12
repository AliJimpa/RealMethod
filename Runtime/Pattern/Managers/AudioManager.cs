using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace RealMethod
{

    public abstract class AudioManager : MixerManager
    {
        [Header("Audio")]
        [SerializeField]
        private AudioMixerGroup DefaultGroup;
        [SerializeField]
        private AudioClip[] DefaultStamp;



        private Transform SoundStorage;
        private Dictionary<string, AudioSource> Stamps;


        protected override void InitiateManager(bool AlwaysLoaded)
        {
            if (AlwaysLoaded)
            {
                Debug.LogError("You can't use Audiomanager in [Game] Scope");
                Destroy(this);
            }

            if (Game.TryFindService(out Spawn SpawnServ))
            {
                SpawnServ.BringManager(this);
            }

            SoundStorage = transform;


            if (DefaultStamp != null && DefaultStamp.Length > 0)
            {
                Stamps = new Dictionary<string, AudioSource>(DefaultStamp.Length);
                foreach (var clip in DefaultStamp)
                {
                    CreateStamp(clip.name, clip, DefaultGroup);
                }
            }
            else
            {
                Stamps = new Dictionary<string, AudioSource>();
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
        public AudioSource PlaySound2D(AudioClip clip, bool autoDestroy = true)
        {
            GameObject SoundObject = new GameObject("Audio_" + clip.name);
            SoundObject.transform.SetParent(SoundStorage);
            AudioSource source = SoundObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = 0;
            source.outputAudioMixerGroup = DefaultGroup;
            source.Play();
            if (autoDestroy)
            {
                StartCoroutine(DestroyAtEnd(source));
            }
            return source;
        }
        public AudioSource PlaySound2D(AudioClip clip, AudioMixerGroup group, bool autoDestroy = true)
        {
            GameObject SoundObject = new GameObject("Audio_" + clip.name);
            SoundObject.transform.SetParent(SoundStorage);
            AudioSource source = SoundObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = 0;
            source.outputAudioMixerGroup = group;
            source.Play();
            if (autoDestroy)
            {
                StartCoroutine(DestroyAtEnd(source));
            }
            return source;
        }
        public AudioSource PlaySound2D(AudioClip clip, float rolloffDistanceMin = 1f, bool loop = false, float pauseTime = 0, bool autoDestroy = true)
        {
            GameObject SoundObject = new GameObject("Audio_" + clip.name);
            SoundObject.transform.SetParent(SoundStorage);
            AudioSource source = SoundObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = 0;
            source.minDistance = rolloffDistanceMin;
            source.outputAudioMixerGroup = DefaultGroup;
            source.loop = loop;
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
        public AudioSource PlaySound2D(AudioClip clip, AudioMixerGroup group, float rolloffDistanceMin = 1f, bool loop = false, float pauseTime = 0, bool autoDestroy = true)
        {
            GameObject SoundObject = new GameObject("Audio_" + clip.name);
            SoundObject.transform.SetParent(SoundStorage);
            AudioSource source = SoundObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = 0;
            source.minDistance = rolloffDistanceMin;
            source.outputAudioMixerGroup = group;
            source.loop = loop;
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
        public AudioSource PlaySound(AudioClip clip, Vector3 location, bool autoDestroy = true)
        {
            GameObject SoundObject = new GameObject("Audio_" + clip.name);
            SoundObject.transform.position = location;
            AudioSource source = SoundObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = 1;
            source.outputAudioMixerGroup = DefaultGroup;
            SoundObject.transform.SetParent(SoundStorage);
            source.Play();
            if (autoDestroy)
            {
                StartCoroutine(DestroyAtEnd(source));
            }
            return source;
        }
        public AudioSource PlaySound(AudioClip clip, AudioMixerGroup group, Vector3 location, bool autoDestroy = true)
        {
            GameObject SoundObject = new GameObject("Audio_" + clip.name);
            SoundObject.transform.position = location;
            AudioSource source = SoundObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = 1;
            source.outputAudioMixerGroup = group;
            SoundObject.transform.SetParent(SoundStorage);
            source.Play();
            if (autoDestroy)
            {
                StartCoroutine(DestroyAtEnd(source));
            }
            return source;
        }
        public AudioSource PlaySound(AudioClip clip, AudioMixerGroup group, Vector3 location, float rolloffDistanceMin = 1f, bool loop = false, float pauseTime = 0, bool autoDestroy = true)
        {
            GameObject SoundObject = new GameObject("Audio_" + clip.name);
            SoundObject.transform.position = location;
            AudioSource source = SoundObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = 1;
            source.minDistance = rolloffDistanceMin;
            source.outputAudioMixerGroup = group;
            source.loop = loop;
            SoundObject.transform.SetParent(SoundStorage);
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
            GameObject SoundObject = new GameObject("Audio_" + clip.name);
            SoundObject.transform.position = location;
            AudioSource source = SoundObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = 1;
            source.minDistance = rolloffDistanceMin;
            source.outputAudioMixerGroup = DefaultGroup;
            source.loop = loop;
            SoundObject.transform.SetParent(SoundStorage);
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
        public bool HoldSource(AudioSource source)
        {
            if (source != null)
            {
                source.transform.SetParent(SoundStorage);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool UnHoldSource(AudioSource source)
        {
            if (source != null)
            {
                source.transform.SetParent(null);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void CreateStamp(string stampName, AudioClip clip, AudioMixerGroup group)
        {
            GameObject SoundObject = new GameObject("Audio_" + clip.name);
            SoundObject.transform.SetParent(SoundStorage);
            AudioSource source = SoundObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = 0;
            source.outputAudioMixerGroup = group;
            Stamps.Add(stampName, source);
        }
        public void PlayStamp(string stampName, bool PlayForce = false)
        {
            if (Stamps.ContainsKey(stampName))
            {
                if (Stamps[stampName].isPlaying)
                {
                    if (!PlayForce)
                        return;
                }
                Stamps[stampName].Play();
            }
            else
            {
                Debug.LogWarning($"Cant find Stamp with this name {stampName}");
            }
        }
        public void PauseStamp(string stampName)
        {
            if (Stamps.ContainsKey(stampName))
            {
                Stamps[stampName].Pause();
            }
            else
            {
                Debug.LogWarning($"Cant find Stamp with this name {stampName}");
            }
        }
        public void StopStamp(string stampName)
        {
            if (Stamps.ContainsKey(stampName))
            {
                Stamps[stampName].Stop();
            }
            else
            {
                Debug.LogWarning($"Cant find Stamp with this name {stampName}");
            }
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