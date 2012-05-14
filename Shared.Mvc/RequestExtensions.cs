using System.Linq;
using System.Web;
using System.Web.Mvc;

public static class RequestExtensions
{
    public static string GetExistingReturnUrl(this HttpRequestBase request)
    {
        if (request.QueryString.AllKeys.Any(x => x == "returnUrl"))
            return request.QueryString["returnUrl"];

        return null;
    }

    public static string GetReturnUrl(this HttpRequestBase request)
    {
        if (request.QueryString.AllKeys.Any(x => x == "returnUrl"))
            return request.QueryString["returnUrl"];

        var retval = request.Path +
            (string.IsNullOrEmpty(request.QueryString.ToString()) == false ?
            "?" + request.QueryString : string.Empty);
        return retval;
    }

    public static bool IsAjaxRequest(this HttpContextBase context)
    {
        try
        {
            if (context == null)
                return false;
            if (context.Request == null)
                return false;
            return context.Request.IsAjaxRequest();
        }
        catch
        {
            return false;
        }
    }
}