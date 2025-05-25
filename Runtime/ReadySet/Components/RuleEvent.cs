using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    public class RuleEvent : MonoBehaviour
    {
        [Header("Rule")]
        [SerializeField]
        private string Rule;
        private bool CallOnAwake = false;
        [SerializeField]
        private UnityEvent<bool> RuleEffect;
        [Header("Advance")]
        [SerializeField]
        private string ServiceName = "GameRule";

        private RuleService RuleBox;

        private void Awake()
        {
            if (Game.TryFindService(ServiceName, out RuleBox))
            {
                if (RuleBox.IsValid(Rule))
                {
                    RuleBox.BindRule(Rule, OnRuleChanged);
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
                Debug.LogError($"RuleEvent: Could not find service '{ServiceName}' for Rule '{Rule}' on GameObject '{gameObject.name}'.");
                enabled = false;
            }
        }
        private void OnDisable()
        {
            RuleBox.OnAddedRule -= OnNewRuleCreate;
        }

        public string GetRuleName()
        {
            return Rule;
        }
        
        private void OnNewRuleCreate(string Name)
        {
            if (Name == Rule)
            {
                RuleBox.BindRule(Rule, OnRuleChanged);
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
