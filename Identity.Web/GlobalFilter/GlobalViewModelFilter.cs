/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Web.Mvc;
using Highway.Identity.Core;
using Highway.Identity.Web.Security;

namespace Highway.Identity.Web.GlobalFilter
{
    public class GlobalViewModelFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.Controller.ViewBag.IsAdministrator = ClaimsAuthorize.CheckAccess(Constants.Actions.Administration, Constants.Resources.Configuration);
            filterContext.Controller.ViewBag.IsSignedIn = filterContext.HttpContext.User.Identity.IsAuthenticated;

            base.OnActionExecuting(filterContext);
        }
    }
}