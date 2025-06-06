using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public abstract class Ability : MonoBehaviour
    {
        private enum AbilityBehavior
        {
            Nothing,
            SendMessage,
            Action,
            Both,
        }
        public enum AbilityState
        {
            Create,
            Active,
            Deactive,
            Delete,
            Move,
        }

        [Header("Setting")]
        [SerializeField]
        private GameObject[] StartAbility;
        [SerializeField]
        private UpdateMethod Method = UpdateMethod.Update;
        [SerializeField]
        private AbilityBehavior Behavior;

        public Action<AbilityCommand> OnCreated;
        public Action<AbilityCommand> OnActive;
        public Action<AbilityCommand> OnDeactive;
        public Action<AbilityCommand> OnDeleted;

        private Hictionary<AbilityCommand> Abilities = new Hictionary<AbilityCommand>();


        public AbilityCommand[] ActiveAbilityCommand
        {
            get
            {
                var result = new List<AbilityCommand>();
                foreach (var ability in Abilities.GetValues())
                {
                    result.Add(ability);
                }
                return result.ToArray();
            }
        }
        public bool HasAbility => Abilities.Count > 0;
        public AbilityCommand this[string Name]
        {
            get => Abilities[Name];
            set => Abilities[Name] = value;
        }


        private void Awake()
        {
            if (StartAbility != null)
            {
                foreach (var ablilty in StartAbility)
                {
                    Apply(ablilty, this);
                }
            }
        }

        private void LateUpdate()
        {
            if (Method == UpdateMethod.LateUpdate)
            {
                UpdateAbility();
            }
        }
        private void Update()
        {
            if (Method == UpdateMethod.Update)
            {
                UpdateAbility();
            }
        }
        private void FixedUpdate()
        {
            if (Method == UpdateMethod.FixedUpdate)
            {
                UpdateAbility();
            }
        }

        public bool Active(string name)
        {
            if (Abilities.ContainsKey(name))
            {
                Abilities[name].GetComponent<ILiveCommand>().StartCommand();
                MessageBehavior(AbilityState.Active, Abilities[name]);
                return true;
            }
            else
            {
                Debug.LogError($"Active Ability Failed: No ability with ID '{name}' exists.");
                return false;
            }
        }
        public bool Deactive(string name)
        {
            if (Abilities.ContainsKey(name))
            {
                Abilities[name].GetComponent<ILiveCommand>().StopCommand();
                MessageBehavior(AbilityState.Deactive, Abilities[name]);
                return true;
            }
            else
            {
                Debug.LogError($"Active Ability Failed: No ability with ID '{name}' exists.");
                return false;
            }
        }
        public bool Apply(GameObject prefab, object author, bool AutoActive = false)
        {
            AbilityCommand command = prefab.GetComponent<AbilityCommand>();
            if (command == null)
            {
                Debug.LogError($"Create Ability Failed: The GameObject '{prefab.name}' does not have a command of type '{typeof(AbilityCommand).Name}'.");
                return false;
            }

            if (author == null)
            {
                Debug.LogError($"Spawn Command Failed: Author Not valid.");
                return false;
            }

            if (Abilities.ContainsKey(prefab.name))
            {
                Debug.LogError($"Create Ability Failed: An ability with ID '{prefab.name}' already exists.");
                return false;
            }

            GameObject SpawnedObject = Instantiate(prefab, transform);
            SpawnedObject.name = $"Ability_{prefab.name}";
            AbilityCommand TargetCommand = SpawnedObject.GetComponent<AbilityCommand>();
            TargetCommand.GetComponent<ICommandInitiator>().Initiate(author, this);
            Abilities.Add(prefab.name, TargetCommand);
            if (AutoActive)
            {
                Active(prefab.name);
            }
            MessageBehavior(AbilityState.Create, TargetCommand);
            return true;
        }
        public bool Deny(string name)
        {
            if (Abilities.ContainsKey(name))
            {
                MessageBehavior(AbilityState.Delete, Abilities[name]);
                Destroy(Abilities[name].gameObject);
                Abilities.Remove(name);
                return true;
            }
            else
            {
                Debug.LogError($"Delete Ability Failed: No ability with ID '{name}' exists.");
                return false;
            }
        }
        public bool Deny(GameObject prefab)
        {
            return Deny(prefab.name);
        }
        public bool Move(string name, Ability target)
        {
            if (target == null)
            {
                Debug.LogError($"Move Ability Failed: Target Ability is null.");
                return false;
            }

            if (Abilities.ContainsKey(name))
            {
                Abilities[name].GetComponent<ICommandInitiator>().Initiate(this, target);
                Abilities[name].transform.SetParent(target.transform);
                target.Abilities.Add(name, Abilities[name]);
                target.MessageBehavior(AbilityState.Delete, Abilities[name]);
                MessageBehavior(AbilityState.Delete, Abilities[name]);
                Abilities.Remove(name);
                return true;
            }
            else
            {
                Debug.LogError($"Delete Ability Failed: No ability with ID '{name}' exists.");
                return false;
            }


        }

        private void UpdateAbility()
        {
            foreach (var ability in Abilities.GetValues())
            {
                ability.GetComponent<ILiveCommand>().UpdateCommand();
            }
        }
        private void MessageBehavior(AbilityState State, AbilityCommand command)
        {
            if (Behavior == AbilityBehavior.Action || Behavior == AbilityBehavior.Both)
            {
                switch (State)
                {
                    case AbilityState.Create:
                        OnCreated?.Invoke(command);
                        break;
                    case AbilityState.Active:
                        OnActive?.Invoke(command);
                        break;
                    case AbilityState.Deactive:
                        OnDeactive?.Invoke(command);
                        break;
                    case AbilityState.Delete:
                        OnDeleted?.Invoke(command);
                        break;
                    case AbilityState.Move:
                        break;
                }
            }
            else
            {
                SendMessage("OnAbilityUpdated", State, SendMessageOptions.RequireReceiver);
            }

        }



    }

    public abstract class AbilityCommand : LifecycleCommand
    {
        [SerializeField, Tooltip("Ziro means infinit")]
        private float LifeTime;
        public float Duration => LifeTime;

        // Implemented Override Methods
        protected sealed override void OnInitiate()
        {
            enabled = true;
            if (MyAuthor is Ability && MyOwner is Ability)
            {
                OnAssign(MyAuthor as Ability);
            }
            else
            {
                OnAssign(MyOwner as Ability);
            }
        }
        protected sealed override float PreProcessDuration(float StartDuration)
        {
            return LifeTime;
        }
        protected sealed override bool CanUpdate()
        {
            return enabled;
        }

        // Abstract Method
        protected abstract void OnAssign(Ability target);

        // Unity Method
        private void OnDestroy()
        {
            ((ILiveCommand)this).StopCommand();
        }
    }

}