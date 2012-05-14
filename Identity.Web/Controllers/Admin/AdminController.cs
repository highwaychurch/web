/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using Highway.Identity.Core;
using Highway.Identity.Core.Repositories;
using Highway.Identity.Core.TokenService;
using Highway.Identity.Web.Security;
using Highway.Identity.Web.ViewModels.Administration;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.SecurityTokenService;
using Microsoft.IdentityModel.Tokens;

namespace Highway.Identity.Web.Controllers.Admin
{
    [ClaimsAuthorize(Constants.Actions.Administration, Constants.Resources.Configuration)]
    public class AdminController : Controller
    {
        readonly IConfigurationRepository _configurationRepository;
        readonly IUserRepository _userRepository;

        public AdminController(IUserRepository userRepository, IConfigurationRepository configurationRepository)
        {
            _userRepository = userRepository;
            _configurationRepository = configurationRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyClaims()
        {
            var myModel = new MyModel
            {
                Claims = GetClaims()
            };

            return View(myModel);
        }

        public ActionResult MyToken()
        {
            var config = _configurationRepository.Configuration;
            var samlHandler = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection()[config.DefaultTokenType];
            
            var descriptor = new SecurityTokenDescriptor
            {
                AppliesToAddress = "http://self",
                Lifetime = new Lifetime(DateTime.UtcNow, DateTime.UtcNow.AddHours(config.DefaultTokenLifetime)),
                SigningCredentials = new X509SigningCredentials(_configurationRepository.SigningCertificate.Certificate),
                TokenIssuerName = config.IssuerUri,
                Subject = new ClaimsIdentity(GetClaims())
            };

            var token = samlHandler.CreateToken(descriptor);

            var sb = new StringBuilder(1024);
            samlHandler.WriteToken(XmlWriter.Create(new StringWriter(sb)), token);

            return new ContentResult
            {
                ContentType = "text/xml",
                Content = sb.ToString()
            };
        }

        public ActionResult Configuration()
        {
            return View(_configurationRepository.Configuration.ToViewModel());
        }

        [HttpPost]
        public ActionResult Configuration(GlobalConfigurationViewModel configuration)
        {
            if (ModelState.IsValid)
            {
                _configurationRepository.UpdateConfiguration(configuration.ToDomainModel());
                return RedirectToAction("Index");
            }

            return View();
        }

        public ActionResult Endpoints()
        {
            return View(_configurationRepository.Endpoints.ToViewModel());
        }

        [HttpPost]
        public ActionResult Endpoints(EndpointsConfigurationViewModel endpoints)
        {
            if (ModelState.IsValid)
            {
                _configurationRepository.UpdateEndpoints(endpoints.ToDomainModel());
                return RedirectToAction("Index");
            }

            return View();
        }

        [ClaimsAuthorize(Constants.Actions.Administration, Constants.Resources.ServiceCertificates)]
        public ActionResult Certificates()
        {
            var model = new EditCertificatesModel
            {
                AvailableCertificates = GetAvailableCertificatesFromStore(),
                
                SigningCertificate = _configurationRepository.SigningCertificate.SubjectDistinguishedName,
                SslCertificate = _configurationRepository.SslCertificate.SubjectDistinguishedName
            };

            return View(model);
        }

        [HttpPost]
        [ClaimsAuthorize(Constants.Actions.Administration, Constants.Resources.ServiceCertificates)]
        public ActionResult Certificates(EditCertificatesModel model)
        {
            string newSsl = null, newSigning = null;

            if (model.UpdateSslCertificate)
            {
                newSsl = model.UpdatedSslCertificate;
            }
            if (model.UpdateSigningCertificate)
            {
                newSigning = model.UpdatedSigningCertificate;
            }

            _configurationRepository.UpdateCertificates(newSsl, newSigning);

            return RedirectToAction("Index");
        }

        #region Helper
        private List<Claim> GetClaims()
        {
            return TokenService.GetOutputClaims(HttpContext.User as IClaimsPrincipal, null, _userRepository);
        }

        private List<string> GetAvailableCertificatesFromStore()
        {
            var list = new List<string>();
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            try
            {
                foreach (var cert in store.Certificates)
                {
                    list.Add(cert.Subject);
                }
            }
            finally
            {
                store.Close();
            }

            return list;
        }
        #endregion
    }
}
