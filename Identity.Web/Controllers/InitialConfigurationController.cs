using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using Highway.Identity.Core.Repositories;
using Highway.Identity.Web.ViewModels;

namespace Highway.Identity.Web.Controllers
{
    public class InitialConfigurationController : Controller
    {
        readonly IConfigurationRepository _configurationRepository;

        public InitialConfigurationController(IConfigurationRepository configuration)
        {
            _configurationRepository = configuration;
        }

        public ActionResult Index()
        {
            if (!string.IsNullOrWhiteSpace(_configurationRepository.SigningCertificate.SubjectDistinguishedName))
            {
                return RedirectToAction("index", "home");
            }

            var model = new InitialConfigurationModel
            {
                AvailableCertificates = GetAvailableCertificatesFromStore(),
                IssuerUri = _configurationRepository.Configuration.IssuerUri,
                SiteName = _configurationRepository.Configuration.SiteName
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(InitialConfigurationModel model)
        {
            if (ModelState.IsValid)
            {
                var config = _configurationRepository.Configuration;
                config.SiteName = model.SiteName;
                config.IssuerUri = model.IssuerUri;

                _configurationRepository.UpdateConfiguration(config);
                _configurationRepository.UpdateCertificates(null, model.SigningCertificate);

                return RedirectToAction("index", "home");
            }

            ModelState.AddModelError("", "Errors ocurred...");
            model.AvailableCertificates = GetAvailableCertificatesFromStore();
            return View(model);
        }

        #region Helper
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
