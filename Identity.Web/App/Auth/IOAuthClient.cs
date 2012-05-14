using System;

namespace Highway.Identity.Web.App.Auth
{
    interface IOauthClient
    {
        void Authenticate(Uri callback);
        OpenIdentity Verify();
    }
}
