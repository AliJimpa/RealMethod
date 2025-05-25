using System;
using System.Collections.Generic;

namespace RealMethod
{
    public abstract class Observer
    {
        protected Action<Observer> OnObserve;
        public abstract void Check();
        public void Bind(Action<Observer> callback)
        {
            OnObserve = callback;
        }
        public void Unbind(Action<Observer> callback)
        {
            if (OnObserve == callback)
            {
            OnObserve = null;
            }
        }
    }

    public class Observer<T> : Observer
    {
        public Func<T> Variable { get; private set; }
        public T Value => Variable();
        private T lastValue;
        private Action<T> OnChanged;


        public Observer(Func<T> getter, Action<Observer> onObserv)
        {
            Variable = getter;
            OnObserve = onObserv;
            lastValue = getter(); // store initial value
        }
        public Observer(Func<T> getter, Action<T> onChanged)
        {
            Variable = getter;
            OnChanged = onChanged;
            lastValue = getter(); // store initial value
        }

        public override void Check()
        {
            T current = Variable();
            if (!EqualityComparer<T>.Default.Equals(current, lastValue))
            {
                OnChanged?.Invoke(current); // Trigger the action
                OnObserve?.Invoke(this);
                lastValue = current;
            }
        }
    }
}