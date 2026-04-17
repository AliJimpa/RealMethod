using System;
using System.Collections.Generic;

public enum GameEvent
{
    Spawn,
    Despawn,
    Die,
    TakeDamage,
    Heal,
    Attach,
    Detach,
    Share,
    Reset
}


public static class EventBus
{
    private static readonly Dictionary<string, Action<object>> _eventMap =
        new Dictionary<string, Action<object>>();

    public static void Subscribe(string eventName, Action<object> callback)
    {
        if (!_eventMap.ContainsKey(eventName))
            _eventMap[eventName] = delegate { };

        _eventMap[eventName] += callback;
    }

    public static void Unsubscribe(string eventName, Action<object> callback)
    {
        if (_eventMap.ContainsKey(eventName))
            _eventMap[eventName] -= callback;
    }

    public static void Raise(string eventName, object payload = null)
    {
        if (_eventMap.TryGetValue(eventName, out var callbacks))
            callbacks?.Invoke(payload);
    }

    public static void Raise(GameEvent ev, object payload = null)
        => Raise(ev.ToName(), payload);
}

// EventBus.Subscribe(GameEvent.Spawn.ToName(), OnSpawn);
// private void OnSpawn(object payload)
// {
//     // Handle your event
// }
// EventBus.Raise(GameEvent.Spawn);
// EventBus.Raise(GameEvent.TakeDamage, new DamageInfo { amount = 20 });


public static class GameEventExtensions
{
    public static string ToName(this GameEvent ev)
    {
        return ev.ToString();
    }
}

public static class TypedEventBus<T>
{
    private static event Action<T> OnEvent;

    public static void Subscribe(Action<T> callback) => OnEvent += callback;
    public static void Unsubscribe(Action<T> callback) => OnEvent -= callback;

    public static void Raise(T payload) => OnEvent?.Invoke(payload);
}

// TypedEventBus<DamageInfo>.Subscribe(OnDamage);
// TypedEventBus<DamageInfo>.Raise(new DamageInfo(25));

