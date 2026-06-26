using System;

namespace RealMethod
{
    public interface ITask
    {
        void Active(object Instigator);
        void Deactive(object Instigator);
    }

}