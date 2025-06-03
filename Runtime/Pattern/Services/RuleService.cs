using System;

namespace RealMethod
{
    public abstract class RuleService : Service
    {
        public Action<string> OnAddedRule;
        public Action<string> OnFinishRule;
        private Hictionary<Observer<bool>> Rules;

        public RuleService()
        {
            Rules = new Hictionary<Observer<bool>>(5);
        }

        public override void WorldUpdated()
        {
            foreach (var item in Rules.GetValues())
            {
                item.Check();
            }
        }

        public void AddRule(string rule, Func<bool> conditional)
        {
            Rules.Add(rule, new Observer<bool>(conditional, AnyRuleChanged));
            OnAddedRule?.Invoke(rule);
        }
        public void RemoveRule(string rule)
        {
            Rules.Remove(rule);
            OnFinishRule?.Invoke(rule);
        }
        public void Check(string rule)
        {
            Rules[rule].Check();
        }
        public void UpdateRules()
        {
            foreach (Observer<bool> item in Rules.GetValues())
            {
                item.Check();
            }
        }
        public bool InEffect(string rule)
        {
            Rules[rule].Check();
            return Rules[rule].Value;
        }
        public bool InEffect(string[] rules)
        {
            foreach (var rule in rules)
            {
                if (InEffect(rule))
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        public bool IsValid(string rule)
        {
            Observer<bool> result;
            return Rules.TryGetValue(rule, out result);
        }
        public void BindRule(string Name, Action<Observer> callback)
        {
            Rules[Name].Bind(callback);
        }
        public void UnbindRule(string Name, Action<Observer> callback)
        {
            Rules[Name].Unbind(callback);
        }

        protected abstract void AnyRuleChanged(Observer obs);

    }

}