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
            Modefiy,
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

        public Action<Power> OnCreated;
        public Action<Power> OnActive;
        public Action<Power> OnDeactive;
        public Action<Power> OnDeleted;
        public Action<Power> OnModified;

        private Hictionary<Power> Abilities = new Hictionary<Power>(5);


        public bool HasAbility => Abilities.Count > 0;
        public int Count => Abilities.Count;

        public Power this[string Name]
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
                Abilities[name].gameObject.SetActive(true);
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
        public bool Active(string name, float Duration)
        {
            if (Abilities.ContainsKey(name))
            {
                Abilities[name].gameObject.SetActive(true);
                Abilities[name].GetComponent<ICommandLife>().StartCommand(Duration);
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
                Abilities[name].gameObject.SetActive(false);
                MessageBehavior(AbilityState.Deactive, Abilities[name]);
                return true;
            }
            else
            {
                Debug.LogError($"Active Ability Failed: No ability with ID '{name}' exists.");
                return false;
            }
        }
        public bool Pause(string name)
        {
            if (Abilities.ContainsKey(name))
            {
                Abilities[name].GetComponent<ICommandBehaviour>().PauseCommand();
                Abilities[name].enabled = false;
                MessageBehavior(AbilityState.Modefiy, Abilities[name]);
                return true;
            }
            else
            {
                Debug.LogError($"Pause Ability Failed: No ability with ID '{name}' exists.");
                return false;
            }
        }
        public bool Resume(string name)
        {
            if (Abilities.ContainsKey(name))
            {
                Abilities[name].GetComponent<ICommandBehaviour>().ResumeCommand();
                Abilities[name].enabled = true;
                MessageBehavior(AbilityState.Modefiy, Abilities[name]);
                return true;
            }
            else
            {
                Debug.LogError($"Resume Ability Failed: No ability with ID '{name}' exists.");
                return false;
            }
        }
        public bool Apply(UnityEngine.Object author, Power command, Ability target)
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
            Power command = prefab.GetComponent<Power>();
            if (command == null)
            {
                Debug.LogError($"Create Ability Failed: The GameObject '{prefab.name}' does not have a command of type '{typeof(Power).Name}'.");
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
            Power TargetCommand = SpawnedObject.GetComponent<Power>();
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
            Power Targetcommand = prefab.GetComponent<Power>();
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
        public Power[] CopyActiveAbilities()
        {
            var result = new List<Power>();
            foreach (var ability in Abilities.GetValues())
            {
                if (!ability.IsFinished)
                {
                    result.Add(ability);
                }
            }
            return result.ToArray();
        }
        public Power[] CopyAbilities()
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
        private void MessageBehavior(AbilityState State, Power command)
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
                    case AbilityState.Modefiy:
                        OnModified?.Invoke(command);
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
                    case AbilityState.Modefiy:
                        SendMessage("OnAbilityModify", command, MessageOption);
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
                case AbilityState.Modefiy:
                    ModefiyAbility(command);
                    break;
            }
        }


        protected abstract void ActiveAbility(Power Ability);
        protected abstract void CreateAbility(Power Ability);
        protected abstract void DeleteAbility(Power Ability);
        protected abstract void DeactiveAbility(Power Ability);
        protected abstract void ModefiyAbility(Power Ability);
    }


    public abstract class Power : ActionCommand
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
        protected override float PreProcessDuration(float StartDuration)
        {
            return LifeTime;
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