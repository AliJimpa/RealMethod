using UnityEngine;


namespace RealMethod
{
    public static class RM_Math
    {
        public static Vector3 TwoD => new Vector3(1, 0, 1);

        public class Map
        {
            /// <summary>
            /// Maps a float value from one range to another and clamps the result to the output range.
            /// </summary>
            /// <param name="value">The input value to map.</param>
            /// <param name="inMin">Minimum of the input range.</param>
            /// <param name="inMax">Maximum of the input range.</param>
            /// <param name="outMin">Minimum of the output range.</param>
            /// <param name="outMax">Maximum of the output range.</param>
            /// <returns>Mapped and clamped float value.</returns>
            public static float RemapClamped(float value, float inMin, float inMax, float outMin, float outMax)
            {
                // Prevent divide by zero
                if (Mathf.Approximately(inMax, inMin))
                {
                    Debug.LogWarning("Input range is zero. Returning outMin.");
                    return outMin;
                }

                // Normalize the input value to 0–1 within the input range
                float t = (value - inMin) / (inMax - inMin);

                // Scale and offset to target range
                float mappedValue = t * (outMax - outMin) + outMin;

                // Clamp result to the output range
                return Mathf.Clamp(mappedValue, Mathf.Min(outMin, outMax), Mathf.Max(outMin, outMax));
            }
            /// <summary>
            /// Determines whether a value is within a specified range, with control over boundary inclusivity.
            /// </summary>
            /// <param name="value">The value to check.</param>
            /// <param name="min">The lower bound of the range.</param>
            /// <param name="max">The upper bound of the range.</param>
            /// <param name="inclusiveMin">Whether the lower bound is inclusive (>=).</param>
            /// <param name="inclusiveMax">Whether the upper bound is inclusive (&lt;=).</param>
            /// <returns>True if the value is within the range; otherwise, false.</returns>
            public static bool IsInRange(float value, float min, float max, bool inclusiveMin = true, bool inclusiveMax = true)
            {
                bool isAboveMin = inclusiveMin ? value >= min : value > min;
                bool isBelowMax = inclusiveMax ? value <= max : value < max;

                return isAboveMin && isBelowMax;
            }
        }

        public class Interpolate
        {
            /// <summary>
            /// Interpolates a value from Current to Target, applying a given speed over DeltaTime.
            /// Mimics Unreal Engine's FInterpTo function.
            /// </summary>
            /// <param name="current">The current value.</param>
            /// <param name="target">The target value.</param>
            /// <param name="deltaTime">The time step to apply the interpolation.</param>
            /// <param name="interpSpeed">The interpolation speed.</param>
            /// <returns>The interpolated value.</returns>
            public static float FInterpTo(float current, float target, float deltaTime, float interpSpeed)
            {
                if (interpSpeed <= 0f || Mathf.Approximately(current, target))
                {
                    return target; // If speed is zero or already at the target, no interpolation needed
                }

                float delta = target - current; // Difference between current and target
                float step = delta * Mathf.Clamp01(interpSpeed * deltaTime); // Calculate interpolation step

                return current + step; // Move the current value closer to the target
            }
            /// <summary>
            /// Interpolates a value from Current to Target at a constant speed over DeltaTime.
            /// Mimics Unreal Engine's FInterpToConstant function.
            /// </summary>
            /// <param name="current">The current value.</param>
            /// <param name="target">The target value.</param>
            /// <param name="deltaTime">The time step to apply the interpolation.</param>
            /// <param name="interpSpeed">The constant speed of interpolation.</param>
            /// <returns>The interpolated value.</returns>
            public static float FInterpToConstant(float current, float target, float deltaTime, float interpSpeed)
            {
                if (interpSpeed <= 0f || Mathf.Approximately(current, target))
                {
                    return target; // If speed is zero or already at the target, no interpolation needed
                }

                // Calculate the step size for this frame
                float step = interpSpeed * deltaTime;

                // Move towards the target by the step size, clamped to not overshoot
                if (Mathf.Abs(target - current) <= step)
                {
                    return target; // If within one step, snap to target
                }

                // Move towards the target
                return current + Mathf.Sign(target - current) * step;
            }
            public static Vector3 VInterpTo(Vector3 current, Vector3 target, float deltaTime, float interpSpeed)
            {
                if (interpSpeed <= 0f || current == target)
                {
                    return target; // If speed is zero or already at the target, no interpolation needed
                }

                // Interpolate towards the target
                return Vector3.Lerp(current, target, Mathf.Clamp01(interpSpeed * deltaTime));
            }
            public static Vector3 VInterpToConstant(Vector3 current, Vector3 target, float deltaTime, float interpSpeed)
            {
                if (interpSpeed <= 0f || current == target)
                {
                    return target; // If speed is zero or already at the target, no interpolation needed
                }

                // Calculate the step size
                Vector3 direction = (target - current).normalized;
                float distance = Vector3.Distance(current, target);
                float step = interpSpeed * deltaTime;

                // Move towards the target by the step size, clamped to not overshoot
                return distance <= step ? target : current + direction * step;
            }
            public static Quaternion RInterpTo(Quaternion current, Quaternion target, float deltaTime, float interpSpeed)
            {
                if (interpSpeed <= 0f || current == target)
                {
                    return target; // If speed is zero or already at the target, no interpolation needed
                }

                // Interpolate towards the target
                return Quaternion.Slerp(current, target, Mathf.Clamp01(interpSpeed * deltaTime));
            }
            public static Quaternion RInterpToConstant(Quaternion current, Quaternion target, float deltaTime, float interpSpeed)
            {
                if (interpSpeed <= 0f || current == target)
                {
                    return target; // If speed is zero or already at the target, no interpolation needed
                }

                // Calculate the step size
                float angle = Quaternion.Angle(current, target);
                float step = interpSpeed * deltaTime;

                // Rotate towards the target by the step size, clamped to not overshoot
                return angle <= step ? target : Quaternion.RotateTowards(current, target, step);
            }

        }

    }

}
