using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace RealMethod
{
    public abstract class CompositManager<T, J> : MixerManager where T : StateService where J : Enum
    {
        [Serializable]
        protected struct MusicLayer
        {
            [SerializeField]
            private J Layer;
            public J layer => Layer;
            [SerializeField]
            private AudioSource Source;
            public AudioSource source => Source;
            [SerializeField]
            private AudioMixerSnapshot SnapShot;
            public AudioMixerSnapshot snapShot => SnapShot;
            [SerializeField]
            private float LerpDuration;
            public float lerpDuration => LerpDuration;

            public bool Compair(J targetLayer)
            {
                return EqualityComparer<J>.Default.Equals(targetLayer, Layer);
            }
            public void SetSource(AudioSource newSource)
            {
                Source = newSource;
                Source.loop = true;
                Source.spatialBlend = 0;
                Source.playOnAwake = false;
            }
            public void SetSnapShot(AudioMixerSnapshot newSnapshot)
            {
                SnapShot = newSnapshot;
            }
        }
        public struct MusicLerp
        {
            private AudioSource LayerA;
            private AudioSource LayerB;

            public MusicLerp(AudioSource A, AudioSource B)
            {
                LayerA = A;
                LayerB = B;
            }

            public void Lerp(float alpha)
            {
                LayerA.volume = Mathf.Lerp(1f, 0f, alpha);
                LayerB.volume = Mathf.Lerp(0f, 1f, alpha);
            }
        }


        [Header("Composit")]
        [SerializeField]
        protected MusicLayer[] Layers;


        protected T stateService;
        public J currentState => stateService.GetCurrentState<J>();

        // Operators
        public AudioSource this[int index]
        {
            get => Layers[index].source;
        }
        public AudioSource this[J layer]
        {
            get
            {
                foreach (var lay in Layers)
                {
                    if (lay.Compair(layer))
                    {
                        return lay.source;
                    }
                }
                return null;
            }
        }


        // IGameManager Interface Implementation
        protected override void InitiateManager(bool AlwaysLoaded)
        {
            if (!AlwaysLoaded)
            {
                Debug.LogError("You can't use MusicManager in [World] Scope");
                Destroy(this);
            }

            if (Game.TryFindService(out stateService))
            {
                stateService.OnStateUpdate += OnStateChanged;
                ServiceAssigned();
            }

            CheckDuplicateLayers();
        }
        protected override void InitiateService(Service service)
        {
            if (service is T stateserv)
            {
                stateService = stateserv;
                stateService.OnStateUpdate += OnStateChanged;
                ServiceAssigned();
            }
        }

        // Public Methods
        public void CrossfadeLayer(J LayerA, J LayerB, float Duration)
        {
            StartCoroutine(CrossfadeTracks(this[LayerA], this[LayerB], Duration));
        }
        public void FadeInLayer(J Layer, float Duration)
        {
            StartCoroutine(fadeTrack(this[Layer], true, Duration));
        }
        public void FadeOutLayer(J Layer, float Duration)
        {
            StartCoroutine(fadeTrack(this[Layer], false, Duration));
        }
        public MusicLerp CreateInterp(J LayerA, J LayerB)
        {
            return new MusicLerp(this[LayerA], this[LayerB]);
        }
        private void TransitionToSnapshot(AudioMixerSnapshot snapshot, float transitionTime = 1f)
        {
            snapshot?.TransitionTo(transitionTime);
        }

        // Protected Methods
        protected MusicLayer GetLayer(J targetlayer)
        {
            foreach (var layer in Layers)
            {
                if (layer.Compair(targetlayer))
                {
                    return layer;
                }
            }
            Debug.LogWarning("Can't Find Layer!");
            return new MusicLayer();
        }
        protected virtual void OnStateChanged(StateService service)
        {
            J OldState = stateService.GetPreviousState<J>();
            J NewState = stateService.GetCurrentState<J>();
            MusicLayer TargetLayer = GetLayer(NewState);
            CrossfadeLayer(OldState, NewState, TargetLayer.lerpDuration);
            if (TargetLayer.snapShot != null)
                TransitionToSnapshot(TargetLayer.snapShot, TargetLayer.lerpDuration);
        }

        // Private Methods
        private void CheckDuplicateLayers()
        {
            List<J> seen = new List<J>();

            for (int i = 0; i < Layers.Length; i++)
            {
                if (seen.Contains(Layers[i].layer))
                {
                    Debug.LogWarning($"Duplicate layer '{Layers[i].layer}' found at index {i} (already added at index {seen.IndexOf(Layers[i].layer)})");
                }
                else
                {
                    seen.Add(Layers[i].layer);
                }
            }
        }

        //IEnumerator Corotine
        private IEnumerator CrossfadeTracks(AudioSource sourceA, AudioSource sourceB, float duration)
        {
            sourceA.Play();
            sourceB.Play();

            float timer = 0f;
            while (timer < duration)
            {
                float t = timer / duration;
                sourceA.volume = Mathf.Lerp(1f, 0f, t);
                sourceB.volume = Mathf.Lerp(0f, 1f, t);
                timer += Time.deltaTime;
                yield return null;
            }

            sourceA.Stop();
        }
        private IEnumerator fadeTrack(AudioSource source, bool fadeIn, float duration)
        {
            if (!fadeIn)
                source.Play();

            float timer = 0f;

            while (timer < duration)
            {
                float t = timer / duration;
                if (fadeIn)
                {
                    source.volume = Mathf.Lerp(1f, 0f, t);
                }
                else
                {
                    source.volume = Mathf.Lerp(0f, 1f, t);
                }
                timer += Time.deltaTime;
                yield return null;
            }

            if (fadeIn)
                source.Stop();
        }

        //Abstract Method
        public abstract void ServiceAssigned();


    }


}