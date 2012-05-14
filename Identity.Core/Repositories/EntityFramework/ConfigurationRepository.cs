/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using Highway.Identity.Core.Helper;
using Highway.Identity.Core.Models;
using Highway.Identity.Core.Repositories.EntityFramework.EntityModel;

namespace Highway.Identity.Core.Repositories.EntityFramework
{        
    public class ConfigurationRepository : IConfigurationRepository
    {
        private const string StandardConfigurationName = "Standard";
        private const string SslCertificateName        = "SSL";
        private const string SigningCertificateName    = "SigningCertificate";

        private const string EndpointConfigurationCacheKey = "Cache_EndpointConfiguration";
        private const string GlobalConfigurationCacheKey   = "Cache_GlobalConfiguration";
        private const string SslCertificateCachekey        = "Cache_SslCertificate";
        private const string SigningCertificateCacheKey    = "Cache_SigningCertificate";

        readonly ICacheRepository _cacheRepository;

        static ConfigurationRepository()
        {
            Mapper.CreateMap<GlobalConfigurationModel, GlobalConfigurationEntity>()
                .ForMember(m => m.Name, opt => opt.Ignore());

            Mapper.CreateMap<GlobalConfigurationEntity, GlobalConfigurationModel>();

            Mapper.CreateMap<EndpointsConfigurationModel, EntityModel.EndpointsConfigurationEntity>()
                .ForMember(m => m.Name, opt => opt.Ignore());

            Mapper.CreateMap<EntityModel.EndpointsConfigurationEntity, EndpointsConfigurationModel>();

            Mapper.AssertConfigurationIsValid();
        }

        public ConfigurationRepository(ICacheRepository cacheRepository)
        {
            _cacheRepository = cacheRepository;
        }

        #region Runtime
        public GlobalConfigurationModel Configuration
        {
            get
            {
                return Cache.ReturnFromCache<GlobalConfigurationModel>(_cacheRepository, GlobalConfigurationCacheKey, 1, () =>
                {
                    using (var entities = IdentityServerConfigurationContext.Get())
                    {
                        var global = (from c in entities.Global where c.Name == StandardConfigurationName select c).FirstOrDefault();
                        if (global == null)
                        {
                            throw new ConfigurationErrorsException("No standard global configuration found");
                        }

                        return Mapper.Map<GlobalConfigurationEntity, GlobalConfigurationModel>(global);
                    }
                });
            }
        }

        public EndpointsConfigurationModel Endpoints
        {
            get
            {
                return Cache.ReturnFromCache<EndpointsConfigurationModel>(_cacheRepository, EndpointConfigurationCacheKey, 1, () =>
                {
                    using (var entities = IdentityServerConfigurationContext.Get())
                    {
                        var eps = (from c in entities.Endpoints where c.Name == StandardConfigurationName select c).FirstOrDefault();
                        if (eps == null)
                        {
                            throw new ConfigurationErrorsException("No standard endpoint configuration found in database");
                        }

                        return Mapper.Map<EntityModel.EndpointsConfigurationEntity, EndpointsConfigurationModel>(eps);
                    }
                });
            }
        }

        public CertificateModel SslCertificate
        {
            get
            {
                return Cache.ReturnFromCache<CertificateModel>(_cacheRepository, SslCertificateCachekey, 1, () =>
                {
                    using (var entities = IdentityServerConfigurationContext.Get())
                    {
                        var cert = (from c in entities.Certificates where c.Name == SslCertificateName select c).FirstOrDefault();
                        if (cert == null)
                        {
                            throw new ConfigurationErrorsException("No SSL certificate found in database");
                        }

                        return cert.ToDomainModel();
                    }
                });
            }
        }

        public CertificateModel SigningCertificate
        {
            get
            {
                return Cache.ReturnFromCache<CertificateModel>(_cacheRepository, SigningCertificateCacheKey, 1, () =>
                {
                    using (var entities = IdentityServerConfigurationContext.Get())
                    {
                        var cert = (from c in entities.Certificates where c.Name == SigningCertificateName select c).FirstOrDefault();
                        if (cert == null)
                        {
                            throw new ConfigurationErrorsException("No signing certificate found in database");
                        }

                        return cert.ToDomainModel();
                    }
                });
            }
        }

        #endregion

        #region Management
        public bool SupportsWriteAccess
        {
            get { return true; }
        }

        public void UpdateConfiguration(GlobalConfigurationModel updatedConfiguration)
        {
            using (var entities = IdentityServerConfigurationContext.Get())
            {
                var entity = Mapper.Map<GlobalConfigurationModel, GlobalConfigurationEntity>(updatedConfiguration);
                entity.Name = StandardConfigurationName;

                entities.Global.Attach(entity);
                entities.Entry(entity).State = EntityState.Modified;

                entities.SaveChanges();
                _cacheRepository.Invalidate(GlobalConfigurationCacheKey);
                _cacheRepository.Invalidate(Constants.CacheKeys.WSFedMetadata);
            }
        }

        public void UpdateEndpoints(EndpointsConfigurationModel endpoints)
        {
            using (var entities = IdentityServerConfigurationContext.Get())
            {
                var entity = Mapper.Map<EndpointsConfigurationModel, EntityModel.EndpointsConfigurationEntity>(endpoints);
                entity.Name = StandardConfigurationName;

                entities.Endpoints.Attach(entity);
                entities.Entry(entity).State = System.Data.EntityState.Modified;

                entities.SaveChanges();
                _cacheRepository.Invalidate(EndpointConfigurationCacheKey);
                _cacheRepository.Invalidate(Constants.CacheKeys.WSFedMetadata);
            }
        }

        public void UpdateCertificates(string sslSubjectName, string signingSubjectName)
        {
            using (var entities = IdentityServerConfigurationContext.Get())
            {
                var certs = entities.Certificates;

                if (!string.IsNullOrWhiteSpace(sslSubjectName))
                {
                    var ssl = new CertificateEntity
                    {
                        Name = SslCertificateName,
                        SubjectDistinguishedName = sslSubjectName
                    };

                    certs.Attach(ssl);
                    entities.Entry(ssl).State = EntityState.Modified;
                }

                if (!string.IsNullOrWhiteSpace(signingSubjectName))
                {
                    var signing = new CertificateEntity
                    {
                        Name = SigningCertificateName,
                        SubjectDistinguishedName = signingSubjectName
                    };

                    certs.Attach(signing);
                    entities.Entry(signing).State = EntityState.Modified;
                }

                entities.SaveChanges();

                _cacheRepository.Invalidate(SigningCertificateCacheKey);
                _cacheRepository.Invalidate(SslCertificateCachekey);
                _cacheRepository.Invalidate(Constants.CacheKeys.WSFedMetadata);
            }
        }
        #endregion
    }
}
