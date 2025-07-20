using System.Collections;

namespace RealMethod
{
    public class CoroutineSequencer
    {
        public bool IsDone { get; private set; }

        public IEnumerator Run(IEnumerator coroutine)
        {
            yield return coroutine;
            IsDone = true;
        }
    }
}