using System.Web.Mvc;

namespace Highway.Shared.Mvc.ActionResults
{
    public class RedirectParentFrameToActionResult : ActionResult
    {
        readonly string _actionName;
        readonly string _controllerName;
        readonly object _routeValues;

        public RedirectParentFrameToActionResult(string actionName, string controllerName, object routeValues = null)
        {
            _actionName = actionName;
            _controllerName = controllerName;
            _routeValues = routeValues;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var helper = new UrlHelper(context.RequestContext);
            var url = helper.Action(_actionName, _controllerName, _routeValues);

            context.Controller.TempData.Keep();

            context.HttpContext.Response.Write(
                string.Format("<html><head></head><body><script type='text/javascript'>window.top.location.href = '{0}';</script></body></html>", 
                url
                ));
        }
    }
}