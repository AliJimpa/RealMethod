using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Manager/ScreenManager")]
    public sealed class ScreenManager : UIManager
    {
        [Header("Events")]
        [SerializeField]
        private UnityEvent<bool> OnLoadScne;
        [SerializeField]
        private UnityEvent<float> OnLoading;

        // UIManager Methods
        protected override void InitiateManager(bool alwaysLoaded)
        {
            Game.Service.OnSceneLoading += (value) => OnLoadScne?.Invoke(value);
            Game.Service.OnSceneLoadingProcess += (value) => OnLoading?.Invoke(value);
        }
        protected override void InitiateService(Service newService)
        {

        }

    }
}