using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/RPG/StatContainer")]
    public class StatContainerComponent : StatContainer
    {
        [Header("Events")]
        public UnityEvent<bool> OnListChanged;

        protected override void OnStatAdded(IStat stat)
        {
            OnListChanged?.Invoke(true);
        }
        protected override void OnStatRemoved(string statName)
        {
            OnListChanged?.Invoke(false);
        }
    }
}