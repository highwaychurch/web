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
using Highway.Identity.Core.Helper;
using Highway.Identity.Core.Repositories;
using Highway.Identity.Core.TokenService;

namespace Highway.Identity.Web.Controllers
{
    public class FederationMetadataController : Controller
    {
        readonly IConfigurationRepository _configurationRepository;
        readonly ICacheRepository _cacheRepository;
        readonly IUserRepository _userRepository;

        public FederationMetadataController(IConfigurationRepository configurationRepository, ICacheRepository cacheRepository, IUserRepository userRepository)
        {
            _configurationRepository = configurationRepository;
            _cacheRepository = cacheRepository;
            _userRepository = userRepository;
        }

        public ActionResult Generate()
        {
            if (_configurationRepository.Endpoints.FederationMetadata)
            {
                return Cache.ReturnFromCache<ActionResult>(_cacheRepository, Constants.CacheKeys.WSFedMetadata, 1, () =>
                    {
                        var endpoints = Endpoints.Create(
                            HttpContext.Request.Headers["Host"],
                            HttpContext.Request.ApplicationPath,
                            _configurationRepository.Endpoints.HttpPort,
                            _configurationRepository.Endpoints.HttpsPort);

                        return new ContentResult
                        {
                            Content = new WSFederationMetadataGenerator(endpoints, _configurationRepository, _userRepository).Generate(),
                            ContentType = "text/xml"
                        };
                    });
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }
    }
}
