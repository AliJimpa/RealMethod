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

        // Actions
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


        // Unity Methods
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

        // Public Functions
        public bool IsValid(Power power)
        {
            // Ensure the command is a scene object, not an asset
#if UNITY_EDITOR
            if (UnityEditor.AssetDatabase.Contains(power.gameObject))
            {
                Debug.LogError($"Apply Power Failed: The Power '{power.Label}' is an asset, not a scene object.");
                return false;
            }
#endif
            return IsValid(power.Label);
        }
        public bool IsValid(string label)
        {
            return Abilities.ContainsKey(label);
        }
        public void Active(Power power)
        {
            // Ensure the command is a scene object, not an asset
#if UNITY_EDITOR
            if (UnityEditor.AssetDatabase.Contains(power.gameObject))
            {
                Debug.LogError($"Apply Power Failed: The Power '{power.Label}' is an asset, not a scene object.");
                return;
            }
#endif
            Active(power.Label);
        }
        public void Active(string label)
        {
            Abilities[label].GetComponent<ICommandLife>().StartCommand();
            MessageBehavior(AbilityState.Active, Abilities[label]);
        }
        public void Active(Power power, float Duration)
        {
            // Ensure the command is a scene object, not an asset
#if UNITY_EDITOR
            if (UnityEditor.AssetDatabase.Contains(power.gameObject))
            {
                Debug.LogError($"Apply Power Failed: The Power '{power.Label}' is an asset, not a scene object.");
                return;
            }
#endif
            Active(power.Label, Duration);
        }
        public void Active(string label, float Duration)
        {
            Abilities[label].GetComponent<ICommandLife>().StartCommand(Duration);
            MessageBehavior(AbilityState.Active, Abilities[label]);
        }
        public void Deactive(Power power)
        {
            // Ensure the command is a scene object, not an asset
#if UNITY_EDITOR
            if (UnityEditor.AssetDatabase.Contains(power.gameObject))
            {
                Debug.LogError($"Apply Power Failed: The Power '{power.Label}' is an asset, not a scene object.");
                return;
            }
#endif
            Deactive(power.Label);
        }
        public void Deactive(string label)
        {
            Abilities[label].GetComponent<ICommandLife>().StopCommand();
            MessageBehavior(AbilityState.Deactive, Abilities[label]);
        }
        public void Pause(Power power)
        {
            // Ensure the command is a scene object, not an asset
#if UNITY_EDITOR
            if (UnityEditor.AssetDatabase.Contains(power.gameObject))
            {
                Debug.LogError($"Apply Power Failed: The Power '{power.Label}' is an asset, not a scene object.");
                return;
            }
#endif
            Pause(power.Label);
        }
        public void Pause(string label)
        {
            Abilities[label].GetComponent<ICommandBehaviour>().PauseCommand();
            MessageBehavior(AbilityState.Modefiy, Abilities[label]);
        }
        public void Resume(Power power)
        {
            // Ensure the command is a scene object, not an asset
#if UNITY_EDITOR
            if (UnityEditor.AssetDatabase.Contains(power.gameObject))
            {
                Debug.LogError($"Apply Power Failed: The Power '{power.Label}' is an asset, not a scene object.");
                return;
            }
#endif
            Resume(power.Label);
        }
        public void Resume(string label)
        {
            Abilities[label].GetComponent<ICommandBehaviour>().ResumeCommand();
            MessageBehavior(AbilityState.Modefiy, Abilities[label]);
        }
        public void ResetPower(Power power)
        {
            // Ensure the command is a scene object, not an asset
#if UNITY_EDITOR
            if (UnityEditor.AssetDatabase.Contains(power.gameObject))
            {
                Debug.LogError($"Apply Power Failed: The Power '{power.Label}' is an asset, not a scene object.");
                return;
            }
#endif
            ResetPower(power.Label);
        }
        public void ResetPower(string label)
        {
            if (Abilities[label].IsActive && !Abilities[label].IsPaused)
            {
                Abilities[label].GetComponent<ICommandBehaviour>().RestartCommand();
            }
            else
            {
                Abilities[label].GetComponent<ICommandBehaviour>().ResetCommand();
            }
            MessageBehavior(AbilityState.Modefiy, Abilities[label]);
        }
        public bool Apply(Power power, Transform target, UnityEngine.Object author)
        {
            // Ensure the command is a scene object, not an asset
#if UNITY_EDITOR
            if (UnityEditor.AssetDatabase.Contains(power.gameObject))
            {
                Debug.LogError($"Apply Power Failed: The Power '{power.Label}' is an asset, not a scene object.");
                return false;
            }
#endif
            if (!Abilities.ContainsKey(power.Label))
            {
                power.transform.SetParent(target);
                Abilities.Add(power.Label, power);
                power.GetComponent<ICommandInitiator>().Initiate(author, this);
                MessageBehavior(AbilityState.Add, power);
                return true;
            }
            else
            {
                Debug.LogWarning($"Apply Power Failed: An ability with ID '{power.Label}' already exists.");
                return false;
            }
        }
        public bool Deny(Power power)
        {
            // Ensure the command is a scene object, not an asset
#if UNITY_EDITOR
            if (UnityEditor.AssetDatabase.Contains(power.gameObject))
            {
                Debug.LogError($"Apply Power Failed: The Power '{power.Label}' is an asset, not a scene object.");
                return false;
            }
#endif
            return Deny(power.Label);
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
        public Power Create(GameObject prefab, UnityEngine.Object author, bool AutoActive = false)
        {
            return Create(prefab, author, transform, AutoActive);
        }
        public Power Create(GameObject prefab, UnityEngine.Object author, Transform parent, bool AutoActive = false)
        {
            Power command = prefab.GetComponent<Power>();
            if (command == null)
            {
                Debug.LogError($"Create Power Failed: The GameObject '{prefab.name}' does not have a command of type '{typeof(Power).Name}'.");
                return null;
            }

            if (author == null)
            {
                Debug.LogError($"Create Power Failed: Author Not valid.");
                return null;
            }

            if (Abilities.ContainsKey(command.Label))
            {
                Debug.LogError($"Create Power Failed: An ability with ID '{prefab.name}' already exists.");
                return null;
            }

            GameObject SpawnedObject = Instantiate(prefab, parent);
            SpawnedObject.name = $"{command.Label}_Power";
            Power SpawnedPower = SpawnedObject.GetComponent<Power>();
            SpawnedPower.GetComponent<ICommandInitiator>().Initiate(author, this);
            Abilities.Add(SpawnedPower.Label, SpawnedPower);
            if (AutoActive)
            {
                Active(SpawnedPower.Label);
            }
            MessageBehavior(AbilityState.Add, SpawnedPower);
            return SpawnedPower;
        }
        public bool Delete(Power power)
        {
            // Ensure the command is a scene object, not an asset
#if UNITY_EDITOR
            if (UnityEditor.AssetDatabase.Contains(power.gameObject))
            {
                Debug.LogError($"Apply Power Failed: The Power '{power.Label}' is an asset, not a scene object.");
                return false;
            }
#endif
            return Delete(power.Label);
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
                Debug.LogError($"Delete Power Failed: No ability with ID '{label}' exists.");
                return false;
            }
        }
        public bool Move(string label, Ability target)
        {
            if (target == null)
            {
                Debug.LogError($"Move Power Failed: Target Ability is null.");
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
                Debug.LogError($"Delete Power Failed: No ability with ID '{label}' exists.");
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
        [Header("Power")]
        [SerializeField]
        private string label;
        public string Label => label;
        [SerializeField]
        protected bool Tick = true;
        [SerializeField, Tooltip("Zero means infinit"), ConditionalHide("Tick", true, false)]
        private float LifeTime;
        public bool IsActive => !IsFinished;


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
                Debug.LogError("The 'Owner' should be 'Ability' Class!");
            }
        }
        protected sealed override void OnBegin()
        {
            gameObject.SetActive(true);
            enabled = true;
            OnStart(true);
        }
        protected sealed override void OnUpdate()
        {
            OnTick(1 - NormalizedTime);
        }
        protected sealed override void OnEnd()
        {
            gameObject.SetActive(false);
            enabled = false;
            OnStop(true);
        }
        protected sealed override void OnResume()
        {
            enabled = true;
            OnStart(false);
        }
        protected sealed override void OnPause()
        {
            enabled = false;
            OnStop(false);
        }
        protected sealed override float PreProcessDuration(float StartDuration)
        {
            return StartDuration > 0 ? StartDuration : LifeTime;
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
        protected abstract void OnAssign(Ability Owner);
        protected abstract void OnStart(bool IsBegin);
        protected abstract void OnTick(float Alpha);
        protected abstract void OnStop(bool IsFinish);


#if UNITY_EDITOR
        protected void ChangeLabel(string NewName)
        {
            label = NewName;
        }
#endif

    }

}