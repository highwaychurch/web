using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using Highway.Identity.Core.Repositories;
using Highway.Identity.Web.Security;
using Highway.Identity.Web.ViewModels;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Web;

namespace Highway.Identity.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        readonly IConfigurationRepository _configurationRepository;
        readonly IUserRepository _userRepository;
        readonly AuthenticationHelper _authenticationHelper;

        public AuthenticationController(
            IConfigurationRepository configurationRepository,
            IUserRepository userRepository,
            AuthenticationHelper authenticationHelper)
        {
            _configurationRepository = configurationRepository;
            _userRepository = userRepository;
            _authenticationHelper = authenticationHelper;
        }

        public ActionResult SignIn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ShowClientCertificateLink = _configurationRepository.Configuration.EnableClientCertificates;

            return View();
        }

        [HttpPost]
        public ActionResult SignIn(SignInModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (_userRepository.ValidateUser(model.UserName, model.Password))
                {
                    return SetPrincipalAndRedirect(model.UserName, AuthenticationMethods.Password, returnUrl, model.EnableSSO, _configurationRepository.Configuration.SsoCookieLifetime);
                }
            }

            ModelState.AddModelError("", "Incorrect credentials or no authorization.");

            ViewBag.ShowClientCertificateLink = _configurationRepository.Configuration.EnableClientCertificates;
            return View(model);
        }

        public ActionResult CertificateSignIn(string returnUrl)
        {
            if (!_configurationRepository.Configuration.EnableClientCertificates)
            {
                return new HttpNotFoundResult();
            }

            var clientCert = HttpContext.Request.ClientCertificate;

            if (clientCert != null && clientCert.IsPresent && clientCert.IsValid)
            {
                string userName;
                if (_userRepository.ValidateUser(new X509Certificate2(clientCert.Certificate), out userName))
                {
                    return SetPrincipalAndRedirect(userName, AuthenticationMethods.X509, returnUrl, false, _configurationRepository.Configuration.SsoCookieLifetime);
                }
            }

            throw new NotImplementedException();
        }

        public ActionResult SignOut()
        {
            if (Request.IsAuthenticated)
            {
                FederatedAuthentication.SessionAuthenticationModule.DeleteSessionTokenCookie();
            }

            return RedirectToAction("Index", "Home");
        }

        #region Private
        private ActionResult SetPrincipalAndRedirect(string userName, string authenticationMethod, string returnUrl, bool isPersistent, int ttl)
        {
            _authenticationHelper.SetSessionToken(userName, authenticationMethod, isPersistent, ttl, HttpContext.Request.Url.AbsoluteUri);

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = HttpUtility.UrlDecode(returnUrl);
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
            }

            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}
