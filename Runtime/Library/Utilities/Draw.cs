using UnityEngine;

namespace RealMethod
{
    static class Draw
    {
        static void DrawCurve(AnimationCurve curve)
        {
            for (float t = 0; t < curve.keys[curve.length - 1].time; t += 0.1f)
            {
                Gizmos.DrawSphere(new Vector3(t, curve.Evaluate(t), 0), 0.1f);
            }
        }
    }
}