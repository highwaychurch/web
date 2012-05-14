using System;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Highway.Shared.Persistence;

namespace Highway.Shared.Mvc.Persistence
{
    public class TransactedActionFilter : IActionFilter
    {
        readonly ITransactionContext _transactionContext;

        public TransactedActionFilter(ITransactionContext transactionContext)
        {
            if (transactionContext == null) throw new ArgumentNullException("transactionContext");
            _transactionContext = transactionContext;
        }

        bool NoTransaction(ActionDescriptor descriptor)
        {
            return descriptor.GetCustomAttributes(true).OfType<NoTransactionAttribute>().Any();
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (NoTransaction(filterContext.ActionDescriptor))
            {
                return;
            }

            _transactionContext.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (NoTransaction(filterContext.ActionDescriptor))
            {
                return;
            }
            
            if (filterContext.Exception != null)
                _transactionContext.SetAbort();

            _transactionContext.EndTransaction();
        }
    }
}