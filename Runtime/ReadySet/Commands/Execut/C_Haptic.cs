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
        protected override bool OnInitiate(Object author, Object owner)
        {
            manager = Game.FindManager<HapticManager>();
            return manager != null;
        }
        protected override bool CanExecute(object Owner)
        {
            return enabled && manager != null;
        }
        protected override void Execute(object Owner)
        {
            foreach (var conf in configs)
            {
                manager.Play(conf);
            }
        }


    }
}