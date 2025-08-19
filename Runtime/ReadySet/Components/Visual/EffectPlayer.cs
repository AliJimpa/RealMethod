using UnityEngine;
using UnityEngine.VFX;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Visual/EffectPlayer")]
    public sealed class EffectPlayer : MonoBehaviour
    {
        public bool IsPlaying { get; private set; }
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
        private void OnEnable()
        {
            Play();
        }
        private void OnDisable()
        {
            Pause();
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
            IsPlaying = true;
        }
        public void Pause()
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
            IsPlaying = false;
        }
        public void Stop()
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
            IsPlaying = false;
        }

    }
    [System.Serializable]
    public class EPrefab : PrefabCore<EffectPlayer>
    {

    }



#if UNITY_EDITOR
    [CustomEditor(typeof(EffectPlayer), true)]
    public class EffectPlayerEditor : Editor
    {
        private EffectPlayer MyPlayer;

        private void OnEnable()
        {
            MyPlayer = (EffectPlayer)target;
        }
        private void OnDisable()
        {

        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw default inspector
            DrawDefaultInspector();

            // Dropdown
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();

            GUILayout.EndHorizontal();
            //EditorGUILayout.IntField("Count:", EffectList.GetCount());
        }




    }
#endif


}