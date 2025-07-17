using UnityEngine;

namespace RealMethod
{
    public sealed class TimeService : Service
    {
        private float serviceTime;
        private float worldTime;
        private Hictionary<float> RecordTime;

        // Service Methods
        protected override void OnStart(object Author)
        {
            serviceTime = Time.time;
            worldTime = Time.time;
            RecordTime = new Hictionary<float>(10);
        }
        protected override void OnNewWorld()
        {
            worldTime = Time.time;
        }
        protected override void OnEnd(object Author)
        {

        }

        // Public Functions
        public bool CheckRecord(string tag, float targettime)
        {
            return Time.time - RecordTime[tag] >= targettime;
        }
        public void StartRecord(string tag)
        {
            RecordTime.Add(tag, Time.time);
        }
        public void ResetRecord(string tag)
        {
            RecordTime[tag] = Time.time;
        }
        public bool RemoveRecord(string Tag)
        {
            return RecordTime.Remove(Tag);
        }
        public float GetServiceTime()
        {
            return Time.time - serviceTime;
        }
        public float GetWorldTime()
        {
            return Time.time - worldTime;
        }
        public bool TryGetTime(string tag, out float time)
        {
            if (RecordTime.TryGetValue(tag, out float startTime))
            {
                time = Time.time - startTime;
                return true;
            }

            time = -1f;
            return false;
        }
        public float GetTime(string tag)
        {
            return Time.time - RecordTime[tag];
        }
        public bool IsValidTime(string tag)
        {
            return RecordTime.ContainsKey(tag);
        }
    }

}
