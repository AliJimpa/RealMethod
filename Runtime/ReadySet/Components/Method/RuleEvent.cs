using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Method/RuleEvent")]
    public sealed class RuleEvent : MonoBehaviour
    {
        [Header("Rule")]
        [SerializeField]
        private string Rule;
        [SerializeField]
        private bool CallOnAwake = false;
        [SerializeField]
        private UnityEvent<bool> RuleEffect;

        private RuleService ruleServ;
        public bool IsServiceRegistered => ruleServ != null;

        private void Awake()
        {
            if (Game.TryFindService(out ruleServ))
            {
                if (ruleServ.IsValid(Rule))
                {
                    ruleServ.BindRule(Rule, OnRuleChanged);
                    if (CallOnAwake)
                    {
                        RuleEffect.Invoke(ruleServ.InEffect(Rule));
                    }
                }
                else
                {
                    ruleServ.OnAddedRule += OnNewRuleCreate;
                }
            }
            else
            {
                Debug.LogError($"RuleEvent: Could not find 'RuleService' for on GameObject '{gameObject.name}'.");
                enabled = false;
            }
        }
        private void OnDisable()
        {
            if (ruleServ != null)
            {
                ruleServ.OnAddedRule -= OnNewRuleCreate;
            }
        }

        public string GetRuleName()
        {
            return Rule;
        }


        private void OnNewRuleCreate(string Name)
        {
            if (Name == Rule)
            {
                ruleServ.BindRule(Rule, OnRuleChanged);
                ruleServ.OnAddedRule -= OnNewRuleCreate;
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
