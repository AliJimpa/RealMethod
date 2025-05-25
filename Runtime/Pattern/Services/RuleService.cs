using System;

namespace RealMethod
{
    public abstract class RuleService : Service
    {
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
        }
        public void RemoveRule(string rule)
        {
            Rules.RemoveItem(rule);
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


        public void BindRule(string Name, Action<Observer> callback)
        {
            Rules[Name].Bind(callback);
        }
        
        protected abstract void AnyRuleChanged(Observer obs);

    }

}