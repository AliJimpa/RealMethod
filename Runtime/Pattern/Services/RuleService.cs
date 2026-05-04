using System;

namespace RealMethod
{
    public abstract class RuleService : GameService
    {
        public Action<string> OnAddedRule;
        public Action<string> OnFinishRule;
        private NameTable<Observer<bool>> Rules;

        public RuleService()
        {
            Rules = new NameTable<Observer<bool>>(5);
        }


        protected override void OnWorldChanged()
        {
            foreach (var item in Rules)
            {
                item.Value.Check();
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
            foreach (var item in Rules)
            {
                item.Value.Check();
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