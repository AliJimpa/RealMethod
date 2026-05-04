using UnityEngine;
using UnityEngine.InputSystem;

namespace RealMethod
{
    public abstract class InputService : GameService
    {
        private InputActionAsset CurrentInputAsset;

        protected override void OnBegin()
        {
            if (TryFindInputAsset(out InputActionAsset newAsset))
            {
                ReplaceInputAsset(newAsset);
            }
        }
        protected override void OnWorldChanged()
        {
            if (TryFindInputAsset(out InputActionAsset newAsset))
            {
                ReplaceInputAsset(newAsset);
            }
        }
        protected override void OnEnd()
        {
            if (CurrentInputAsset != null)
            {
                OnDisposeInputAsset(CurrentInputAsset);
            }
        }
#if UNITY_EDITOR
        protected override string GetInspectorInfo()
        {
            return CurrentInputAsset.name;
        }
#endif


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