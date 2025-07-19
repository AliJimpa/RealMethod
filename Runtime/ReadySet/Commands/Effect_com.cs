using UnityEngine;
using UnityEngine.VFX;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Commands/Action/Effect")]
    public sealed class Effect_com : MonoBehaviour, ICommandLife
    {
        [Header("Effect")]
        [SerializeField]
        private bool PlayOnStart = false;


        private ParticleSystem[] AllParticles;
        private AudioSource[] AllAudioSources;
        private VisualEffect[] AllVisualEffects;
        private ICommandExecuter[] AllCommands;

        float ICommandLife.RemainingTime => 0;
        bool ICommandLife.IsFinished => false;

        void ICommandLife.StartCommand(float Duration)
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
            foreach (var com in AllCommands)
            {
                com.ExecuteCommand(this);
            }
        }
        void ICommandLife.UpdateCommand()
        {

        }
        void ICommandLife.StopCommand()
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
        void ICommandLife.ClearCommand()
        {
            foreach (var part in AllParticles)
            {
                part.Stop(false,ParticleSystemStopBehavior.StopEmittingAndClear);
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
            AllCommands = GetComponents<ICommandExecuter>();
            ICommandInitiator[] CommandInitators = GetComponents<ICommandInitiator>();
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
            ((ICommandLife)this).StartCommand();
        }
        public void Stop()
        {
            ((ICommandLife)this).StopCommand();
        }
        public void Clear()
        {
            ((ICommandLife)this).ClearCommand();
        }


    }
}