using UnityEngine;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "CurveAsset", menuName = "RealMethod/Misc/CurveAsset", order = 1)]
    public class CurveAsset : ScriptableObject
    {
        [Header("Asset")]
        [SerializeField]
        private AnimationCurve Curve;


        public float GetValue(float Time)
        {
            return Curve.Evaluate(Time);
        }

        public float GetTime(float Value)
        {
            return FindTimeByValue(Curve, Value);
        }

        public float Getlength()
        {
            return Curve.length;
        }

        public AnimationCurve GetCurve()
        {
            return Curve;
        }



        /// <summary>
        /// Finds the time corresponding to a given value in the AnimationCurve.
        /// </summary>
        /// <param name="curve">The AnimationCurve to search.</param>
        /// <param name="value">The target value.</param>
        /// <returns>The time corresponding to the value, or -1 if not found.</returns>
        private float FindTimeByValue(AnimationCurve curve, float value)
        {
            for (float t = curve.keys[0].time; t <= curve.keys[curve.length - 1].time; t += 0.01f)
            {
                if (Mathf.Approximately(curve.Evaluate(t), value))
                {
                    return t;
                }
            }

            Debug.LogWarning("Value not found in curve within a reasonable precision.");
            return -1f;
        }



    }

}