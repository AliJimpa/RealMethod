using UnityEngine;

namespace RealMethod
{
    public static class RM_Physics
    {
        public class Trace
        {
            public static bool Line(Vector3 Start, Vector3 Direction, float Length, LayerMask Layer, QueryTriggerInteraction TriggerInteraction, Color RayColor, Color HitColor, out RaycastHit HitResult)
            {
                bool Result;
                Result = Physics.Raycast(Start, Direction, out HitResult, Length, Layer, TriggerInteraction);
                float debuglength = HitResult.collider ? HitResult.distance : Length;
                Color debugcolor = HitResult.collider ? HitColor : RayColor;
                Debug.DrawRay(Start, Direction * debuglength, debugcolor);
                return Result;
            }

            public static bool Line(Vector3 Start, Vector3 Direction, float Length, LayerMask Layer, QueryTriggerInteraction TriggerInteraction, out RaycastHit HitResult)
            {
                bool Result;
                Result = Physics.Raycast(Start, Direction, out HitResult, Length, Layer, TriggerInteraction);
                float debuglength = HitResult.collider ? HitResult.distance : Length;
                Color debugcolor = HitResult.collider ? Color.green : Color.red;
                Debug.DrawRay(Start, Direction * debuglength, debugcolor);
                return Result;
            }

            public static bool Line(Vector3 Start, Vector3 Direction, float Length, LayerMask Layer, out RaycastHit HitResult)
            {
                bool Result;
                Result = Physics.Raycast(Start, Direction, out HitResult, Length, Layer);
                float debuglength = HitResult.collider ? HitResult.distance : Length;
                Color debugcolor = HitResult.collider ? Color.green : Color.red;
                Debug.DrawRay(Start, Direction * debuglength, debugcolor);
                return Result;
            }

            public static bool Line(Vector3 Start, Vector3 Direction, float Length, out RaycastHit HitResult)
            {
                bool Result;
                Result = Physics.Raycast(Start, Direction, out HitResult, Length);
                float debuglength = HitResult.collider ? HitResult.distance : Length;
                Color debugcolor = HitResult.collider ? Color.green : Color.red;
                Debug.DrawRay(Start, Direction * debuglength, debugcolor);
                return Result;
            }
        }

    }
}