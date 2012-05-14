/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */


using System;
using System.Security.Cryptography;
using System.Web.Mvc;
using Highway.Identity.Core;
using Highway.Identity.Core.Repositories;
using Highway.Identity.Web.Security;
using Highway.Identity.Web.ViewModels.Administration;

namespace Highway.Identity.Web.Controllers.Admin
{
    [ClaimsAuthorize(Constants.Actions.Administration, Constants.Resources.RelyingParty)]
    public class RelyingPartiesAdminController : Controller
    {
        readonly IRelyingPartyRepository _repository;

        public RelyingPartiesAdminController(IRelyingPartyRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            var rps = _repository.List(-1, -1);
            return View(rps.ToViewModel());
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(RelyingPartyViewModel relyingParty)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TrySetCertificateFromUpload(relyingParty);
                    _repository.Add(relyingParty.ToDomainModel());

                    return RedirectToAction("Index");
                }

                return View(relyingParty);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(relyingParty);
            }
        }

        public ActionResult Edit(string id)
        {
            var rp = _repository.Get(id);
            return View(rp.ToViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RelyingPartyViewModel relyingParty)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TrySetCertificateFromUpload(relyingParty);
                    _repository.Update(relyingParty.ToDomainModel());

                    return RedirectToAction("Index");
                }

                return View(relyingParty);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException.Message);
                return View(relyingParty);
            }
        }

        public ActionResult Delete(string id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, FormCollection collection)
        {
            try
            {
                _repository.Delete(id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException.Message);
                return View();
            }
        }

        [HttpPost]
        public ActionResult GenerateSymmetricSigningKey()
        {
            var bytes = new byte[32];
            new RNGCryptoServiceProvider().GetBytes(bytes);

            return Json(Convert.ToBase64String(bytes));
        }

        private void TrySetCertificateFromUpload(RelyingPartyViewModel relyingParty)
        {
            if (relyingParty.CertificateUpload != null && relyingParty.CertificateUpload.ContentLength > 0)
            {
                byte[] bytes = new byte[relyingParty.CertificateUpload.InputStream.Length];
                relyingParty.CertificateUpload.InputStream.Read(bytes, 0, bytes.Length);

                relyingParty.EncryptingCertificate = Convert.ToBase64String(bytes);
            }
        }
    }
}
