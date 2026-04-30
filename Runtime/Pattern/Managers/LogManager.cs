using UnityEngine;

namespace RealMethod
{
    public class LogManager : MonoBehaviour, IGameManager
    {
        void IGameManager.InitiateManager(Scope owner)
        {
            if (owner.IsWorldScope())
            {
                Debug.LogError("LogManager should be Initiate in GameScope.");
                Destroy(this);
            }
        }







    }
}