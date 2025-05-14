using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


namespace RealMethod
{
    [Serializable]
    public class AudioParameters
    {
        [SerializeField]
        private string name;
        [SerializeField, ReadOnly]
        private float value;

        public AudioParameters(string Name, float Value)
        {
            name = Name;
            value = Value;
        }


        public string GetName()
        {
            return name;
        }
        public float GetValue()
        {
            return value;
        }

    }

    public abstract class AudioManager : MonoBehaviour, IGameManager
    {
        [Header("Mixer")]
    [SerializeField]
    private AudioMixer audioMixer;
    public List<AudioParameters> param;


    // IGameManager Interface Implementation
    public MonoBehaviour GetManagerClass()
    {
        return this;
    }
    public void InitiateManager(bool AlwaysLoaded)
    {
    }
    public void InitiateService(Service service)
    {

    }


    // Basic Methods
    public void SetParameter(int index, float value)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat(param[index].GetName(), value);
        }
        else
        {
            //Game.LogError("AudioMixers Not Valid!");
        }
    }
    public float GetParameter(int index)
    {
        float value;
        if (audioMixer != null)
        {
            audioMixer.GetFloat(param[index].GetName(), out value);
            return value;
        }
        else
        {
            //Game.LogError("AudioMixers Not Valid!");
            return -1;
        }
    }


    protected AudioMixer GetMixer()
    {
        return audioMixer;
    }
    protected void SetParameter(string name, float value)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat(name, value);
        }
        else
        {
            //Game.LogError("AudioMixers Not Valid!");
        }
    }
    protected float GetParameter(string name)
    {
        float value;
        if (audioMixer != null)
        {
            audioMixer.GetFloat(name, out value);
            return value;
        }
        else
        {
            //Game.LogError("AudioMixers Not Valid!");
            return -1;
        }
    }


#if UNITY_EDITOR
    [ContextMenu("UpdatePropertyMixer")]
    private void ListPropertyMixer()
    {
        param.Clear();
        System.Array parameters = (System.Array)audioMixer.GetType().GetProperty("exposedParameters").GetValue(audioMixer, null);
        for (int i = 0; i < parameters.Length; i++)
        {
            var o = parameters.GetValue(i);
            string PrametrName = (string)o.GetType().GetField("name").GetValue(o);
            AudioParameters newap = new AudioParameters(PrametrName, GetParameter(PrametrName));
            param.Add(newap);
        }
    }
#endif



}

}
