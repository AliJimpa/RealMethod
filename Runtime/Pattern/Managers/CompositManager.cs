using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace RealMethod
{
    public abstract class CompositManager<T, J> : MixerManager where T : StateService where J : Enum
    {
        [Serializable]
        public class LayerAudioDictionary : SerializableDictionary<J, AudioSource> { }

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
        private LayerAudioDictionary Layers;
        [SerializeField]
        private AudioMixerSnapshot[] Snapshots;
        [SerializeField]
        private float SnapshotTime = 2;



        protected T service;

        // Operators
        public AudioSource this[J index]
        {
            get => Layers[index];
            private set => Layers[index] = value;
        }

        // IGameManager Interface Implementation
        protected override void InitiateManager(bool AlwaysLoaded)
        {
            if (!AlwaysLoaded)
            {
                Debug.LogError("You can't use MusicManager in [World] Scope");
                Destroy(this);
            }

            if (Game.TryFindService(out service))
            {
                service.OnStateChanged += OnStateChanged;
                ServiceAssigned();
            }
        }
        protected override void InitiateService(Service service)
        {
            if (service is T stateserv)
            {
                this.service = stateserv;
                this.service.OnStateChanged += OnStateChanged;
                ServiceAssigned();
            }
        }

        // Public Methods
        public void CrossfadeLayer(J LayerA, J LayerB, float Duration)
        {
            StartCoroutine(CrossfadeTracks(Layers[LayerA], Layers[LayerB], Duration));
        }
        public void FadeInLayer(J Layer, float Duration)
        {
            StartCoroutine(fadeTrack(Layers[Layer], true, Duration));
        }
        public void FadeOutLayer(J Layer, float Duration)
        {
            StartCoroutine(fadeTrack(Layers[Layer], false, Duration));
        }
        public MusicLerp Interp(J LayerA, J LayerB)
        {
            return new MusicLerp(Layers[LayerA], Layers[LayerB]);
        }
        private void TransitionToSnapshot(AudioMixerSnapshot snapshot, float transitionTime = 1f)
        {
            snapshot?.TransitionTo(transitionTime);
        }

        // Private Methods
        private void OnStateChanged(StateService service)
        {
            if (Snapshots[service.GetStateIndex()] != null)
            {
                TransitionToSnapshot(Snapshots[service.GetStateIndex()], SnapshotTime);
            }
        }

        //IEnumerator Corotine
        private IEnumerator CrossfadeTracks(AudioSource sourceA, AudioSource sourceB, float duration)
        {
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
            source.Stop();
        }

        //Abstract Method
        public abstract void ServiceAssigned();


#if UNITY_EDITOR
        private void OnValidate()
        {

            foreach (Transform item in transform)
            {
                if (item.gameObject.name == "MusicLayers")
                {
                    return;
                }
            }

            GameObject targetlayer = new GameObject("MusicLayers");
            targetlayer.transform.SetParent(transform);

            if (Layers != null && Layers.Count > 0)
            {
                foreach (var layer in Layers)
                {
                    if (layer.Value == null)
                    {
                        GameObject layerobject = new GameObject($"Layer {layer.Key}");
                        Layers[layer.Key] = layerobject.AddComponent<AudioSource>();
                        Layers[layer.Key].loop = true;
                        Layers[layer.Key].playOnAwake = false;
                        layerobject.transform.SetParent(transform);
                    }
                }
            }
        }
#endif

    }


}