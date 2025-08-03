using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Physic/Trigger2D")]
    public sealed class CommandTrigger2D : Trigger2D
    {
        // Unity Methods
        private void Awake()
        {
            foreach (var command in GetComponents<ICommand>())
            {
                command.Initiate(null, this);
            }
        }

        // Trigger3D Methods
        protected override void OnEnter(Collider2D other)
        {
            foreach (var command in GetComponents<ICommand>())
            {
                command.ExecuteCommand(other);
            }
        }
        protected override void OnStay(Collider2D other)
        {
            foreach (var command in GetComponents<ICommand>())
            {
                command.ExecuteCommand(other);
            }
        }
        protected override void OnExit(Collider2D other)
        {
            foreach (var command in GetComponents<ICommand>())
            {
                command.ExecuteCommand(other);
            }
        }
    }
}
