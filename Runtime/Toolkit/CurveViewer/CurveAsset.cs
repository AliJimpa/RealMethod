using UnityEngine;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "CurveConfig", menuName = "RealMethod/Misc/CurveConfig", order = 1)]
    public class CurveConfig : ConfigAsset
    {
        [Header("Asset")]
        [SerializeField]
        private AnimationCurve _Curve;

        public AnimationCurve Curve => _Curve;
        public float Length => _Curve.length;
        public Keyframe[] Key => _Curve.keys;

        public float this[float Time]
        {
            get => _Curve.Evaluate(Time);
        }


        // Public Functions
        public float GetTime(float Value)
        {
            return FindTimeByValue(_Curve, Value);
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