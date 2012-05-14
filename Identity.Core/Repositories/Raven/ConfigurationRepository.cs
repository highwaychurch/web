using System.Configuration;
using System.Linq;
using AutoMapper;
using Highway.Identity.Core.Models;
using Highway.Identity.Core.Repositories.Raven.Documents;
using Raven.Client;

namespace Highway.Identity.Core.Repositories.Raven
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private const string SslCertificateName = "SSL";
        private const string SigningCertificateName = "SigningCertificate";

        readonly IDocumentSession _session;
        
        static ConfigurationRepository()
        {
            Mapper.CreateMap<GlobalConfigurationModel, GlobalConfiguration>();
            Mapper.CreateMap<GlobalConfiguration, GlobalConfigurationModel>();
            Mapper.CreateMap<EndpointsConfigurationModel, EndpointsConfiguration>();
            Mapper.CreateMap<EndpointsConfiguration, EndpointsConfigurationModel>();

            Mapper.AssertConfigurationIsValid();
        }

        public ConfigurationRepository(IDocumentSession session)
        {
            _session = session;
        }

        public GlobalConfigurationModel Configuration
        {
            get
            {
                var global = _session.Query<GlobalConfiguration>().FirstOrDefault();
                if (global == null)
                {
                    global = new GlobalConfiguration();
                    _session.Store(global);
                }
                return Mapper.Map<GlobalConfiguration, GlobalConfigurationModel>(global);
            }
        }

        public EndpointsConfigurationModel Endpoints
        {
            get
            {
                var global = _session.Query<EndpointsConfiguration>().FirstOrDefault();
                if (global == null)
                {
                    global = new EndpointsConfiguration();
                    _session.Store(global);
                }
                return Mapper.Map<EndpointsConfiguration, EndpointsConfigurationModel>(global);
            }
        }

        public CertificateModel SslCertificate
        {
            get
            {
                var sslCertificate = _session.Query<Certificate>().FirstOrDefault(c => c.Name == SslCertificateName) ?? new Certificate();
                return sslCertificate.ToModel();
            }
        }

        public CertificateModel SigningCertificate
        {
            get
            {
                var signingCertificate = _session.Query<Certificate>().FirstOrDefault(c => c.Name == SigningCertificateName) ?? new Certificate();
                return signingCertificate.ToModel();
            }
        }

        public bool SupportsWriteAccess
        {
            get { return true; }
        }

        public void UpdateConfiguration(GlobalConfigurationModel updatedConfiguration)
        {
            var doc = _session.Load<GlobalConfiguration>(updatedConfiguration.Id);
            Mapper.Map(updatedConfiguration, doc);
        }

        public void UpdateEndpoints(EndpointsConfigurationModel updatedEndpointsConfiguration)
        {
            var doc = _session.Load<EndpointsConfiguration>(updatedEndpointsConfiguration.Id);
            Mapper.Map(updatedEndpointsConfiguration, doc);
        }

        public void UpdateCertificates(string sslSubjectName, string signingSubjectName)
        {
            if (!string.IsNullOrWhiteSpace(sslSubjectName))
            {
                var sslCertificate = _session.Query<Certificate>().FirstOrDefault(c => c.Name == SslCertificateName);
                if (sslCertificate == null)
                {
                    sslCertificate = new Certificate();
                    sslCertificate.Name = SslCertificateName;
                    _session.Store(sslCertificate);
                }
                sslCertificate.SubjectDistinguishedName = sslSubjectName;
            }

            if (!string.IsNullOrWhiteSpace(signingSubjectName))
            {
                var signingCertificate = _session.Query<Certificate>().FirstOrDefault(c => c.Name == SigningCertificateName);
                if (signingCertificate == null)
                {
                    signingCertificate = new Certificate();
                    signingCertificate.Name = SigningCertificateName;
                    _session.Store(signingCertificate);
                }
                signingCertificate.SubjectDistinguishedName = signingSubjectName;
            }
        }
    }
}
