using System;
using System.Collections;
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
        private AudioMixerGroup DefaultGroup;

        private Hictionary<AudioBehaviour> Layers = new Hictionary<AudioBehaviour>(5);
        public int LayerCount => Layers.Count;

        // Public Functions
        public void PlayLayer(string layerName)
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
        public void PauseLayer(string layerName)
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
        public bool IsValidLayer(string layerName)
        {
            return Layers.ContainsKey(layerName);
        }
        public void AddLayer(string layerName, APrefab prefab)
        {
            if (IsValidLayer(layerName))
            {
                Debug.LogWarning($"Layer '{layerName}' already exists. Use a different name.");
                return;
            }
            AudioSource source = Instantiate(prefab.asset, transform).GetComponent<AudioSource>();
            Layers.Add(layerName, source);
        }
        public void CreateLayer(AudioClip clip)
        {
            CreateLayer(clip.name, clip);
        }
        public void CreateLayer(string layerName, AudioClip clip)
        {
            if (IsValidLayer(layerName))
            {
                Debug.LogWarning($"Layer '{layerName}' already exists. Use a different name.");
                return;
            }
            GameObject SoundObject = new GameObject("Layer_" + clip.name, new Type[1] { typeof(AudioSource) });
            SoundObject.transform.SetParent(transform);
            AudioSource source = SoundObject.GetComponent<AudioSource>();
            source.clip = clip;
            source.outputAudioMixerGroup = DefaultGroup;
            source.spatialBlend = 0;
            source.playOnAwake = false;
            source.volume = 1f; // Default volume
            source.loop = true;
            Layers.Add(layerName, source);
        }
        public void RemoveLayer(string layerName)
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
        public void FadeInLayer(string layerName, float duration)
        {
            if (Layers.ContainsKey(layerName))
            {
                AudioSource source = Layers[layerName] as AudioSource;
                if (source != null)
                {
                    StartCoroutine(FadeIn(source, duration));
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
        public void FadeOutLayer(string layerName, float duration)
        {
            if (Layers.ContainsKey(layerName))
            {
                AudioSource source = Layers[layerName] as AudioSource;
                if (source != null)
                {
                    StartCoroutine(FadeOut(source, duration));
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
        public void CrossfadeLayer(string layerA, string layerB, float duration)
        {
            if (Layers.ContainsKey(layerA) && Layers.ContainsKey(layerB))
            {
                AudioSource sourceA = Layers[layerA] as AudioSource;
                AudioSource sourceB = Layers[layerB] as AudioSource;
                if (sourceA != null && sourceB != null)
                {
                    StartCoroutine(FadeIn(sourceB, duration));
                    StartCoroutine(FadeOut(sourceA, duration));
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
        public void TransitionToSnapshot(AudioMixerSnapshot snapshot, float transitionTime = 1f)
        {
            snapshot?.TransitionTo(transitionTime);
        }
        public T GetLayerComponent<T>(string layerName) where T : AudioBehaviour
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
        public AudioSource GetLayer(string layerName)
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
        public MusicLerp CreateLerp(string LayerA, string LayerB)
        {
            if (!Layers.ContainsKey(LayerA) || !Layers.ContainsKey(LayerB))
            {
                Debug.LogWarning($"Cannot create lerp: {LayerA} or {LayerB} not found.");
                return default;
            }
            return new MusicLerp((AudioSource)Layers[LayerA], (AudioSource)Layers[LayerB]);
        }
        public AudioBehaviour[] GetLayers()
        {
            return Layers.GetValues().ToArray();
        }

        // Protected Functions
        protected void InitiateLayers()
        {
            foreach (Transform item in transform)
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

        // Enumerator
        private IEnumerator FadeIn(AudioSource source, float duration)
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
        private IEnumerator FadeOut(AudioSource source, float duration)
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


    public abstract class CompositManager<T, J> : CompositManager where T : StateService where J : Enum
    {
        [Serializable]
        protected struct MusicState
        {
            [SerializeField]
            private J State;
            public J state => State;
            [SerializeField]
            private bool CrossWithPreviousState;
            [Space]
            [SerializeField]
            private string[] FadeInLayers;
            [SerializeField]
            private string[] FadeOutLayers;
            [Space]
            [SerializeField]
            public AudioMixerSnapshot shot;
            [SerializeField]
            private float duration;

            private CompositManager manager;

            public void Start(CompositManager owner)
            {
                manager = owner;
            }

            public void Run(MusicState previousState)
            {
                if (manager == null)
                {
                    return;
                }
                if (CrossWithPreviousState)
                {
                    previousState.Reverse();
                }
                if (FadeInLayers != null)
                {
                    foreach (var layer in FadeInLayers)
                    {
                        manager.FadeInLayer(layer, duration);
                    }
                }
                if (FadeOutLayers != null)
                {
                    foreach (var layer in FadeOutLayers)
                    {
                        manager.FadeOutLayer(layer, duration);
                    }
                }
                if (shot != null)
                {
                    manager.TransitionToSnapshot(shot, duration);
                }
            }

            public void Reverse()
            {
                if (manager == null)
                {
                    return;
                }
                if (FadeInLayers != null)
                {
                    foreach (var layer in FadeInLayers)
                    {
                        manager.FadeOutLayer(layer, duration);
                    }
                }
            }
        }


        [Header("State")]
        [SerializeField]
        private MusicState[] StateBehavior;
        [SerializeField, Tooltip("Run the default state on initiate, if true, the 'FirstState' in the Service Class will be run Behavior.")]
        private bool RunFirstState = false;

        protected T stateService;
        public J currentState => stateService.GetCurrentState<J>();

        // IGameManager Interface Implementation
        protected sealed override void InitiateManager(bool AlwaysLoaded)
        {
            InitiateLayers();
            OnInitiate(AlwaysLoaded);
            InitiateMusicStates();
            if (Game.TryFindService(out stateService))
            {
                stateService.OnStateUpdate += OnStateChanged;
                if (RunFirstState)
                {
                    if (IsValidState(currentState))
                    {
                        GetMusicState(currentState).Run(default);
                    }
                    else
                    {
                        Debug.LogWarning($"First state '{currentState}' is not valid in the music state behavior.");
                    }
                }
                MusicStateAssigned();
            }
        }
        protected sealed override void InitiateService(Service service)
        {
            if (stateService == null)
            {
                if (service is T stateserv)
                {
                    stateService = stateserv;
                    stateService.OnStateUpdate += OnStateChanged;
                    if (IsValidState(currentState))
                    {
                        GetMusicState(currentState).Run(default);
                    }
                    else
                    {
                        Debug.LogWarning($"First state '{currentState}' is not valid in the music state behavior.");
                    }
                    MusicStateAssigned();
                }
            }
        }


        // Protected Methods
        protected virtual void OnStateChanged(StateService service)
        {
            J NewState = stateService.GetCurrentState<J>();
            if (IsValidState(NewState))
            {
                MusicState NewMusicState = GetMusicState(NewState);
                J LastState = stateService.GetPreviousState<J>();
                MusicState LastMusicState = GetMusicState(LastState);
                NewMusicState.Run(LastMusicState);
            }
        }

        // Private Methods
        private void InitiateMusicStates()
        {
            List<J> seen = new List<J>();

            for (int i = 0; i < StateBehavior.Length; i++)
            {
                if (seen.Contains(StateBehavior[i].state))
                {
                    Debug.LogWarning($"Duplicate layer '{StateBehavior[i].state}' found at index {i} (already added at index {seen.IndexOf(StateBehavior[i].state)})");
                }
                else
                {
                    StateBehavior[i].Start(this);
                    seen.Add(StateBehavior[i].state);
                }
            }
        }
        private bool IsValidState(J state)
        {
            return StateBehavior.Any(s => s.state.Equals(state));
        }
        private MusicState GetMusicState(J state)
        {
            return StateBehavior.FirstOrDefault(s => s.state.Equals(state));
        }


        //Abstract Method
        protected abstract void OnInitiate(bool AlwaysLoaded);
        protected abstract void MusicStateAssigned();

    }


}