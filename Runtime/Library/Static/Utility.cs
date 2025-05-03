using System;
using System.Linq.Expressions;
using UnityEngine;

namespace RealMethod
{
    static class Utility
    {
        public static string GetVariableName<T>(Expression<Func<T>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            throw new ArgumentException("Expression is not a valid member expression.");
        }
        public static Vector3 Multiply(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3 Multiply(this Vector3 a, float b)
        {
            return new Vector3(a.x * b, a.y * b, a.z * b);
        }

        public static Hash128 Vector2ConvertToHash(Vector2Int Target)
        {
            return Hash128.Compute(Target.x + "." + Target.y);
        }

        public static Hash128 Vector3ConvertToHash(Vector3 Target)
        {
            return Hash128.Compute(Target.x + "." + Target.y + "." + Target.z);
        }

        public static Hash128 Vector2ConvertToHash(Vector2 Target)
        {
            return Hash128.Compute(Target.x + "." + Target.y);
        }

        public static Hash128 Vector3ConvertToHash(Vector3Int Target)
        {
            return Hash128.Compute(Target.x + "." + Target.y + "." + Target.z);
        }

        public static Vector3 Vector2ToVector3(Vector2 Target)
        {
            return new Vector3(Target.x, 0, Target.y);
        }

        public static Vector2 Vector3ToVector2(Vector3 Target)
        {
            return new Vector2(Target.x, Target.z);
        }

        public static Vector2Int Vector3ToVector2Int(Vector3 Target)
        {
            return new Vector2Int((int)Target.x, (int)Target.z);
        }

        public static bool NullChecker(UnityEngine.Object Obj, string Name)
        {
            if (Obj)
            {
                return true;
            }
            else
            {
                Debug.LogError($"Target Object [{Name}] Is Not Valid");
                return false;
            }
        }
    }
}