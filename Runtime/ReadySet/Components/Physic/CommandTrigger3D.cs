using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Physic/CommandTrigger3D")]
    public sealed class CommandTrigger3D : Trigger3D
    {
        // Unity Methods
        private void Awake()
        {
            foreach (var command in GetComponents<ICommandInitiator>())
            {
                command.Initiate(null, this);
            }
        }

        // Trigger3D Methods
        protected override void OnEnter(Collider other)
        {
            foreach (var command in GetComponents<ICommandExecuter>())
            {
                command.ExecuteCommand(other);
            }
        }
        protected override void OnStay(Collider other)
        {
            foreach (var command in GetComponents<ICommandExecuter>())
            {
                command.ExecuteCommand(other);
            }
        }
        protected override void OnExit(Collider other)
        {
            foreach (var command in GetComponents<ICommandExecuter>())
            {
                command.ExecuteCommand(other);
            }
        }
    }
}
