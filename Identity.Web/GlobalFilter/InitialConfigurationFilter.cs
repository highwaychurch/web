/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Highway.Identity.Core.Repositories;

namespace Highway.Identity.Web.GlobalFilter
{
    public class InitialConfigurationFilter : ActionFilterAttribute
    {
        readonly IConfigurationRepository _configurationRepository;

        public InitialConfigurationFilter(IConfigurationRepository configurationRepository)
        {
            _configurationRepository = configurationRepository;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Equals("InitialConfiguration"))
            {
                if (string.IsNullOrWhiteSpace(_configurationRepository.SigningCertificate.SubjectDistinguishedName))
                {
                    var route = new RouteValueDictionary(new Dictionary<string, object>
                        {
                            { "Controller", "InitialConfiguration" },
                        });

                    filterContext.Result = new RedirectToRouteResult(route);
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}