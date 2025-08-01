using System;
using UnityEngine;

namespace RealMethod
{

    public abstract class StateService : Service
    {
        protected byte currentState { get; private set; }
        protected byte previousState { get; private set; }
        public Action<StateService> OnStateUpdate;

        public StateService(byte firstState)
        {
            previousState = firstState;
            currentState = firstState;
        }

        // StateService Methods
        protected sealed override void OnNewWorld()
        {
            if (CanResetforNewWorld(Game.World))
            {
                ResetToDefault();
            }
        }

        // Public Functions
        public bool SetState(byte target)
        {
            if (CanSwitch(currentState, target))
            {
                previousState = currentState;
                currentState = target;
                OnStateUpdate?.Invoke(this);
                OnStateChanged();
                return true;
            }
            else
            {
                return false;
            }
        }
        public void ResetToDefault()
        {
            SetState(GetDefaultState());
        }

        // Abstract Methods
        public abstract bool CanSwitch(byte A, byte B);
        public abstract Tenum GetCurrentState<Tenum>() where Tenum : Enum;
        public abstract Tenum GetPreviousState<Tenum>() where Tenum : Enum;
        protected abstract byte GetDefaultState();
        protected abstract void OnStateChanged();
        protected abstract bool CanResetforNewWorld(World NewWorld);
    }
    public abstract class StateService<T> : StateService where T : Enum
    {
        public T CurrentState => (T)Enum.ToObject(typeof(T), currentState);
        public T PreviousState => (T)Enum.ToObject(typeof(T), previousState);
        public Action<T, T> OnStateChange;

        public StateService(T firstState) : base(Convert.ToByte(firstState))
        {

        }

        // Public Functions
        public bool SetState(T target)
        {
            return SetState(Convert.ToByte(target));
        }

        // StateService Meethods
        public sealed override Tenum GetCurrentState<Tenum>()
        {
            if (typeof(Tenum) != typeof(T))
                throw new InvalidOperationException($"State type mismatch. Expected {typeof(T)}, but got {typeof(Tenum)}");

            return (Tenum)Enum.ToObject(typeof(Tenum), currentState);
        }
        public sealed override Tenum GetPreviousState<Tenum>()
        {
            if (typeof(Tenum) != typeof(T))
                throw new InvalidOperationException($"State type mismatch. Expected {typeof(T)}, but got {typeof(Tenum)}");

            return (Tenum)Enum.ToObject(typeof(Tenum), previousState);
        }
        protected sealed override void OnStateChanged()
        {
            OnStateChange?.Invoke(PreviousState, CurrentState);
        }
        public sealed override bool CanSwitch(byte A, byte B)
        {
            return CanSwitch((T)Enum.ToObject(typeof(T), A), (T)Enum.ToObject(typeof(T), B));
        }
        protected sealed override byte GetDefaultState()
        {
            return Convert.ToByte(DefaultState());
        }

        // Abstract Methods
        public abstract bool CanSwitch(T A, T B);
        protected abstract T DefaultState();
    }

}