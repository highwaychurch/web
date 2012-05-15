using System;
using System.Web.Mvc;

namespace F1OAuthTest.Integration.FellowshipOne
{
    public static class UrlHelperExtensions
    {
        public static string ToPublicUrl(this UrlHelper urlHelper, Uri relativeUri)
        {
            var httpContext = urlHelper.RequestContext.HttpContext;

            var uriBuilder = new UriBuilder
                                 {
                                     Host = httpContext.Request.Url.Host,
                                     Path = "/",
                                     Port = 80,
                                     Scheme = "http",
                                 };

            if (httpContext.Request.IsLocal)
            {
                uriBuilder.Path = "/F1OAuthTest/";
                uriBuilder.Port = httpContext.Request.Url.Port;
            }

            return new Uri(uriBuilder.Uri, relativeUri).AbsoluteUri;
        }
    }
}