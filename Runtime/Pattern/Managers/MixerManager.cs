using UnityEngine;
using UnityEngine.Audio;


namespace RealMethod
{
    public abstract class MixerManager : MonoBehaviour, IGameManager
    {
        [Header("Setting")]
        [SerializeField] private AudioMixer mixer;
        public AudioMixer Mixer => mixer;
        [SerializeField]
        private StringFloatDictionary Parameter;


        // Operators
        public float this[string name]
        {
            get => Parameter[name];
            set => Parameter[name] = value;
        }

        // IGameManager Interface Implementation
        public MonoBehaviour GetManagerClass()
        {
            return this;
        }
        // Basic Methods
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
        // Public Methods
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
        public abstract void InitiateManager(bool AlwaysLoaded);
        public abstract void InitiateService(Service service);


        // protected void TransitionToSnapshot(AudioMixerSnapshot snapshot, float transitionTime = 1f)
        // {
        //     snapshot?.TransitionTo(transitionTime);
        // }
    }



}
