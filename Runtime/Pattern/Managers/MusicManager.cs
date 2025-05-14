using System.Collections.Generic;
using UnityEngine;


namespace RealMethod
{
    public abstract class MusicManager : MonoBehaviour, IGameManager
    {
        public List<AudioClip> MusicList;

        public MonoBehaviour GetManagerClass()
        {
            throw new System.NotImplementedException();
        }
        public void InitiateManager(bool AlwaysLoaded)
        {
            if (AlwaysLoaded)
                DontDestroyOnLoad(this);

        }
        public void InitiateService(Service service)
        {
        }

    }
}
