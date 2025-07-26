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

        // UIManager Methods
        protected override void InitiateManager(bool alwaysLoaded)
        {
            Game.Service.SetFadeTime(FadeDuraction);
            Game.Service.OnSceneLoading += (value) => OnLoadScne?.Invoke(value);
            Game.Service.OnSceneLoadingProcess += (value) => OnLoading?.Invoke(value);
        }
        protected override void InitiateService(Service newService)
        {

        }

    }
}