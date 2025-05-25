using System;

namespace RealMethod
{
    public abstract class StateService<T> : Service where T : Enum
    {
        public T State { get; private set; }
        public bool ResetStateInNewWorld = false;
        public T DefaultState { get; private set; }
        public Action<T> OnChangeState;


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
            }
        }

        public bool SwitchState(T Target)
        {
            if (CanSwitch(State, Target))
            {
                State = Target;
                OnChangeState?.Invoke(State);
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
        }


        // Can Switch State
        public abstract bool CanSwitch(T A, T B);
        // The state Set Back to Defaul State when Changing Your Scene
        public abstract bool ResetOnNewWorld();

    }
}