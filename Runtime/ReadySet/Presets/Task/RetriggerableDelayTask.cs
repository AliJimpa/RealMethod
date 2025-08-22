

using RealMethod;
using UnityEngine;

namespace RealMethodF
{
    public class RetriggerableDelay : ITask
    {
        private float _delay;
        private System.Action _onElapsed;
        private float _endTime;
        private bool _running;
        private Object MyAuthor;

        public RetriggerableDelay(float delay, System.Action onElapsed)
        {
            _delay = delay;
            _onElapsed = onElapsed;
        }

        // Implement ITask Interface
        void ITask.Enable(Object author)
        {
            MyAuthor = author;
            _endTime = Time.time + _delay;
            _running = true;
        }
        void ITask.Disable(Object author)
        {
            MyAuthor = null;
        }
        // Implement ITick Interface
        void ITick.Tick(float deltaTime)
        {
            if (_running && Time.time >= _endTime)
            {
                _running = false;
                _onElapsed?.Invoke();
                if (MyAuthor != null)
                    Despawn.Task(this, MyAuthor);
            }
        }


        /// <summary>
        /// Restart the delay countdown.
        /// </summary>
        public void Trigger()
        {
            _endTime = Time.time + _delay;
            _running = true;
        }
        /// <summary>Cancel without firing.</summary>
        public void Cancel() => _running = false;



    }


}