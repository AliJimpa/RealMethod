using System;

namespace RealMethod
{
    public abstract class StateService : Service
    {
        public Action<StateService> OnStateChanged;

        public abstract int GetIndex();
    }

    public abstract class StateService<T> : StateService where T : Enum
    {
        public T State { get; private set; }
        public bool ResetStateInNewWorld = false;
        public T DefaultState { get; private set; }
        public Action<T> OnNewState;


        public StateService(T Default)
        {
            DefaultState = Default;
            State = Default;
        }

        public override void WorldUpdated()
        {
            if (ResetOnNewWorld())
            {
                State = DefaultState;
                OnStateChanged?.Invoke(this);
                OnNewState?.Invoke(State);
            }
        }
        public override int GetIndex()
        {
            return Convert.ToInt32(State);
        }

        public bool SwitchState(T Target)
        {
            if (CanSwitch(State, Target))
            {
                State = Target;
                OnStateChanged?.Invoke(this);
                OnNewState?.Invoke(State);
                return true;
            }
            else
            {
                return false;
            }

        }
        public void SetDefaultState(T state)
        {
            DefaultState = state;
            OnStateChanged?.Invoke(this);
            OnNewState?.Invoke(State);
        }

        // Can Switch State
        public abstract bool CanSwitch(T A, T B);
        // The state Set Back to Defaul State when Changing Your Scene
        public abstract bool ResetOnNewWorld();

    }
}