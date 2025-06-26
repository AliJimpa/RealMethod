using System;
using System.Collections.Generic;
using UnityEditor;
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
            Add,
            Active,
            Deactive,
            Remove,
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

        public Action<Power> OnAddPower;
        public Action<Power> OnActivePower;
        public Action<Power> OnDeactivePower;
        public Action<Power> OnRemovePower;
        public Action<Power> OnModifiyPower;

        private Hictionary<Power> Abilities = new Hictionary<Power>(5);


        public bool HasAbility => Abilities.Count > 0;
        public int Count => Abilities.Count;

        public Power this[string label]
        {
            get => Abilities[label];
            protected set => Abilities[label] = value;
        }


        protected virtual void Awake()
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

        public bool Active(string label)
        {
            if (Abilities.ContainsKey(label))
            {
                Abilities[label].GetComponent<ICommandLife>().StartCommand();
                MessageBehavior(AbilityState.Active, Abilities[label]);
                return true;
            }
            else
            {
                Debug.LogError($"Active Ability Failed: No ability with ID '{label}' exists.");
                return false;
            }
        }
        public bool Active(string label, float Duration)
        {
            if (Abilities.ContainsKey(label))
            {
                Abilities[label].GetComponent<ICommandLife>().StartCommand(Duration);
                MessageBehavior(AbilityState.Active, Abilities[label]);
                return true;
            }
            else
            {
                Debug.LogError($"Active Ability Failed: No ability with ID '{label}' exists.");
                return false;
            }
        }
        public bool Deactive(string label)
        {
            if (Abilities.ContainsKey(label))
            {
                Abilities[label].GetComponent<ICommandLife>().StopCommand();
                MessageBehavior(AbilityState.Deactive, Abilities[label]);
                return true;
            }
            else
            {
                Debug.LogError($"Active Ability Failed: No ability with ID '{label}' exists.");
                return false;
            }
        }
        public bool Pause(string label)
        {
            if (Abilities.ContainsKey(label))
            {
                Abilities[label].GetComponent<ICommandBehaviour>().PauseCommand();
                MessageBehavior(AbilityState.Modefiy, Abilities[label]);
                return true;
            }
            else
            {
                Debug.LogError($"Pause Ability Failed: No ability with ID '{label}' exists.");
                return false;
            }
        }
        public bool Resume(string label)
        {
            if (Abilities.ContainsKey(label))
            {
                Abilities[label].GetComponent<ICommandBehaviour>().ResumeCommand();
                MessageBehavior(AbilityState.Modefiy, Abilities[label]);
                return true;
            }
            else
            {
                Debug.LogError($"Resume Ability Failed: No ability with ID '{label}' exists.");
                return false;
            }
        }
        public bool ResetPower(string label)
        {
            if (Abilities.ContainsKey(label))
            {
                Abilities[label].GetComponent<ICommandBehaviour>().ResetCommand();
                MessageBehavior(AbilityState.Modefiy, Abilities[label]);
                return true;
            }
            else
            {
                Debug.LogError($"Active Ability Failed: No ability with ID '{label}' exists.");
                return false;
            }
        }
        public bool Apply(UnityEngine.Object author, Power command, Ability target)
        {
            if (!Abilities.ContainsKey(command.Label))
            {
                command.transform.SetParent(target.transform);
                Abilities.Add(command.Label, command);
                command.GetComponent<ICommandInitiator>().Initiate(author, this);
                MessageBehavior(AbilityState.Add, command);
                return true;
            }
            else
            {
                Debug.LogWarning($"Apply Ability Failed: An ability with ID '{command.Label}' already exists.");
                return false;
            }
        }
        public bool Deny(string label)
        {
            if (Abilities.ContainsKey(label))
            {
                Abilities[label].GetComponent<ICommandLife>().ClearCommand();
                Abilities[label].transform.SetParent(null);
                Abilities.Remove(label);
                MessageBehavior(AbilityState.Remove, null);
                return true;
            }
            else
            {
                Debug.LogWarning($"Deny Ability Failed:  No ability with ID '{label}' exists.");
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
            SpawnedObject.name = $"{command.Label}_Power";
            Power TargetCommand = SpawnedObject.GetComponent<Power>();
            TargetCommand.GetComponent<ICommandInitiator>().Initiate(author, this);
            Abilities.Add(TargetCommand.Label, TargetCommand);
            if (AutoActive)
            {
                Active(prefab.name);
            }
            else
            {
                SpawnedObject.gameObject.SetActive(false);
            }
            MessageBehavior(AbilityState.Add, TargetCommand);
            return true;
        }
        public bool Delete(string label)
        {
            if (Abilities.ContainsKey(label))
            {
                MessageBehavior(AbilityState.Remove, Abilities[label]);
                Destroy(Abilities[label].gameObject);
                Abilities.Remove(label);
                return true;
            }
            else
            {
                Debug.LogError($"Delete Ability Failed: No ability with ID '{label}' exists.");
                return false;
            }
        }
        public bool Delete(GameObject prefab)
        {
            Power Targetcommand = prefab.GetComponent<Power>();
            if (Targetcommand)
            {
                return Delete(Targetcommand.Label);
            }
            else
            {
                Debug.LogError($"Delete Ability Failed: The prefab with with ID '{Targetcommand.name}' Doesnot have AbillityCommand.");
                return false;
            }

        }
        public bool Move(string label, Ability target)
        {
            if (target == null)
            {
                Debug.LogError($"Move Ability Failed: Target Ability is null.");
                return false;
            }

            if (Abilities.ContainsKey(label))
            {
                Abilities[label].GetComponent<ICommandInitiator>().Initiate(this, target);
                Abilities[label].transform.SetParent(target.transform);
                target.Abilities.Add(label, Abilities[label]);
                target.MessageBehavior(AbilityState.Add, Abilities[label]);
                MessageBehavior(AbilityState.Remove, Abilities[label]);
                Abilities.Remove(label);
                return true;
            }
            else
            {
                Debug.LogError($"Delete Ability Failed: No ability with ID '{label}' exists.");
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
                    case AbilityState.Add:
                        OnAddPower?.Invoke(command);
                        break;
                    case AbilityState.Active:
                        OnActivePower?.Invoke(command);
                        break;
                    case AbilityState.Deactive:
                        OnDeactivePower?.Invoke(command);
                        break;
                    case AbilityState.Remove:
                        OnRemovePower?.Invoke(command);
                        break;
                    case AbilityState.Modefiy:
                        OnModifiyPower?.Invoke(command);
                        break;
                }
            }

            if (Behavior == AbilityBehavior.SendMessage || Behavior == AbilityBehavior.Both)
            {
                switch (State)
                {
                    case AbilityState.Add:
                        SendMessage("OnPowerAdded", command, MessageOption);
                        break;
                    case AbilityState.Active:
                        SendMessage("OnPowerActivated", command, MessageOption);
                        OnActivePower?.Invoke(command);
                        break;
                    case AbilityState.Deactive:
                        SendMessage("OnPowerDeactivated", command, MessageOption);
                        break;
                    case AbilityState.Remove:
                        SendMessage("OnPowerRemoved", command, MessageOption);
                        break;
                    case AbilityState.Modefiy:
                        SendMessage("OnPowerModefiy", command, MessageOption);
                        break;
                }
            }

            switch (State)
            {
                case AbilityState.Add:
                    AddAbility(command);
                    break;
                case AbilityState.Active:
                    ActiveAbility(command);
                    break;
                case AbilityState.Deactive:
                    DeactiveAbility(command);
                    break;
                case AbilityState.Remove:
                    RemoveAbility(command);
                    break;
                case AbilityState.Modefiy:
                    ModefiyAbility(command);
                    break;
            }
        }


        protected abstract void ActiveAbility(Power PowerUp);
        protected abstract void AddAbility(Power PowerUp);
        protected abstract void RemoveAbility(Power PowerUp);
        protected abstract void DeactiveAbility(Power PowerUp);
        protected abstract void ModefiyAbility(Power PowerUp);
    }


    public abstract class Power : ActionCommand
    {
        [Header("General")]
        [SerializeField]
        private string PowerLabel;
        [SerializeField, Tooltip("Ziro means infinit")]
        private float LifeTime;
        public float Duration => HiddeTime == -1 ? LifeTime : HiddeTime;
        public string Label => PowerLabel;

        // Private Variable
        private float HiddeTime = -1;


        // Implemented Override Methods
        protected sealed override void OnInitiate()
        {
            gameObject.SetActive(false);
            enabled = false;
            if (MyOwner is Ability Result)
            {
                OnAssign(Result);
            }
            else
            {
                Debug.LogError("Ther Owner Should be Ability Class");
            }
        }
        protected sealed override void OnBegin()
        {
            gameObject.SetActive(true);
            OnStartPower();
        }
        protected sealed override void OnUpdate()
        {
            OnTickPower(ElapsedTime / Duration);
        }
        protected sealed override void OnEnd()
        {
            gameObject.SetActive(false);
            enabled = false;
            OnFinishPower();
        }
        protected sealed override void OnPause()
        {
            enabled = false;
            OnPause(true);
        }
        protected sealed override void OnResume()
        {
            enabled = true;
            OnPause(false);
        }
        protected sealed override float PreProcessDuration(float StartDuration)
        {
            if (StartDuration > 0)
            {
                HiddeTime = StartDuration;
                return StartDuration;
            }
            else
            {
                HiddeTime = -1;
                return LifeTime;
            }
        }
        protected sealed override bool CanUpdate()
        {
            return enabled && gameObject.activeSelf;
        }


        // Abstract Method
        protected abstract void OnAssign(Ability target);
        protected abstract void OnStartPower();
        protected abstract void OnTickPower(float alpha);
        protected abstract void OnFinishPower();
        protected abstract void OnPause(bool stop);


        // Unity Method
        private void OnDestroy()
        {
            ((ICommandLife)this).StopCommand();
        }
    }



}