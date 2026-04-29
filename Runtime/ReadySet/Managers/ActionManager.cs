using System;
using UnityEngine;

namespace RealMethod
{

    public interface IAction : IHandle
    {
        void Starter();
    }
    public interface IActionTask : ITask<IAction>
    {

    }

    public sealed class ActionClass : SoftType<IActionTask>
    {

    }



    [AddComponentMenu("RealMethod/Manager/ActionManager")]
    public sealed class ActionManager : TaskManager<IActionTask, IAction>
    {
        [Header("Action")]
        private SoftType<ActionClass>[] DefaultActions;

        // TaskMaanger Methods
        protected override void InitiateManager(bool alwaysLoaded)
        {
            if (DefaultActions != null)
            {
                for (int i = 0; i < DefaultActions.Length; i++)
                {
                    Create(DefaultActions[i]);
                }
            }
        }
        protected override bool TryCreateNewInstance<P>(object ClassType, out P result)
        {
            if (ClassType is ActionClass ActionType)
            {
                result = ActionType.Type.CreateInstance() as P;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
        protected override void AutoStart(IAction Handle)
        {
            Handle.Starter();
        }

    }
}