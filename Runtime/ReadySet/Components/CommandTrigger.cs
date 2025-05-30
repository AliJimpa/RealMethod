using UnityEngine;

namespace RealMethod
{
    [RequireComponent(typeof(Collider)), AddComponentMenu("RealMethod/General/CommandTrigger")]
    public sealed class CommandTrigger : MonoBehaviour
    {
        public Command[] EnterCommands;
        public Command[] ExitCommand;

        private void OnValidate()
        {
            if (!GetComponent<Collider>().isTrigger)
            {
                GetComponent<Collider>().isTrigger = true;
            }
        }
        private void Awake()
        {
            if (EnterCommands != null)
            {
                foreach (var command in EnterCommands)
                {
                    if (!command.Initiate(this))
                    {
                        Debug.LogWarning($"Failed to initiate command '{command.name}' in {nameof(CommandTrigger)} on GameObject '{gameObject.name}'.");
                    }
                }
                
                foreach (var command in ExitCommand)
                {
                    if (!command.Initiate(this))
                    {
                        Debug.LogWarning($"Failed to initiate command '{command.name}' in {nameof(CommandTrigger)} on GameObject '{gameObject.name}'.");
                    }
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (EnterCommands != null)
            {
                foreach (var command in EnterCommands)
                {
                    command.GetComponent<ICommandExecuter>().ExecuteCommand(other);
                }
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (ExitCommand != null)
            {
                foreach (var command in ExitCommand)
                {
                    command.GetComponent<ICommandExecuter>().ExecuteCommand(other);
                }
            }
        }

    }
}
