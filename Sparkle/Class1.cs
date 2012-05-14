using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Configuration;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.SecurityTokenService;

namespace Sparkle
{
    public class SparkleTokenServiceConfiguration : SecurityTokenServiceConfiguration
    {

        public override SecurityTokenService CreateSecurityTokenService()
        {
            return base.CreateSecurityTokenService();
        }
    }

    public class SparkleTokenService : SecurityTokenService
    {
        public SparkleTokenService(SecurityTokenServiceConfiguration securityTokenServiceConfiguration) : base(securityTokenServiceConfiguration)
        {
        }

        protected override Scope GetScope(IClaimsPrincipal principal, RequestSecurityToken request)
        {
            throw new NotImplementedException();
        }

        protected override IClaimsIdentity GetOutputClaimsIdentity(IClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        {
            throw new NotImplementedException();
        }
    }
}
