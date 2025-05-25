using System;

namespace RealMethod
{
    public abstract class RuleService : Service
    {
        public Action<string> OnAddedRule;
        public Action<string> OnFinishRule;
        private HashedKeyItem<Observer<bool>> Rules;

        public RuleService()
        {
            Rules = new HashedKeyItem<Observer<bool>>(5);
        }

        public override void WorldUpdated()
        {
            foreach (var item in Rules.List)
            {
                item.Value.Check();
            }
        }

        public void NewRule(string rule, Func<bool> conditional)
        {
            Rules.AddItem(rule, new Observer<bool>(conditional, AnyRuleChanged));
            OnAddedRule?.Invoke(rule);
        }
        public void RemoveRule(string rule)
        {
            Rules.RemoveItem(rule);
            OnFinishRule?.Invoke(rule);
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
            return Rules.TryGetItem(rule, out result);
        }
        public void BindRule(string Name, Action<Observer> callback)
        {
            Rules[Name].Bind(callback);
        }

        protected abstract void AnyRuleChanged(Observer obs);

    }

}