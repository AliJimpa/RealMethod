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

        // Operators
        public float this[string param]
        {
            get
            {
                mixer.GetFloat(param, out float result);
                return result;
            }
            set => mixer.SetFloat(param, value);
        }

        // IGameManager Interface Implementation
        MonoBehaviour IGameManager.GetManagerClass()
        {
            return this;
        }
        void IGameManager.InitiateManager(bool AlwaysLoaded)
        {
            InitiateManager(AlwaysLoaded);
        }
        void IGameManager.ResolveService(Service service, bool active)
        {
            ResolveService(service, active);
        }

        // Unity Methods
#if UNITY_EDITOR
        private void OnValidate()
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
        public void TransitionToSnapshot(AudioMixerSnapshot snapshot, float transitionTime = 1f)
        {
            snapshot?.TransitionTo(transitionTime);
        }

        // Protected Method
        protected virtual void InitiateManager(bool AlwaysLoaded)
        {

        }
        protected virtual void ResolveService(Service service, bool active)
        {

        }
    }



}
