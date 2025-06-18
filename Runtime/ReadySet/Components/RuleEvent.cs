using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    public sealed class RuleEvent : MonoBehaviour
    {
        [Header("Rule")]
        [SerializeField]
        private string Rule;
        [SerializeField]
        private bool CallOnAwake = false;
        [SerializeField]
        private UnityEvent<bool> RuleEffect;
        [Header("Advance")]
        [SerializeField]
        private string ServiceName = "GameRule";
        [SerializeField, ShowOnly]
        private bool IsConnectRule = false;

        private RuleService RuleBox;

        private void Awake()
        {
            if (Game.TryFindService(ServiceName, out RuleBox))
            {
                if (RuleBox.IsValid(Rule))
                {
                    RuleBox.BindRule(Rule, OnRuleChanged);
                    IsConnectRule = true;
                    if (CallOnAwake)
                    {
                        RuleEffect.Invoke(RuleBox.InEffect(Rule));
                    }
                }
                else
                {
                    RuleBox.OnAddedRule += OnNewRuleCreate;
                }
            }
            else
            {
                Debug.LogError($"RuleEvent: Could not find service '{ServiceName}' for on GameObject '{gameObject.name}'.");
                enabled = false;
            }
        }
        private void OnDisable()
        {
            if (RuleBox != null)
            {
                RuleBox.OnAddedRule -= OnNewRuleCreate;
            }
        }

        public string GetRuleName()
        {
            return Rule;
        }
        public bool IsConnect()
        {
            return IsConnectRule;
        }


        private void OnNewRuleCreate(string Name)
        {
            if (Name == Rule)
            {
                RuleBox.BindRule(Rule, OnRuleChanged);
                IsConnectRule = true;
                RuleBox.OnAddedRule -= OnNewRuleCreate;
            }
        }
        private void OnRuleChanged(Observer obs)
        {
            if (obs is Observer<bool> result)
            {
                RuleEffect?.Invoke(result.Value);
            }
        }

    }
}
