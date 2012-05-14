using System;
using System.Web;
using System.Web.Mvc;

namespace Highway.Shared.Mvc
{
    /// <summary>
    /// Specifies that this response should not be cached by the client.
    /// </summary>
    /// <remarks>
    /// There are known issues with IE browsers caching 'get' ajax requests. 
    /// This attribute can be used to prevent caching where needed.
    /// </remarks>
    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var cache = filterContext.HttpContext.Response.Cache;
            cache.SetAllowResponseInBrowserHistory(false);
            cache.SetCacheability(HttpCacheability.NoCache);
            cache.SetExpires(DateTime.Now.AddSeconds(-1));
            cache.SetNoStore();
        }
    }
}