using UnityEngine;

namespace RealMethod
{
    [RequireComponent(typeof(Collider)), AddComponentMenu("RealMethod/General/CommandTrigger")]
    public sealed class CommandTrigger : MonoBehaviour
    {
        public ExecutCommand[] EnterCommands;
        public ExecutCommand[] ExitCommand;

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
                    command.GetComponent<ICommandInitiator>().Initiate(this, this);
                }

                foreach (var command in ExitCommand)
                {
                    command.GetComponent<ICommandInitiator>().Initiate(this, this);
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
