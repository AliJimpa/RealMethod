using System;

namespace RealMethod.Editor
{
    public class ServiceRule : CompileRule
    {
        // CompileRule Methods
        protected override void Initilized()
        {
            
        }
        public override RuleExecutionMode GetRuleMode()
        {
            return RuleExecutionMode.AfterCompilation;
        }
        public override Type GetBaseType()
        {
            return typeof(Service);
        }
        public override void OnCheck(Type type)
        {
            throw new NotImplementedException();
        }


    }
}