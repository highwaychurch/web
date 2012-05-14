/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Highway.Identity.Core.Helper;
using Highway.Identity.Core.Models;
using Highway.Identity.Core.Repositories.EntityFramework.EntityModel;

namespace Highway.Identity.Core.Repositories.EntityFramework
{
    internal static class Extensions
    {
        #region Relying Party
        public static RelyingPartyModel ToDomainModel(this RelyingPartyEntity rpEntity)
        {
            var rp = new RelyingPartyModel 
            {
                Id = rpEntity.Id.ToString(),
                Name = rpEntity.Name,
                Realm = new Uri("http://" + rpEntity.Realm),
                ExtraData1 = rpEntity.ExtraData1,
                ExtraData2 = rpEntity.ExtraData2,
                ExtraData3 = rpEntity.ExtraData3
            };

            if (!string.IsNullOrWhiteSpace(rpEntity.ReplyTo))
            {
                rp.ReplyTo = new Uri(rpEntity.ReplyTo);
            }

            if (!string.IsNullOrWhiteSpace(rpEntity.EncryptingCertificate))
            {
                rp.EncryptingCertificate = new X509Certificate2(Convert.FromBase64String(rpEntity.EncryptingCertificate));
            }

            if (!string.IsNullOrWhiteSpace(rpEntity.SymmetricSigningKey))
            {
                rp.SymmetricSigningKey = Convert.FromBase64String(rpEntity.SymmetricSigningKey);
            }

            return rp;
        }

        public static RelyingPartyEntity ToEntity(this RelyingPartyModel relyingParty)
        {
            var rpEntity = new RelyingPartyEntity
            {
                Name = relyingParty.Name,
                Realm = relyingParty.Realm.AbsoluteUri.StripProtocolMoniker(),
                ExtraData1 = relyingParty.ExtraData1,
                ExtraData2 = relyingParty.ExtraData2,
                ExtraData3 = relyingParty.ExtraData3,
            };

            if (!string.IsNullOrEmpty(relyingParty.Id))
            {
                rpEntity.Id = int.Parse(relyingParty.Id);
            }

            if (relyingParty.ReplyTo != null)
            {
                rpEntity.ReplyTo = relyingParty.ReplyTo.AbsoluteUri;
            }

            if (relyingParty.EncryptingCertificate != null)
            {
                rpEntity.EncryptingCertificate = Convert.ToBase64String(relyingParty.EncryptingCertificate.RawData);
            }

            if (relyingParty.SymmetricSigningKey != null && relyingParty.SymmetricSigningKey.Length != 0)
            {
                rpEntity.SymmetricSigningKey = Convert.ToBase64String(relyingParty.SymmetricSigningKey);
            }

            return rpEntity;
        }

        public static IEnumerable<RelyingPartyModel> ToDomainModel(this List<RelyingPartyEntity> relyingParties)
        {
            return
                (from rp in relyingParties
                 select new RelyingPartyModel
                 {
                     Id = rp.Id.ToString(),
                     Name = rp.Name,
                     Realm = new Uri("http://" + rp.Realm)
                 }).ToList();
        }
        #endregion

        #region Client Certificates
        public static List<ClientCertificateModel> ToDomainModel(this List<ClientCertificateEntity> entities)
        {
            return
                (from entity in entities
                 select new ClientCertificateModel
                 {
                     UserName = entity.UserName,
                     Thumbprint = entity.Thumbprint,
                     Description = entity.Description
                 }
                ).ToList();
        }
        #endregion

        #region Delegation
        public static List<DelegationModel> ToDomainModel(this List<DelegationEntity> entities)
        {
            return
                (from entity in entities
                 select new DelegationModel
                 {
                     UserName = entity.UserName,
                     Realm = new Uri(entity.Realm),
                     Description = entity.Description
                 }
                ).ToList();
        }
        #endregion

        #region Misc
        public static string StripProtocolMoniker(this string uriString)
        {
            var uri = new Uri(uriString);
            string stripped = uri.AbsoluteUri.Substring(uri.Scheme.Length + 3);
            return stripped.ToLowerInvariant();
        }
        #endregion

        public static CertificateModel ToDomainModel(this CertificateEntity certificate)
        {
            object findValue;
            X509FindType findType;

            var certConfig = new CertificateModel
            {
                SubjectDistinguishedName = certificate.SubjectDistinguishedName,
            };

            if (!string.IsNullOrWhiteSpace(certificate.SubjectDistinguishedName))
            {
                findValue = certificate.SubjectDistinguishedName;
                findType = X509FindType.FindBySubjectDistinguishedName;
            }
            else
            {
                Tracing.Tracing.Error("No distinguished name or thumbprint for certificate: " + certificate.Name);
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
