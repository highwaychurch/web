using System;
using System.Net;
using DotNetOpenAuth.OAuth2;

namespace Highway.Identity.Web.App.Auth
{
    public class Facebook : IOauthClient
    {
        public  void Authenticate(Uri callback)
        {
            var fb = new FacebookClient();

           fb.RequestUserAuthorization(returnTo:callback);
        }

        public  OpenIdentity Verify()
        {
            var fb = new FacebookClient();

            IAuthorizationState authorization = fb.ProcessUserAuthorization();
        
            var request = WebRequest.Create("https://graph.facebook.com/me?access_token=" + Uri.EscapeDataString(authorization.AccessToken));
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    var graph = FacebookGraph.Deserialize(responseStream);
                    return new OpenIdentity()
                               {Id = graph.Id.ToString(), Username = graph.Name, SiteProvider = "Facebook"};
                }
            }
        }
    }
}