using System;
using System.Web.Mvc;
using Highway.Shared.Diagnostics;

namespace Highway.Shared.Mvc.Diagnostics
{
    public class RequestLoggingFilter : IActionFilter
    {
        readonly ILog<RequestLoggingFilter> _log;

        public RequestLoggingFilter(ILog<RequestLoggingFilter> log)
        {
            if (log == null) throw new ArgumentNullException("log");
            _log = log;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _log.Debug(string.Format("Invoking action {0}.{1} for URL '{2}'...",
                                     filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                                     filterContext.ActionDescriptor.ActionName,
                                     filterContext.HttpContext.Request.RawUrl));
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _log.Debug("Action invocation complete.");
        }
    }
}