using System;
using System.Collections.Generic;

namespace RealMethod
{
    public sealed class EnumStorage : IDisposable
    {
        private GlobalEnum _state;
        private readonly Dictionary<Type, byte> _values = new();
        /// <summary>
        /// Invoked when game state changed.
        /// </summary>
        public event Action<GlobalEnum, GlobalEnum> OnStateChanged;

        public GlobalEnum Global
        {
            get => _state;
            set
            {
                if (_state == value)
                    return;

                var old = _state;
                _state = value;

                OnStateChanged?.Invoke(old, _state);
            }
        }

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


        // -------- Operator Overloads --------
        public static implicit operator GlobalEnum(EnumStorage storage)
        {
            return storage.Global;
        }

        public static implicit operator EnumStorage(GlobalEnum value)
        {
            return new EnumStorage { Global = value };
        }

        public static bool operator ==(EnumStorage a, GlobalEnum b)
            => a is not null && a._state == b;

        public static bool operator !=(EnumStorage a, GlobalEnum b)
            => !(a == b);

        public static bool operator <(EnumStorage a, GlobalEnum b)
            => a is not null && a._state < b;

        public static bool operator >(EnumStorage a, GlobalEnum b)
            => a is not null && a._state > b;

        public static bool operator <=(EnumStorage a, GlobalEnum b)
            => a is not null && a._state <= b;

        public static bool operator >=(EnumStorage a, GlobalEnum b)
            => a is not null && a._state >= b;

        public static bool operator ==(GlobalEnum a, EnumStorage b)
            => b == a;

        public static bool operator !=(GlobalEnum a, EnumStorage b)
            => !(b == a);

        public static bool operator <(GlobalEnum a, EnumStorage b)
            => b is not null && a < b._state;

        public static bool operator >(GlobalEnum a, EnumStorage b)
            => b is not null && a > b._state;

        public static bool operator <=(GlobalEnum a, EnumStorage b)
            => b is not null && a <= b._state;

        public static bool operator >=(GlobalEnum a, EnumStorage b)
            => b is not null && a >= b._state;

        // -------- Overrides --------

        public override bool Equals(object obj)
        {
            if (obj is EnumStorage other)
                return _state.Equals(other._state);

            if (obj is GlobalEnum e)
                return _state.Equals(e);

            return false;
        }
        public override int GetHashCode()
        {
            return _state.GetHashCode();
        }
        public override string ToString()
        {
            return _state.ToString();
        }
    }
}