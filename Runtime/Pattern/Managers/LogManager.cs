using UnityEngine;

namespace RealMethod
{
    public class LogManager : MonoBehaviour, IGameManager
    {
        void IGameManager.InitiateManager(bool AlwaysLoaded)
        {
            if (!AlwaysLoaded)
            {
                Debug.LogError("LogManager should be Initiate in GameScope.");
                Destroy(this);
            }
        }







    }
}