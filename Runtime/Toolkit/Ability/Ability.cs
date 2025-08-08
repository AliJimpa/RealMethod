using UnityEngine;
using UnityEngine.InputSystem;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "Ability", menuName = "RealMethod/Ability/Asset", order = 0)]
    public class Ability : AbilityAction
    {
        [Header("Setting")]
        [SerializeField]
        private float cooldown = 2;

        protected override void OnInitiate()
        {
            throw new System.NotImplementedException();
        }

        protected override bool Prerequisite(GameObject user)
        {
            // Resource
            return true;
        }

        protected override bool CheckInput(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        protected override float GetCooldown()
        {
            return cooldown;
        }
    }



}