/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using Highway.Identity.Core;
using Highway.Identity.Core.Models;
using Highway.Identity.Core.Repositories;
using Highway.Identity.Web.Security;
using Highway.Identity.Web.ViewModels.Administration;

namespace Highway.Identity.Web.Controllers.Admin
{
    [ClaimsAuthorize(Constants.Actions.Administration, Constants.Resources.ClientCertificates)]
    public class ClientCertificatesAdminController : Controller
    {
        readonly IClientCertificateRepository _repository;

        public ClientCertificatesAdminController(IClientCertificateRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            var users = _repository.List(-1, -1);

            return View(users.ToList());
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddClientCertificateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var clientCert = new ClientCertificateModel
            {
                UserName = model.UserName,
                Description = model.Description
            };

            if (model.CertificateUpload != null && model.CertificateUpload.ContentLength > 0)
            {
                var bytes = new byte[model.CertificateUpload.InputStream.Length];
                model.CertificateUpload.InputStream.Read(bytes, 0, bytes.Length);

                clientCert.Thumbprint = new X509Certificate2(bytes).Thumbprint;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.Thumbprint))
                {
                    ModelState.AddModelError("", "No certificate (or thumbprint) specified");
                    return View();
                }

                clientCert.Thumbprint = model.Thumbprint;
            }

            try
            {
                _repository.Add(clientCert);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException.Message);
                return View("Add");
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(string userName)
        {
            var model = new EditClientCertificatesModel
            {
                UserName = userName
            };

            model.ClientCertificates = _repository.GetClientCertificatesForUser(userName);

            if (model.ClientCertificates.ToList().Count == 0)
            {
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult DeleteAll(string userName)
        {
            return View("Delete");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAll(string userName, FormCollection collection)
        {
            try
            {
                _repository.GetClientCertificatesForUser(userName).ToList().ForEach(cert =>
                    _repository.Delete(new ClientCertificateModel { UserName = userName, Thumbprint = cert.Thumbprint }));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException.Message);
                return View("Delete");
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(string userName, string thumbprint)
        {
            return View("Delete");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string userName, string thumbprint, FormCollection collection)
        {
            try
            {
                _repository.Delete(new ClientCertificateModel { UserName = userName, Thumbprint = thumbprint });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException.Message);
                return View("Delete");
            }

            return RedirectToAction("Edit", new { userName = userName });
        }
    }
}
