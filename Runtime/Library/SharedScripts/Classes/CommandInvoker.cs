using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public class CommandInvoker
    {
        private readonly Queue<ICommand> _commandQueue = new Queue<ICommand>();
        private Object Owner;


        public CommandInvoker(Object owner)
        {
            Owner = owner;
        }

        public void AddCommand(ICommand command)
        {
            AddCommand(command, null);
        }
        public void AddCommand(ICommand command, Object author)
        {
            command.Initiate(author, Owner);
            _commandQueue.Enqueue(command);
        }

        public void ExecuteCommands()
        {
            while (_commandQueue.Count > 0)
            {
                var command = _commandQueue.Dequeue();
                command.ExecuteCommand(this);
            }
        }

    }
}