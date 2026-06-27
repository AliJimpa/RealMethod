using System;
using System.Collections.Generic;

namespace RealMethod
{
    public class EnumStorage : IDisposable
    {
        private readonly Dictionary<Type, byte> _values = new();

        // Implement IDisposable Interface
        void IDisposable.Dispose()
        {
            _values.Clear();
        }

        public void Set<T>(T enumValue) where T : Enum
        {
            _values[typeof(T)] = Convert.ToByte(enumValue);
        }
        public T Get<T>() where T : Enum
        {
            Type type = typeof(T);

            if (!_values.TryGetValue(type, out byte storedValue))
            {
                storedValue = 0;
                _values[type] = 0;
            }

            return (T)Enum.ToObject(type, storedValue);
        }

    }
}