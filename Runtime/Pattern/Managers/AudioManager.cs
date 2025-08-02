using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

namespace RealMethod
{

    public abstract class AudioManager : MixerManager
    {
        [Header("Audio")]
        [SerializeField]
        private AudioMixerGroup DefaultGroup;
        public AudioMixerGroup defaultGroup => DefaultGroup;

        public ObjectPool<AudioSource> soundPool;

        protected override void InitiateManager(bool AlwaysLoaded)
        {
            if (AlwaysLoaded)
            {
                Debug.LogError("You can't use AudioManager in [Game] Scope");
                return;
            }

            if (CanBringtoSpawn() && Game.TryFindService(out Spawn SpawnServ))
            {
                SpawnServ.BringManager(this);
            }

            soundPool = new ObjectPool<AudioSource>(
            createFunc: CreateSource,
            actionOnGet: OnTakeSource,
            actionOnRelease: OnReturnedSource,
            actionOnDestroy: OnDestroySource,
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 500
        );
        }
        protected override void InitiateService(Service service)
        {
            if (CanBringtoSpawn() && service is Spawn SpawnServ)
            {
                SpawnServ.BringManager(this);
            }
        }

        // Public Functions
        public AudioSource PlaySound(AudioClip clip, Vector3 location, Transform parent = null, AudioMixerGroup group = null, float rolloffDistanceMin = 1f, bool loop = false, float pauseTime = 0, bool autoDestroy = true)
        {
            AudioSource source = soundPool.Get();
            source.clip = clip;
            source.outputAudioMixerGroup = group != null ? group : DefaultGroup;
            source.spatialBlend = 1;
            source.minDistance = rolloffDistanceMin;
            source.loop = loop;
            source.transform.SetParent(parent == null ? transform : parent);
            source.transform.localPosition = location;
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
        public AudioSource PlaySound2D(AudioClip clip, AudioMixerGroup group = null, float rolloffDistanceMin = 1f, bool loop = false, float pauseTime = 0, bool autoDestroy = true)
        {
            AudioSource source = soundPool.Get();
            source.clip = clip;
            source.outputAudioMixerGroup = group != null ? group : DefaultGroup;
            source.spatialBlend = 0;
            source.minDistance = rolloffDistanceMin;
            source.loop = loop;
            source.transform.SetParent(transform);
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
        public void Clear()
        {
            soundPool.Clear();
        }

        // Private Functions
        private AudioSource CreateSource()
        {
            GameObject SoundObject = new GameObject($"Sound_{soundPool.CountAll + 1}", new System.Type[1] { typeof(AudioSource) });
            return SoundObject.GetComponent<AudioSource>(); ;
        }
        private void OnTakeSource(AudioSource source)
        {
            source.gameObject.SetActive(true);
        }
        private void OnReturnedSource(AudioSource source)
        {
            source.Stop();
            source.clip = null;
            source.gameObject.SetActive(false);
        }
        private void OnDestroySource(AudioSource source)
        {
            if (source != null)
                Destroy(source.gameObject);
        }

        // Abstract Methods
        protected abstract bool CanBringtoSpawn();

        // Enumerator Methods
        private IEnumerator PauseAfterSecond(AudioSource source, float time)
        {
            yield return new WaitForSeconds(time);
            source.Pause();
        }
        private IEnumerator DestroyAtEnd(AudioSource source)
        {
            yield return new WaitForSeconds(source.clip.length);
            soundPool.Release(source);
        }

    }
}