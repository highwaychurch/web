using System;
using System.Web;
using System.Web.Mvc;

namespace Highway.Shared.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string Absolute(this UrlHelper url, string relativeOrAbsolute)
        {
            var uri = new Uri(relativeOrAbsolute, UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri)
            {
                return relativeOrAbsolute;
            }
            // At this point, we know the url is relative.
            return VirtualPathUtility.ToAbsolute(relativeOrAbsolute);
        }
    }
}
