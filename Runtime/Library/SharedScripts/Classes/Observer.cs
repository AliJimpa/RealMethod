using System;
using System.Collections.Generic;

namespace RealMethod
{
    public abstract class Observer
    {
        protected Action<Observer> OnObserv;
        public abstract void Check();
        public void Bind(Action<Observer> callback)
        {
            OnObserv = callback;
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
            OnObserv = onObserv;
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
                OnObserv?.Invoke(this);
                lastValue = current;
            }
        }
    }
}