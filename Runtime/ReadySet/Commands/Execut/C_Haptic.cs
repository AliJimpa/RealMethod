using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Command/Haptic")]
    public sealed class C_Haptic : Command
    {
        [SerializeField]
        private HapticConfig[] configs;
        private HapticManager manager;

        // Command Methods
        protected override bool OnInitiate(Object owner)
        {
            manager = Game.FindManager<HapticManager>();
            return manager != null;
        }
        protected override bool CanExecute(object Executer)
        {
            return enabled && manager != null;
        }
        protected override void Execute(object Executer)
        {
            foreach (var conf in configs)
            {
                manager.Play(conf);
            }
        }


    }
}