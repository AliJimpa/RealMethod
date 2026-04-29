using UnityEngine;

namespace RealMethod
{
    public sealed class TimeService : Service
    {
        private float serviceTime;
        private float worldTime;
        private NameTable<float> RecordTime;

        // Service Methods
        public override void OnRegister(object Author)
        {
            serviceTime = Time.time;
            worldTime = Time.time;
            RecordTime = new NameTable<float>(10);
        }
        public override void OnWorldChanging(World Previous, World New)
        {
            worldTime = Time.time;
        }
        public override void OnUnregister(object Author)
        {
            RecordTime.Clear();
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
