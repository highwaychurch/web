using System;
using System.Web.Mvc;

namespace Highway.Identity.Web.GlobalFilter
{
    public class SslRedirectFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsSecureConnection)
            {
                filterContext.Result = new RedirectResult(
                    GetAbsoluteUri(filterContext.HttpContext.Request.Url).AbsoluteUri,
                    true);
            }
        }

        private Uri GetAbsoluteUri(Uri uriFromCaller)
        {
            var builder = new UriBuilder(Uri.UriSchemeHttps, uriFromCaller.Host);
            builder.Path = uriFromCaller.GetComponents(UriComponents.Path, UriFormat.Unescaped);

            var query = uriFromCaller.GetComponents(UriComponents.Query, UriFormat.UriEscaped);
            if (query.Length > 0)
            {
                var uriWithoutQuery = builder.Uri.AbsoluteUri;
                var absoluteUri = string.Format("{0}?{1}", uriWithoutQuery, query);
                return new Uri(absoluteUri, UriKind.Absolute);
            }
            else
            {
                return builder.Uri;
            }
        }
    }
}