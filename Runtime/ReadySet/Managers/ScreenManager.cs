using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Manager/ScreenManager")]
    public sealed class ScreenManager : UIManager
    {
        [Header("Screen")]
        [SerializeField]
        private float FadeDuraction = 1;
        [SerializeField]
        private UnityEvent<bool> OnLoadScne;
        [SerializeField]
        private UnityEvent<float> OnLoading;


        private void Awake()
        {
            GameService GS = Game.Service;
            if (GS != null)
            {
                GS.FadeTime = FadeDuraction;
                GS.OnSceneLoading += (value) => OnLoadScne?.Invoke(value);
                GS.OnSceneLoadingProcess += (value) => OnLoading?.Invoke(value);
            }
            else
            {
                Debug.LogError("The Game Doesn't have Service");
            }
        }

        public override void InitiateService(Service newService)
        {

        }
        public override bool IsMaster()
        {
            return false;
        }
    }
}