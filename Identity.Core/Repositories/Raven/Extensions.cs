using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Highway.Identity.Core.Helper;
using Highway.Identity.Core.Models;
using Highway.Identity.Core.Repositories.Raven.Documents;

namespace Highway.Identity.Core.Repositories.Raven
{
    internal static class Extensions
    {
        public static RelyingPartyModel ToModel(this RelyingParty document)
        {
            var rp = new RelyingPartyModel 
            {
                Id = document.Id,
                Name = document.Name,
                Realm = new Uri("http://" + document.Realm),
                ExtraData1 = document.ExtraData1,
                ExtraData2 = document.ExtraData2,
                ExtraData3 = document.ExtraData3
            };

            if (!string.IsNullOrWhiteSpace(document.ReplyTo))
            {
                rp.ReplyTo = new Uri(document.ReplyTo);
            }

            if (!string.IsNullOrWhiteSpace(document.EncryptingCertificate))
            {
                rp.EncryptingCertificate = new X509Certificate2(Convert.FromBase64String(document.EncryptingCertificate));
            }

            if (!string.IsNullOrWhiteSpace(document.SymmetricSigningKey))
            {
                rp.SymmetricSigningKey = Convert.FromBase64String(document.SymmetricSigningKey);
            }

            return rp;
        }

        public static RelyingParty ToDocument(this RelyingPartyModel model)
        {
            var document = new RelyingParty();
            model.MergeWithDocument(document);
            return document;
        }

        public static void MergeWithDocument(this RelyingPartyModel model, RelyingParty document)
        {
            document.Id = model.Id;
            document.Name = model.Name;
            document.Realm = model.Realm.AbsoluteUri.StripProtocolMoniker();
            document.ExtraData1 = model.ExtraData1;
            document.ExtraData2 = model.ExtraData2;
            document.ExtraData3 = model.ExtraData3;

            if (!string.IsNullOrEmpty(model.Id))
            {
                document.Id = model.Id;
            }

            if (model.ReplyTo != null)
            {
                document.ReplyTo = model.ReplyTo.AbsoluteUri;
            }

            if (model.EncryptingCertificate != null)
            {
                document.EncryptingCertificate = Convert.ToBase64String(model.EncryptingCertificate.RawData);
            }

            if (model.SymmetricSigningKey != null && model.SymmetricSigningKey.Length != 0)
            {
                document.SymmetricSigningKey = Convert.ToBase64String(model.SymmetricSigningKey);
            }
        }

        public static IEnumerable<RelyingPartyModel> ToModel(this List<RelyingParty> documents)
        {
            return
                (from doc in documents
                 select new RelyingPartyModel
                            {
                                Id = doc.Id,
                                Name = doc.Name,
                                Realm = new Uri("http://" + doc.Realm)
                            }).ToList();
        }

        public static List<ClientCertificateModel> ToModels(this List<ClientCertificate> documents)
        {
            return
                (from doc in documents
                 select new ClientCertificateModel
                            {
                                Id = doc.Id,
                                UserName = doc.UserName,
                                Thumbprint = doc.Thumbprint,
                                Description = doc.Description
                            }
                ).ToList();
        }

        public static List<DelegationModel> ToModels(this List<Delegation> documents)
        {
            return
                (from doc in documents
                 select new DelegationModel
                            {
                                Id = doc.Id,
                                UserName = doc.UserName,
                                Realm = new Uri(doc.Realm),
                                Description = doc.Description
                            }
                ).ToList();
        }

        public static string StripProtocolMoniker(this string uriString)
        {
            var uri = new Uri(uriString);
            var stripped = uri.AbsoluteUri.Substring(uri.Scheme.Length + 3);
            return stripped.ToLowerInvariant();
        }

        public static CertificateModel ToModel(this Certificate document)
        {
            object findValue;
            X509FindType findType;

            var certConfig = new CertificateModel
            {
                Id = document.Id,
                SubjectDistinguishedName = document.SubjectDistinguishedName,
            };

            if (!string.IsNullOrWhiteSpace(document.SubjectDistinguishedName))
            {
                findValue = document.SubjectDistinguishedName;
                findType = X509FindType.FindBySubjectDistinguishedName;
            }
            else
            {
                Tracing.Tracing.Error("No distinguished name or thumbprint for certificate: " + document.Name);
                return certConfig;
            }

            try
            {
                certConfig.Certificate = X509Certificates.GetCertificateFromStore(StoreLocation.LocalMachine, StoreName.My, findType, findValue);
            }
            catch
            {
                Tracing.Tracing.Error("No certificate found for: " + findValue);
                throw new ConfigurationErrorsException("No certificate found for: " + findValue);
            }

            return certConfig;
        }
    }
}
