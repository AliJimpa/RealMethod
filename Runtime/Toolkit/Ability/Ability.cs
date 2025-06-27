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
        public bool Apply(Power command, Transform target, UnityEngine.Object author)
        {
            if (!Abilities.ContainsKey(command.Label))
            {
                command.transform.SetParent(target);
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
            return Create(prefab, transform, author, AutoActive);
        }
        public bool Create(GameObject prefab, Transform parent, UnityEngine.Object author, bool AutoActive = false)
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

            GameObject SpawnedObject = Instantiate(prefab, parent);
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
                Power TargetPower = Abilities[label];
                Deny(label);
                target.Apply(TargetPower, target.transform, this);
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
                        SendMessage("OnAddPower", command, MessageOption);
                        break;
                    case AbilityState.Active:
                        SendMessage("OnActivePower", command, MessageOption);
                        OnActivePower?.Invoke(command);
                        break;
                    case AbilityState.Deactive:
                        SendMessage("OnDeactivePower", command, MessageOption);
                        break;
                    case AbilityState.Remove:
                        SendMessage("OnRemovePower", command, MessageOption);
                        break;
                    case AbilityState.Modefiy:
                        SendMessage("OnPowerModefiy", command, MessageOption);
                        break;
                }
            }

            switch (State)
            {
                case AbilityState.Add:
                    OnPowerAdded(command);
                    break;
                case AbilityState.Active:
                    OnPowerActivated(command);
                    break;
                case AbilityState.Deactive:
                    OnPowerDeactivated(command);
                    break;
                case AbilityState.Remove:
                    OnPowerRemoved(command);
                    break;
                case AbilityState.Modefiy:
                    OnPowerModefied(command);
                    break;
            }
        }

        protected abstract void OnPowerAdded(Power PowerUp);
        protected abstract void OnPowerActivated(Power PowerUp);
        protected abstract void OnPowerDeactivated(Power PowerUp);
        protected abstract void OnPowerRemoved(Power PowerUp);
        protected abstract void OnPowerModefied(Power PowerUp);
    }


    public abstract class Power : ActionCommand
    {
        [Header("General")]
        [SerializeField]
        private string PowerLabel;
        [SerializeField]
        protected bool Tick = true;
        [SerializeField, Tooltip("Ziro means infinit"), ConditionalHide("Tick", true, false)]
        private float LifeTime;
        public string Label => PowerLabel;

        // Private Variable
        private float Duration = 0;


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
            enabled = true;
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
            if (Tick)
            {
                return 0;
            }

            Duration = StartDuration > 0 ? StartDuration : LifeTime;
            return Duration;
        }
        protected sealed override bool CanUpdate()
        {
            if (Tick)
            {
                return enabled && gameObject.activeSelf;
            }
            else
            {
                return false;
            }
        }

        // Abstract Method
        protected abstract void OnAssign(Ability target);
        protected abstract void OnStartPower();
        protected abstract void OnTickPower(float alpha);
        protected abstract void OnFinishPower();
        protected abstract void OnPause(bool stop);

        // Unity Method
        protected virtual void OnDestroy()
        {
            Finish();
        }


#if UNITY_EDITOR
        protected void ChangeLabel(string NewName)
        {
            PowerLabel = NewName;
        }
#endif

    }



}