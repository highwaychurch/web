﻿/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Linq;
using System.Web.Mvc;
using Highway.Identity.Core;
using Highway.Identity.Core.Models;
using Highway.Identity.Core.Repositories;
using Highway.Identity.Web.Security;
using Highway.Identity.Web.ViewModels.Administration;

namespace Highway.Identity.Web.Controllers.Admin
{
    [ClaimsAuthorize(Constants.Actions.Administration, Constants.Resources.Delegation)]
    public class DelegationAdminController : Controller
    {
        readonly IDelegationRepository _repository;

        public DelegationAdminController(IDelegationRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            var users = _repository.GetAllUsers(-1, -1);

            return View(users.ToList());
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddDelegationModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var setting = new DelegationModel
            {
                UserName = model.UserName,
                Realm = new Uri(model.Realm),
                Description = model.Description
            };

            try
            {
                _repository.Add(setting);
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
            var model = new EditDelegationModel
            {
                UserName = userName
            };

            model.Settings = _repository.GetDelegationSettingsForUser(userName);

            if (model.Settings.ToList().Count == 0)
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
                _repository.GetDelegationSettingsForUser(userName).ToList().ForEach(setting =>
                    _repository.Delete(new DelegationModel { UserName = userName, Realm = setting.Realm }));
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
        public ActionResult Delete(string userName, string realm, FormCollection collection)
        {
            try
            {
                _repository.Delete(new DelegationModel { UserName = userName, Realm = new Uri(realm) });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException.Message ?? ex.Message);
                return View("Delete");
            }

            return RedirectToAction("Edit", new { userName = userName });
        }
    }
}