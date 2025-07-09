using UnityEngine;
using System.Collections.Generic;

namespace RealMethod
{
    public sealed class TimeService : Service
    {
        private float CreateTime;
        private Dictionary<string, float> RecordTime = new Dictionary<string, float>();

        protected override void OnStart(object Author)
        {
            CreateTime = Time.time;
        }

        protected override void OnNewWorld()
        {
        }

        protected override void OnEnd(object Author)
        {
        }


        public void Record(string Tag)
        {
            if (RecordTime.ContainsKey(Tag))
                StopRecord(Tag);

            RecordTime.Add(Tag, Time.time);
        }
        public bool StopRecord(string Tag)
        {
            return RecordTime.Remove(Tag);
        }


        public float GetServiceTime()
        {
            return Time.time - CreateTime;
        }
        public float GetTime(string Tag)
        {
            if (RecordTime.ContainsKey(Tag))
            {
                return Time.time - RecordTime[Tag];
            }
            else
            {
                Debug.LogError($"Can't find any record for this: {Tag}");
                return 0;
            }
        }

    }

}
