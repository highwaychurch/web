using System;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace Highway.Identity.Web.App.Auth
{
    public class OpenId
    {
        private OpenIdRelyingParty _openIdRelyingParty { get; set; }
        private IAuthenticationResponse _response { get; set; }

        public OpenId()
            : this(null)
        {

        }

        public OpenId(OpenIdRelyingParty relyingParty)
        {
            _openIdRelyingParty = relyingParty ?? new OpenIdRelyingParty();
        }

        public OpenIdentity ProcessOpenId(string openId)
        {
            _response = _openIdRelyingParty.GetResponse();

            if (_response == null)
            {
                Authenticate(openId);
                return null;
            }
         
              return Verify();

          
        }

        public void Authenticate(string openId)
        {
            Identifier id;
            if (Identifier.TryParse(openId, out id))
            {
                var request = _openIdRelyingParty.CreateRequest(id);
                request.AddExtension(
                    new ClaimsRequest
                        {
                            Email = DemandLevel.Require,
                            FullName = DemandLevel.Require,
                            TimeZone = DemandLevel.Require,
                            BirthDate = DemandLevel.Request
                        });
                request.RedirectingResponse.Send();
            }
            else
            {
                throw new ApplicationException("Invalid Identifier");
            }
        }

        public OpenIdentity Verify()
        {
            var oid = new OpenIdentity();

            switch (_response.Status)
            {
                case AuthenticationStatus.Authenticated:
                    oid.Id = _response.ClaimedIdentifier;
                    var claimsResponse = _response.GetExtension<ClaimsResponse>();
                    oid.Username = _response.FriendlyIdentifierForDisplay;
                    break;
                case AuthenticationStatus.Canceled:
                    throw new ApplicationException("Canceled at Provider");
                case AuthenticationStatus.Failed:
                    throw new ApplicationException(_response.Exception.Message);
            }

            return oid;
        }
    }
}