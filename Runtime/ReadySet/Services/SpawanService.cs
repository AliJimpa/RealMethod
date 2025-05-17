using UnityEngine;

namespace RealMethod
{
    public class Spawn : Service
    {
        private static Spawn instance;
        private AudioManager audioBox;

        public override void Start(object Author)
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogError($"Spawn Service Is Created befor You Can't Create Twice! [Author:{Author}]");
            }
        }
        public override void WorldUpdated()
        {
        }
        public override void End(object Author)
        {
            instance = null;
        }


        public void BringManager(IGameManager manager)
        {
            Debug.Log(manager);

            if (manager.GetManagerClass() is AudioManager audiomanager)
            {
                if (audioBox == null)
                {
                    audioBox = audiomanager;
                }
                else
                {
                    Debug.LogError($"Spawn Service already have AudioManager Cant Enter this {audiomanager}");
                }
            }
        }
    }
}