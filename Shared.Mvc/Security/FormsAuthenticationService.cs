using System;
using System.Security.Principal;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;

namespace Highway.Shared.Mvc.Security
{
    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public bool IsAuthenticated
        {
            get { return HttpContext.Current.User.Identity.IsAuthenticated; }
        }

        public void SignIn(string userName, bool createPersistentCookie)
        {
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        public string GetRedirectUrl(string userName, bool createPersistentCookie)
        {
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
            return FormsAuthentication.GetRedirectUrl(userName, createPersistentCookie);
        }
    }

    public static class FormsAuthenticationHelper
    {
        public static void SetAuthCookie(string username, string authenticationMethod, bool isPersistent)
        {
            HttpContext context = HttpContext.Current;
            if (!context.Request.IsSecureConnection && FormsAuthentication.RequireSSL)
            {
                throw new HttpException("SSL required");
            }

            // create forms authentication ticket
            FormsAuthenticationTicket ticket = CreateTicket(username, authenticationMethod, isPersistent);

            // turn into protected cookie
            HttpCookie cookie = CreateCookie(ticket, isPersistent);

            // set cookie
            context.Response.Cookies.Add(cookie);
        }

        public static string GetAuthenticationMethod(IIdentity identity)
        {
            var id = identity as FormsIdentity;

            if (id != null)
            {
                var userData = id.Ticket.UserData;

                if (userData.StartsWith("AuthenticationMethod:", StringComparison.OrdinalIgnoreCase))
                {
                    var method = userData.Substring(userData.IndexOf(":") + 1);
                    if (!string.IsNullOrEmpty(method))
                    {
                        return method;
                    }
                }
            }

            throw new InvalidOperationException("An attempt was made to use an incorrectly formatted forms authentication token.");
        }

        static HttpCookie CreateCookie(FormsAuthenticationTicket ticket, bool isPersistent)
        {
            string protectedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, protectedTicket)
                             {
                                 HttpOnly = true,
                                 Secure = FormsAuthentication.RequireSSL,
                             };

            if (isPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }

            return cookie;
        }

        static FormsAuthenticationTicket CreateTicket(string username, string authenticationMethod,
                                                      bool isPersistent)
        {
            var section = WebConfigurationManager.GetSection("system.web/authentication") as AuthenticationSection;
            var userData = GetUserData(authenticationMethod);

            var ticket = new FormsAuthenticationTicket(
                1,
                username,
                DateTime.Now,
                DateTime.Now.Add(section.Forms.Timeout),
                isPersistent,
                userData);

            return ticket;
        }

        static string GetUserData(string authenticationMethod)
        {
            return string.Format("{0}:{1}", "AuthenticationMethod", authenticationMethod);
        }
    }
}