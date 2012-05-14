using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Highway.Shared.Mvc.Validation
{
    public static class ValidationExtensions
    {
        /// <summary>
        /// Adds the specified error message to the errors collection for the model-state dictionary that is associated with the specified key.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="keyLambda">The key lambda. () => PropertyName</param>
        /// <param name="errorMessage">The error message.</param>
        public static void AddModelError(this ModelStateDictionary dictionary, Expression<Func<object>> keyLambda,
                                         string errorMessage)
        {
            dictionary.AddModelError(Property.GetFor(keyLambda), errorMessage);
        }
    }
}