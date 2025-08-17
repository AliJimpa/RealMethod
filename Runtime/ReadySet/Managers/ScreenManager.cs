using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    public interface IInformer
    {
        void Popup(string text);
        void Popup(string text , float duration);
    }

    [AddComponentMenu("RealMethod/Manager/ScreenManager")]
    public sealed class ScreenManager : UIManager
    {
        [Header("Events")]
        [SerializeField]
        private UnityEvent<bool> OnLoadScne;
        [SerializeField]
        private UnityEvent<float> OnLoading;

        public IInformer Informer { get; private set; }

        // UIManager Methods
        protected override void InitiateManager(bool alwaysLoaded)
        {
            Game.Service.OnSceneLoading += (value) => OnLoadScne?.Invoke(value);
            Game.Service.OnSceneLoadingProcess += (value) => OnLoading?.Invoke(value);
            if (Game.TryFindService(out Spawn SpawnServ))
            {
                SpawnServ.BringManager(this);
            }
        }
        protected override void InitiateService(Service newService)
        {
            if (newService is Spawn spawnservice)
            {
                spawnservice.BringManager(this);
            }
        }

        public void SetInformer(IInformer messanger)
        {
            Informer = messanger;
        }
    }
}