namespace RealMethod
{
    public class DoOnce
    {
        private bool hasRun = false;

        /// <summary>
        /// Executes the given action only once.
        /// </summary>
        public void Run(System.Action action)
        {
            if (!hasRun)
            {
                hasRun = true;
                action?.Invoke();
            }
        }

        /// <summary>
        /// Resets the DoOnce so it can run again.
        /// </summary>
        public void Reset()
        {
            hasRun = false;
        }
    }

}