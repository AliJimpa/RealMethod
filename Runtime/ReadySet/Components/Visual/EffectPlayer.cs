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

        // Unity Methods
        private void Awake()
        {
            AllParticles = GetComponentsInChildren<ParticleSystem>();
            AllAudioSources = GetComponentsInChildren<AudioSource>();
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
            Stop();
        }

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


#if UNITY_EDITOR
        [ContextMenu("AutoFix")]
        private void Fix()
        {
            foreach (var aud in GetComponentsInChildren<AudioSource>())
            {
                aud.playOnAwake = false;
            }
        }
#endif

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
        //private Vector2 scrollPos;
        private Color prevColor;


        private ParticleSystem[] AllParticles;
        private AudioSource[] AllAudioSources;
        private VisualEffect[] AllVisualEffects;
        private ICommand[] AllExecutCommands;

        private void OnEnable()
        {
            MyPlayer = (EffectPlayer)target;
            AllParticles = MyPlayer.GetComponentsInChildren<ParticleSystem>();
            AllAudioSources = MyPlayer.GetComponentsInChildren<AudioSource>();
            AllVisualEffects = MyPlayer.GetComponentsInChildren<VisualEffect>();
            AllExecutCommands = MyPlayer.GetComponents<ICommand>();
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
            prevColor = GUI.contentColor;
            EditorGUILayout.Space();
            //scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(400), GUILayout.Height(150));
            EditorGUILayout.LabelField("1.ParticleSystem", EditorStyles.boldLabel);
            foreach (var parti in AllParticles)
            {
                GUI.contentColor = parti.main.playOnAwake ? Color.red : prevColor;
                EditorGUILayout.LabelField($"{parti.gameObject.name}");
            }
            EditorGUILayout.LabelField("2.AudioSource", EditorStyles.boldLabel);
            foreach (var audi in AllAudioSources)
            {
                GUI.contentColor = audi.playOnAwake ? Color.red : prevColor;
                EditorGUILayout.LabelField($"{audi.gameObject.name}");
            }
            EditorGUILayout.LabelField("3.VisualEffect", EditorStyles.boldLabel);
            foreach (var vis in AllVisualEffects)
            {
                GUI.contentColor = prevColor;
                EditorGUILayout.LabelField($"{vis.gameObject.name}");
            }
            EditorGUILayout.LabelField("4.Command", EditorStyles.boldLabel);
            foreach (var com in AllExecutCommands)
            {
                GUI.contentColor = prevColor;
                EditorGUILayout.LabelField($"{com.GetType()}");
            }
            //EditorGUILayout.EndScrollView();
        }




    }
#endif


}