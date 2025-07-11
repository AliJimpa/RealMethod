using System;
using UnityEditor.Build;

namespace RealMethod
{
    public abstract class StateService : Service
    {
        public Action<StateService> OnStateChanged;

        public abstract int GetStateIndex();
    }

    public abstract class StateService<T> : StateService where T : Enum
    {
        public T currentState { get; private set; }
        public T firstState { get; private set; }
        public Action<T> OnNewState;

        public StateService(T Default)
        {
            firstState = Default;
            currentState = firstState;
        }

        // StateService Methods
        protected sealed override void OnStart(object Author)
        {
            currentState = DefaultState();
            OnStart(Author, currentState);
        }
        protected sealed override void OnNewWorld()
        {
            if (CanResetforNewWorld(Game.World))
            {
                ResetToDefault();
            }
        }
        public sealed override int GetStateIndex()
        {
            return Convert.ToInt32(currentState);
        }


        // Public Functions
        public bool SetState(T Target)
        {
            if (CanSwitch(currentState, Target))
            {
                currentState = Target;
                OnStateChanged?.Invoke(this);
                OnNewState?.Invoke(currentState);
                return true;
            }
            else
            {
                return false;
            }

        }
        public void ResetToDefault()
        {
            SetState(DefaultState());
        }

        // Protected Functions
        protected virtual T DefaultState()
        {
            return firstState;
        }

        // Abstract Methods
        protected abstract void OnStart(object Author, T State);
        public abstract bool CanSwitch(T A, T B);
        protected abstract bool CanResetforNewWorld(World NewWorld);


    }
}