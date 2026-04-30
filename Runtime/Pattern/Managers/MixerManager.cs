using UnityEngine;
using UnityEngine.Audio;


namespace RealMethod
{
    public abstract class MixerManager : MonoBehaviour, IGameManager
    {
        [Header("Mixer")]
        [SerializeField]
        private AudioMixer mixer;
        public AudioMixer Mixer => mixer;
#if UNITY_EDITOR
        [SerializeField]
        private Map<string, float> Parameter;
#endif


        // IGameManager Interface Implementation
        public virtual void InitiateManager(Scope owner)
        {

        }

        // Unity Methods
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (mixer != null)
            {
                Parameter.Clear();
                System.Array parameters = (System.Array)mixer.GetType().GetProperty("exposedParameters").GetValue(mixer, null);
                for (int i = 0; i < parameters.Length; i++)
                {
                    var o = parameters.GetValue(i);
                    string PrametrName = (string)o.GetType().GetField("name").GetValue(o);
                    mixer.GetFloat(PrametrName, out float result);
                    Parameter.Add(PrametrName, result);
                }
            }
        }
#endif

        // Public Functions
        public void SetParam(string param, float value)
        {
            mixer.SetFloat(param, value);
        }
        public float GetParam(string param)
        {
            if (mixer.GetFloat(param, out float result))
            {
                return result;
            }
            else
            {
                Debug.LogError($"Can't find any value with param:({param})");
                return 0;
            }
        }
        public bool TryGetParam(string param, out float result)
        {
            return mixer.GetFloat(param, out result);
        }
        public void TransitionToSnapshot(AudioMixerSnapshot snapshot, float transitionTime = 1f)
        {
            snapshot?.TransitionTo(transitionTime);
        }
    }



}
