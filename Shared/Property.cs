using System;
using System.Linq;
using System.Linq.Expressions;

namespace Highway.Shared
{
    public static class Property
    {
        /// <summary>
        /// Returns the string name of a property, this allows type / member safety
        /// </summary>
        /// <param name="propertyNameLambda">() => obj.Name</param>
        /// <returns>Name</returns>
        public static string GetFor(Expression<Func<object>> propertyNameLambda)
        {
            MemberExpression member;
            if(propertyNameLambda.Body is UnaryExpression)
                member = ((UnaryExpression)propertyNameLambda.Body).Operand as MemberExpression;
            else
                member = propertyNameLambda.Body as MemberExpression;
            if(member != null)
            {
                return member.Member.Name;
            }
            throw new ArgumentException("Could not determine property name.", "propertyNameLambda");
        }

        /// <summary>
        /// Returns the string names of properties, this allows type / member safety
        /// </summary>
        /// <param name="propertyNameLambda">() => obj.Name1, () => obj.Name2</param>
        /// <returns>[Name1, Name2]</returns>
        public static string[] GetFor(params Expression<Func<object>>[] propertyNameLambda)
        {
            return propertyNameLambda
                .Select(property => property.Body)
                .OfType<MemberExpression>()
                .Select(member => member.Member.Name)
                .Concat(propertyNameLambda
                    .Select(property => property.Body)
                    .OfType<UnaryExpression>()
                    .Select(member => member.Operand)
                    .OfType<MemberExpression>()
                    .Select(member => member.Member.Name))
                .ToArray();
        }

        /// <summary>
        /// Returns the string name of a property, this allows type / member safety
        /// </summary>
        /// <typeparam name="T">Type of objects whos properties to evaluate</typeparam>
        /// <param name="propertyNameLambda">obj => obj.Name</param>
        /// <returns>Name</returns>
        public static string For<T>(Expression<Func<T, object>> propertyNameLambda)
        {
            MemberExpression member;
            if(propertyNameLambda.Body is UnaryExpression)
                member = ((UnaryExpression)propertyNameLambda.Body).Operand as MemberExpression;
            else
                member = propertyNameLambda.Body as MemberExpression;
            if(member != null)
            {
                return member.Member.Name;
            }
            throw new ArgumentException("Could not determine property name.", "propertyNameLambda");
        }

        /// <summary>
        /// Returns the string name of a property, this allows type / member safety
        /// </summary>
        /// <remarks> 
        /// Adding the TRetVal parameter allows more efficient Lambda expressions (doesn't create the Convert(p => p.ID) function)
        /// also helps in Generic Type inference
        /// </remarks>
        public static string For<T, TRetVal>(Expression<Func<T, TRetVal>> propertyNameLambda)
        {
            MemberExpression member;
            if(propertyNameLambda.Body is UnaryExpression)
                member = ((UnaryExpression)propertyNameLambda.Body).Operand as MemberExpression;
            else
                member = propertyNameLambda.Body as MemberExpression;
            if(member != null)
            {
                return member.Member.Name;
            }
            throw new ArgumentException("Could not determine property name.", "propertyNameLambda");
        }
    }
}
