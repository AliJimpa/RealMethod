using UnityEngine;
using UnityEngine.InputSystem;

namespace RealMethod
{
    public abstract class AbilityController : MonoBehaviour
    {
        [System.Serializable]
        private class InputAbility : SerializableDictionary<InputActionReference, AbilityAction> { }
        [Header("Setting")]
        [SerializeField]
        private InputAbility abilities;
        // [SerializeField]
        // private Transform aimTransform;


        private void Awake()
        {
            foreach (var abil in abilities)
            {
                ((IAbilityInitializer)abil.Key).Initializer(this);
            }
        }
        private void OnEnable()
        {
            foreach (var abil in abilities)
            {
                abil.Key.action.performed += abil.Value.UseInput;
            }
        }
        private void OnDisable()
        {
            foreach (var abil in abilities)
            {
                abil.Key.action.performed -= abil.Value.UseInput;
            }
        }


        // GameObject GetTarget()
        // {
        //     // Simplified raycast targeting
        //     if (Physics.Raycast(aimTransform.position, aimTransform.forward, out RaycastHit hit, 100f))
        //     {
        //         return hit.collider.gameObject;
        //     }
        //     return null;
        // }


    }




}