using UnityEngine;
using UnityEngine.VFX;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Visual/Effector")]
    public sealed class Effector : MonoBehaviour
    {
        [Header("Effect")]
        [SerializeField]
        private bool PlayOnStart = false;

        private ParticleSystem[] AllParticles;
        private AudioSource[] AllAudioSources;
        private VisualEffect[] AllVisualEffects;
        private ICommand[] AllExecutCommands;

        /// Haptic
        /// UI
        /// Animation
        /// AutoDestroy
        /// Notify
        /// CameraShake

        // Unity Methods
        private void Awake()
        {
            AllParticles = GetComponentsInChildren<ParticleSystem>();
            foreach (var part in AllParticles)
            {
                if (part.isPlaying)
                {
                    Debug.LogWarning($"{part} autoplay is enable ");
                }
            }
            AllAudioSources = GetComponentsInChildren<AudioSource>();
            foreach (var aud in AllAudioSources)
            {
                if (aud.playOnAwake)
                {
                    Debug.LogWarning($"{aud} autoplay is enable ");
                }
            }
            AllVisualEffects = GetComponentsInChildren<VisualEffect>();
            AllExecutCommands = GetComponents<ICommand>();
            ICommand[] CommandInitators = GetComponents<ICommand>();
            foreach (var com in CommandInitators)
            {
                com.Initiate(this, this);
            }
        }
        private void Start()
        {
            if (PlayOnStart)
            {
                Play();
            }
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (var aud in GetComponentsInChildren<AudioSource>())
            {
                aud.playOnAwake = false;
            }
        }
#endif

        // Public Functions
        public void Play()
        {
            foreach (var part in AllParticles)
            {
                part.Play();
            }
            foreach (var aud in AllAudioSources)
            {
                aud.Play();
            }
            foreach (var vef in AllVisualEffects)
            {
                vef.Play();
            }
            foreach (var com in AllExecutCommands)
            {
                com.ExecuteCommand(this);
            }
        }
        public void Stop()
        {
            foreach (var part in AllParticles)
            {
                part.Pause();
            }
            foreach (var aud in AllAudioSources)
            {
                aud.Pause();
            }
            foreach (var vef in AllVisualEffects)
            {
                vef.pause = false;
            }
        }
        public void Clear()
        {
            foreach (var part in AllParticles)
            {
                part.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            foreach (var aud in AllAudioSources)
            {
                aud.Stop();
            }
            foreach (var vef in AllVisualEffects)
            {
                vef.Reinit();
            }
        }

    }


    [System.Serializable]
    public class EPrefab : PrefabCore<Effector>
    {

    }

    
}