using UnityEngine;
using UnityEngine.Audio;


namespace RealMethod
{
    public abstract class MixerManager : MonoBehaviour, IGameManager
    {
        [Header("Mixer")]
        [SerializeField]
        private AudioMixer Mixer;
        public AudioMixer mixer => Mixer;
        [SerializeField]
        private StringFloatDictionary Parameter;

        // Operators
        public float this[string name]
        {
            get => Parameter[name];
            set => Parameter[name] = value;
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
        void IGameManager.InitiateService(Service service)
        {
            InitiateService(service);
        }

        // Unity Methods
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Mixer != null)
            {
                Parameter.Clear();
                System.Array parameters = (System.Array)Mixer.GetType().GetProperty("exposedParameters").GetValue(Mixer, null);
                for (int i = 0; i < parameters.Length; i++)
                {
                    var o = parameters.GetValue(i);
                    string PrametrName = (string)o.GetType().GetField("name").GetValue(o);
                    Mixer.GetFloat(PrametrName, out float result);
                    Parameter.Add(PrametrName, result);
                }
            }
        }
#endif

        // Public Functions
        public void Sync()
        {
            if (Mixer != null)
            {
                foreach (var param in Parameter)
                {
                    Mixer.SetFloat(param.Key, param.Value);
                }
            }
            else
            {
                Debug.LogError("AudioMixers Not Valid!");
            }

        }

        // Abstract Method
        protected abstract void InitiateManager(bool AlwaysLoaded);
        protected abstract void InitiateService(Service service);
    }



}
