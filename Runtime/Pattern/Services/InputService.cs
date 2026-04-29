using UnityEngine;
using UnityEngine.InputSystem;

namespace RealMethod
{
    public abstract class InputService : Service
    {
        private InputActionAsset CurrentInputAsset;

        public sealed override void OnRegister(object Author)
        {
            if (TryFindInputAsset(out InputActionAsset newAsset))
            {
                ReplaceInputAsset(newAsset);
            }
        }
        public sealed override void OnWorldChanging(World Previous, World New)
        {
            if (TryFindInputAsset(out InputActionAsset newAsset))
            {
                ReplaceInputAsset(newAsset);
            }
        }
        public sealed override void OnUnregister(object Author)
        {
            if (CurrentInputAsset != null)
            {
                OnDisposeInputAsset(CurrentInputAsset);
            }
        }


        public void ReplaceInputAsset(InputActionAsset asset)
        {
            if (asset != null)
            {
                if (CurrentInputAsset != null)
                {
                    OnDisposeInputAsset(CurrentInputAsset);
                }
                CurrentInputAsset = asset;
                OnAcquireInputAsset(CurrentInputAsset);
            }
            else
            {
                Debug.LogError("Your asset is not valid!");
            }
        }
        protected virtual bool TryFindInputAsset(out InputActionAsset asset)
        {
            PlayerInput playerinput = Game.World.GetPlayerObject().GetComponent<PlayerInput>();
            if (playerinput != null)
            {
                if (playerinput.actions != null)
                {
                    asset = playerinput.actions;
                    return true;
                }
            }
            asset = null;
            return false;
        }


        protected abstract void OnAcquireInputAsset(InputActionAsset asset);
        protected abstract void OnDisposeInputAsset(InputActionAsset asset);
    }






}