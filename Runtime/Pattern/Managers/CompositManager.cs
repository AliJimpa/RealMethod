using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace RealMethod
{
    public abstract class CompositManager : MixerManager
    {
        public struct MusicLerp
        {
            private AudioSource LayerA;
            private AudioSource LayerB;

            public MusicLerp(AudioSource A, AudioSource B)
            {
                LayerA = A;
                LayerB = B;
            }

            public void Volume(float alpha)
            {
                LayerA.volume = Mathf.Lerp(1f, 0f, alpha);
                LayerB.volume = Mathf.Lerp(0f, 1f, alpha);
            }
            public void Pitch(float alpha, float minPitch = -3f, float maxPitch = 3f)
            {
                LayerA.pitch = Mathf.Lerp(maxPitch, minPitch, alpha);
                LayerB.pitch = Mathf.Lerp(minPitch, maxPitch, alpha);
            }
        }
        [Header("Composit")]
        [SerializeField]
        private Transform header;
        [SerializeField]
        private AudioMixerGroup DefaultGroup;

        protected NameTable<AudioBehaviour> Layers = new NameTable<AudioBehaviour>(5);
        public int LayerCount => Layers.Count;

        // Override Methods
        protected override void InitiateManager(bool AlwaysLoaded)
        {
            base.InitiateManager(AlwaysLoaded);

            foreach (Transform item in header)
            {
                AudioSource source = item.GetComponent<AudioSource>();
                if (source != null)
                {
                    if (!Layers.ContainsKey(item.name))
                    {
                        Layers.Add(item.name, source);
                    }
                    else
                    {
                        Debug.LogWarning($"Layer '{item.name}' already exists in the manager.");
                    }
                }
            }
        }

        // Public Functions
        public void PlayLayer(Name16 layerName)
        {
            if (Layers.ContainsKey(layerName))
            {
                AudioSource source = Layers[layerName] as AudioSource;
                if (source != null)
                {
                    source.Play();
                }
                else
                {
                    Debug.LogWarning($"Layer '{layerName}' is not an AudioSource.");
                }
            }
            else
            {
                Debug.LogWarning($"Layer '{layerName}' does not exist in the manager.");
            }
        }
        public void PauseLayer(Name16 layerName)
        {
            if (Layers.ContainsKey(layerName))
            {
                AudioSource source = Layers[layerName] as AudioSource;
                if (source != null)
                {
                    source.Pause();
                }
                else
                {
                    Debug.LogWarning($"Layer '{layerName}' is not an AudioSource.");
                }
            }
            else
            {
                Debug.LogWarning($"Layer '{layerName}' does not exist in the manager.");
            }
        }
        public void Pause()
        {
            foreach (var layer in Layers)
            {
                if (layer.Value is AudioSource source)
                {
                    source.Pause();
                }
            }
        }
        public bool IsValidLayer(Name16 layerName)
        {
            return Layers.ContainsKey(layerName);
        }
        public void AddLayer(Name16 layerName, APrefab prefab)
        {
            if (IsValidLayer(layerName))
            {
                Debug.LogWarning($"Layer '{layerName}' already exists. Use a different name.");
                return;
            }
            AudioSource source = Instantiate<GameObject>(prefab, header).GetComponent<AudioSource>();
            Layers.Add(layerName, source);
        }
        public void CreateLayer(AudioClip clip)
        {
            CreateLayer(clip.name, clip);
        }
        public void CreateLayer(Name16 layerName, AudioClip clip)
        {
            if (IsValidLayer(layerName))
            {
                Debug.LogWarning($"Layer '{layerName}' already exists. Use a different name.");
                return;
            }
            GameObject SoundObject = new GameObject("Layer_" + clip.name, new Type[1] { typeof(AudioSource) });
            SoundObject.transform.SetParent(header);
            AudioSource source = SoundObject.GetComponent<AudioSource>();
            source.clip = clip;
            source.outputAudioMixerGroup = DefaultGroup;
            source.spatialBlend = 0;
            source.playOnAwake = false;
            source.volume = 1f; // Default volume
            source.loop = true;
            Layers.Add(layerName, source);
        }
        public void RemoveLayer(Name16 layerName)
        {
            if (Layers.ContainsKey(layerName))
            {
                Destroy(Layers[layerName].gameObject);
                Layers.Remove(layerName);
            }
            else
            {
                Debug.LogWarning($"Layer '{layerName}' does not exist in the manager.");
            }
        }
        public void FadeInLayer(Name16 layerName, float duration)
        {
            if (Layers.ContainsKey(layerName))
            {
                AudioSource source = Layers[layerName] as AudioSource;
                if (source != null)
                {
                    StartCoroutine(RM_Audio.FadeIn(source, duration));
                }
                else
                {
                    Debug.LogWarning($"Cannot fade in layer: {layerName} Should AudioSource.");
                }
            }
            else
            {
                Debug.LogWarning($"Cannot fade in layer: {layerName} not found.");
            }
        }
        public void FadeOutLayer(Name16 layerName, float duration)
        {
            if (Layers.ContainsKey(layerName))
            {
                AudioSource source = Layers[layerName] as AudioSource;
                if (source != null)
                {
                    StartCoroutine(RM_Audio.FadeOut(source, duration));
                }
                else
                {
                    Debug.LogWarning($"Cannot fade out layer: {layerName} Should AudioSource.");
                }
            }
            else
            {
                Debug.LogWarning($"Cannot fade out layer: {layerName} not found.");
            }
        }
        public void CrossfadeLayer(Name16 layerA, Name16 layerB, float duration)
        {
            if (Layers.ContainsKey(layerA) && Layers.ContainsKey(layerB))
            {
                AudioSource sourceA = Layers[layerA] as AudioSource;
                AudioSource sourceB = Layers[layerB] as AudioSource;
                if (sourceA != null && sourceB != null)
                {
                    StartCoroutine(RM_Audio.FadeIn(sourceB, duration));
                    StartCoroutine(RM_Audio.FadeOut(sourceA, duration));
                }
                else
                {
                    Debug.LogWarning($"Cannot crossfade layers: {layerA} and {layerB} Should AudioSource.");
                }
            }
            else
            {
                Debug.LogWarning($"Cannot crossfade layers: {layerA} or {layerB} not found.");
            }
        }
        public AudioSource[] GetActiveLayers()
        {
            List<AudioSource> Result = new List<AudioSource>();
            foreach (var layer in Layers)
            {
                if (layer.Value is AudioSource source)
                {
                    if (source.isPlaying)
                    {
                        Result.Add(source);
                    }
                }
            }
            return Result.ToArray();
        }
        public T GetLayer<T>(Name16 layerName) where T : AudioBehaviour
        {
            if (Layers.ContainsKey(layerName))
            {
                return Layers[layerName].GetComponent<T>();
            }
            else
            {
                Debug.LogWarning($"Layer '{layerName}' does not exist in the manager.");
                return null;
            }
        }
        public AudioSource GetLayer(Name16 layerName)
        {
            if (Layers.ContainsKey(layerName))
            {
                return Layers[layerName] as AudioSource;
            }
            else
            {
                Debug.LogWarning($"Layer '{layerName}' does not exist in the manager.");
                return null;
            }
        }
        public AudioBehaviour[] GetLayers()
        {
            return Layers.Values.ToArray();
        }

        public MusicLerp CreateLerp(Name16 LayerA, Name16 LayerB)
        {
            if (!Layers.ContainsKey(LayerA) || !Layers.ContainsKey(LayerB))
            {
                Debug.LogWarning($"Cannot create lerp: {LayerA} or {LayerB} not found.");
                return default;
            }
            return new MusicLerp((AudioSource)Layers[LayerA], (AudioSource)Layers[LayerB]);
        }

    }

    public abstract class CompositManager<T> : CompositManager
    {
        [Serializable]
        protected struct MusicState
        {
            [SerializeField]
            private bool DeactivePreviousLayers;
            [SerializeField]
            private Name16[] ActiveLayers;
            [SerializeField]
            private bool FadingMethod;
            [SerializeField, ConditionalHide("FadingMethod", true, false)]
            private float Duration;
            [Space]
            [SerializeField]
            private bool ApplySnapshot;
            [SerializeField, ConditionalHide("ApplySnapshot", true, false)]
            public AudioMixerSnapshot shot;


            public void Play(CompositManager manager)
            {
                if (DeactivePreviousLayers)
                {
                    AudioSource[] layers = manager.GetActiveLayers();
                    foreach (var layer in layers)
                    {
                        if (FadingMethod)
                        {
                            manager.StartCoroutine(RM_Audio.FadeOut(layer, Duration));
                        }
                        else
                        {
                            layer.Stop();
                        }

                    }
                }

                foreach (var layer in ActiveLayers)
                {
                    if (FadingMethod)
                    {
                        manager.FadeInLayer(layer, Duration);
                    }
                    else
                    {
                        manager.PauseLayer(layer);
                    }
                }

                if (ApplySnapshot)
                {
                    manager.TransitionToSnapshot(shot, Duration);
                }
            }
        }

        [Header("State")]
        [field: SerializeField]
        protected bool PlayOnStart { get; private set; } = true;
        [field: SerializeField]
        public T CurrentState { get; protected set; }
        [SerializeField]
        protected Map<T, MusicState> StateBehavior = new Map<T, MusicState>();

        public event Action<T> OnMusicChange;

        // Unity
        protected virtual void Start()
        {
            if (PlayOnStart)
            {
                PlayState(CurrentState, false);
            }
        }

        // Public Functions
        public void PlayState(T state, bool CompairState = true)
        {
            if (CompairState)
            {
                if (CompairStates(state, CurrentState))
                {
                    Debug.LogWarning($"State {state} already is playing");
                    return;
                }
            }
            if (TryFindState(state, out MusicState Music))
            {
                Music.Play(this);
                CurrentState = state;
                OnMusicChange?.Invoke(CurrentState);
            }
            else
            {
                Debug.LogWarning($"State ({state}) is not valid in the music state behavior.");
            }
        }
        public bool IsValidState(T TargetState)
        {
            return StateBehavior.ContainsKey(TargetState);
        }

        // Private Functions
        protected bool TryFindState(T TargetState, out MusicState result)
        {
            if (StateBehavior.ContainsKey(TargetState))
            {
                result = StateBehavior[TargetState];
                return true;
            }
            result = default;
            return false;
        }

        // Abstraction Methods
        protected abstract bool CompairStates(T State_A, T State_B);
    }

}