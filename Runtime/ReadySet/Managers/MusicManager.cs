using System.Collections;
using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Manager/MusicManager")]
    public sealed class MusicManager : MixerManager
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

            public void Lerp(float alpha)
            {
                LayerA.volume = Mathf.Lerp(1f, 0f, alpha);
                LayerB.volume = Mathf.Lerp(0f, 1f, alpha);
            }
        }

        public AudioSource[] Layers;

        public AudioSource this[int index]
        {
            get => Layers[index];
            private set => Layers[index] = value;
        }

        public override void InitiateManager(bool AlwaysLoaded)
        {
            if (!AlwaysLoaded)
            {
                Debug.LogError("You can't use MusicManager in [World] Scope");
                Destroy(this);
            }
        }
        public override void InitiateService(Service service)
        {
        }

        public void CrossfadeLayer(int LayerA, int LayerB, float Duration)
        {
            StartCoroutine(CrossfadeTracks(Layers[LayerA], Layers[LayerB], Duration));
        }
        public void FadeInLayer(int Layer, float Duration)
        {
            StartCoroutine(fadeTrack(Layers[Layer], true, Duration));
        }
        public void FadeOutLayer(int Layer, float Duration)
        {
            StartCoroutine(fadeTrack(Layers[Layer], false, Duration));
        }
        public MusicLerp Interp(int LayerA, int LayerB)
        {
            return new MusicLerp(Layers[LayerA], Layers[LayerB]);
        }

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


#if UNITY_EDITOR
        [ContextMenu("CreateLayer")]
        private void CreateLayer()
        {
            if (Layers != null && Layers.Length > 0)
            {
                for (int i = 0; i < Layers.Length; i++)
                {
                    if (Layers[i] == null)
                    {
                        GameObject layerobject = new GameObject($"Layer {i}");
                        Layers[i] = layerobject.AddComponent<AudioSource>();
                        Layers[i].loop = true;
                        Layers[i].playOnAwake = false;
                        layerobject.transform.SetParent(transform);
                    }
                }
            }
        }
#endif

    }


}