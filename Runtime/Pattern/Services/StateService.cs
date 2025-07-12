using System;

namespace RealMethod
{
    public abstract class StateService : Service
    {
        public Action<StateService> OnStateUpdate;


        public abstract Tenum GetCurrentState<Tenum>() where Tenum : Enum;
        public abstract Tenum GetPreviousState<Tenum>() where Tenum : Enum;

    }

    public abstract class StateService<T> : StateService where T : Enum
    {
        public T currentState { get; private set; }
        public T previousState { get; private set; }
        public Action<T, T> OnStateChanged;

        public StateService(T firstState)
        {
            previousState = firstState;
            currentState = firstState;
        }

        // StateService Methods
        protected sealed override void OnStart(object Author)
        {
            OnStart(Author, currentState);
        }
        protected sealed override void OnNewWorld()
        {
            if (CanResetforNewWorld(Game.World))
            {
                ResetToDefault();
            }
        }
        public sealed override Tenum GetCurrentState<Tenum>()
        {
            if (typeof(Tenum) != typeof(T))
                throw new InvalidOperationException($"State type mismatch. Expected {typeof(T)}, but got {typeof(Tenum)}");

            return (Tenum)(object)currentState;
        }
        public sealed override Tenum GetPreviousState<Tenum>()
        {
            if (typeof(Tenum) != typeof(T))
                throw new InvalidOperationException($"State type mismatch. Expected {typeof(T)}, but got {typeof(Tenum)}");

            return (Tenum)(object)previousState;
        }

        // Public Functions
        public bool SetState(T Target)
        {
            if (CanSwitch(currentState, Target))
            {
                previousState = currentState;
                currentState = Target;
                OnStateUpdate?.Invoke(this);
                OnStateChanged?.Invoke(previousState, currentState);
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

        // Abstract Methods
        protected abstract T DefaultState();
        protected abstract void OnStart(object Author, T State);
        public abstract bool CanSwitch(T A, T B);
        protected abstract bool CanResetforNewWorld(World NewWorld);
    }

}