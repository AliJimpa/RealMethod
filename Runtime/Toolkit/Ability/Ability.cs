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
        }


        [Header("Setting")]
        [SerializeField]
        private GameObject[] StartAbility;
        [SerializeField]
        private UpdateMethod Method = UpdateMethod.Update;
        [SerializeField]
        private AbilityBehavior Behavior;
        [SerializeField, ShowInInspectorByEnum("Behavior", 1, 3)]
        private SendMessageOptions MessageOption;

        public Action<AbilityCommand> OnCreated;
        public Action<AbilityCommand> OnActive;
        public Action<AbilityCommand> OnDeactive;
        public Action<AbilityCommand> OnDeleted;

        private Hictionary<AbilityCommand> Abilities = new Hictionary<AbilityCommand>(5);


        public bool HasAbility => Abilities.Count > 0;
        public int Count => Abilities.Count;

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
                    Create(ablilty, this);
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
                Abilities[name].GetComponent<ICommandLife>().StartCommand();
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
                Abilities[name].GetComponent<ICommandLife>().StopCommand();
                MessageBehavior(AbilityState.Deactive, Abilities[name]);
                return true;
            }
            else
            {
                Debug.LogError($"Active Ability Failed: No ability with ID '{name}' exists.");
                return false;
            }
        }
        public bool Apply(UnityEngine.Object author, AbilityCommand command, Ability target)
        {
            if (!Abilities.ContainsKey(command.Name))
            {
                command.transform.SetParent(target.transform);
                Abilities.Add(command.Name, command);
                command.GetComponent<ICommandInitiator>().Initiate(author, this);
                MessageBehavior(AbilityState.Create, command);
                return true;
            }
            else
            {
                Debug.LogWarning($"Apply Ability Failed: An ability with ID '{command.Name}' already exists.");
                return false;
            }
        }
        public bool Create(GameObject prefab, UnityEngine.Object author, bool AutoActive = false)
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
            SpawnedObject.name = $"Ability_{command.Name}";
            AbilityCommand TargetCommand = SpawnedObject.GetComponent<AbilityCommand>();
            TargetCommand.GetComponent<ICommandInitiator>().Initiate(author, this);
            Abilities.Add(TargetCommand.Name, TargetCommand);
            if (AutoActive)
            {
                Active(prefab.name);
            }
            MessageBehavior(AbilityState.Create, TargetCommand);
            return true;
        }
        public bool Delete(string name)
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
        public bool Delete(GameObject prefab)
        {
            AbilityCommand Targetcommand = prefab.GetComponent<AbilityCommand>();
            if (Targetcommand)
            {
                return Delete(Targetcommand.Name);
            }
            else
            {
                Debug.LogError($"Delete Ability Failed: The prefab with with ID '{Targetcommand.name}' Doesnot have AbillityCommand.");
                return false;
            }

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
                target.MessageBehavior(AbilityState.Create, Abilities[name]);
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
        public AbilityCommand[] CopyActiveAbilities()
        {
            var result = new List<AbilityCommand>();
            foreach (var ability in Abilities.GetValues())
            {
                if (!ability.IsFinished)
                {
                    result.Add(ability);
                }
            }
            return result.ToArray();
        }
        public AbilityCommand[] CopyAbilities()
        {
            return Abilities.GetValues();
        }

        private void UpdateAbility()
        {
            foreach (var ability in Abilities.GetValues())
            {
                ability.GetComponent<ICommandLife>().UpdateCommand();
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
                }
            }

            if (Behavior == AbilityBehavior.SendMessage || Behavior == AbilityBehavior.Both)
            {
                switch (State)
                {
                    case AbilityState.Create:
                        SendMessage("OnAbilityCreate", command, MessageOption);
                        break;
                    case AbilityState.Active:
                        SendMessage("OnAbilityActive", command, MessageOption);
                        OnActive?.Invoke(command);
                        break;
                    case AbilityState.Deactive:
                        SendMessage("OnAbilityDeactive", command, MessageOption);
                        break;
                    case AbilityState.Delete:
                        SendMessage("OnAbilityDelete", command, MessageOption);
                        break;
                }
            }

            switch (State)
            {
                case AbilityState.Create:
                    CreateAbility(command);
                    break;
                case AbilityState.Active:
                    ActiveAbility(command);
                    break;
                case AbilityState.Deactive:
                    DeactiveAbility(command);
                    break;
                case AbilityState.Delete:
                    DeleteAbility(command);
                    break;
            }
        }


        protected abstract void ActiveAbility(AbilityCommand Ability);
        protected abstract void CreateAbility(AbilityCommand Ability);
        protected abstract void DeleteAbility(AbilityCommand Ability);
        protected abstract void DeactiveAbility(AbilityCommand Ability);
    }


    public abstract class AbilityCommand : LifecycleCommand
    {
        [Header("General")]
        [SerializeField]
        private string AbilityName;
        [SerializeField, Tooltip("Ziro means infinit")]
        private float LifeTime;
        public float Duration => LifeTime;
        public string Name => AbilityName;

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
            ((ICommandLife)this).StopCommand();
        }
    }

}