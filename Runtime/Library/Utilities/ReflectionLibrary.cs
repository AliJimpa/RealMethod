using System;
using System.Linq.Expressions;

namespace RealMethod
{
    public static class RM_Reflection
    {
        /// <summary>
        /// Extracts the variable or property name from a lambda expression.
        /// </summary>
        /// <typeparam name="T">The type of the variable or property.</typeparam>
        /// <param name="expression">
        /// A lambda expression pointing to a variable or property
        /// (e.g. <c>() => myVariable</c> or <c>() => myObject.MyProperty</c>).
        /// </param>
        /// <returns>
        /// The name of the variable or property represented by the expression.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the expression does not represent a valid variable or property access.
        /// </exception>
        /// <remarks>
        /// This method is commonly used to avoid hard-coded string names,
        /// especially for logging, debugging, and change-notification systems.
        /// </remarks>F
        public static string GetVariableName<T>(Expression<Func<T>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            throw new ArgumentException("Expression is not a valid member expression.");
        }
    }
}