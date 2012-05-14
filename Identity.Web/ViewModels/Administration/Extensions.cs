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
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using Highway.Identity.Core.Models;

namespace Highway.Identity.Web.ViewModels.Administration
{
    internal static class Extensions
    {
        static Extensions()
        {
            Mapper.CreateMap<EndpointsConfigurationModel, EndpointsConfigurationViewModel>();
            Mapper.CreateMap<EndpointsConfigurationViewModel, EndpointsConfigurationModel>();

            Mapper.CreateMap<GlobalConfigurationViewModel, GlobalConfigurationModel>();
            Mapper.CreateMap<GlobalConfigurationModel, GlobalConfigurationViewModel>();

            Mapper.AssertConfigurationIsValid();
        }

        #region RelyingParty
        public static RelyingPartyViewModel ToViewModel(this RelyingPartyModel relyingParty)
        {
            var model = new RelyingPartyViewModel
            {
                Name = relyingParty.Name,
                Realm = relyingParty.Realm.AbsoluteUri,
                ExtraData1 = relyingParty.ExtraData1,
                ExtraData2 = relyingParty.ExtraData2,
                ExtraData3 = relyingParty.ExtraData3
            };

            if (relyingParty.EncryptingCertificate != null)
            {
                model.EncryptingCertificate = Convert.ToBase64String(relyingParty.EncryptingCertificate.RawData);
                model.EncryptingCertificateName = relyingParty.EncryptingCertificate.Subject;
            };

            if (relyingParty.ReplyTo != null)
            {
                model.ReplyTo = relyingParty.ReplyTo.AbsoluteUri;
            }

            if (relyingParty.SymmetricSigningKey != null && relyingParty.SymmetricSigningKey.Length != 0)
            {
                model.SymmetricSigningKey = Convert.ToBase64String(relyingParty.SymmetricSigningKey);
            }

            return model;
        }

        public static RelyingPartyModel ToDomainModel(this RelyingPartyViewModel model)
        {
            var rp = new RelyingPartyModel
            {
                Id = model.Id,
                Name = model.Name,
                Realm = new Uri(model.Realm),
                ExtraData1 = model.ExtraData1,
                ExtraData2 = model.ExtraData2,
                ExtraData3 = model.ExtraData3,
            };

            if (!string.IsNullOrWhiteSpace(model.ReplyTo))
            {
                rp.ReplyTo = new Uri(model.ReplyTo);
            }

            if (!string.IsNullOrWhiteSpace(model.EncryptingCertificate))
            {
                rp.EncryptingCertificate = new X509Certificate2(Convert.FromBase64String(model.EncryptingCertificate));
            }

            if (!string.IsNullOrWhiteSpace(model.SymmetricSigningKey))
            {
                rp.SymmetricSigningKey = Convert.FromBase64String(model.SymmetricSigningKey);
            }

            return rp;
        }

        public static RelyingPartiesViewModel ToViewModel(this IEnumerable<RelyingPartyModel> relyingParties)
        {
            var model = new RelyingPartiesViewModel
            {
                RelyingParties =
                    (from rp in relyingParties
                     select new RelyingPartyViewModel
                     {
                         Id = rp.Id,
                         Name = rp.Name,
                         Realm = rp.Realm.AbsoluteUri
                     })
                    .ToList()
            };

            return model;
        }
        #endregion

        #region GlobalConfiguration
        public static GlobalConfigurationViewModel ToViewModel(this GlobalConfigurationModel configuration)
        {
            return Mapper.Map<GlobalConfigurationModel, GlobalConfigurationViewModel>(configuration);
        }

        public static GlobalConfigurationModel ToDomainModel(this GlobalConfigurationViewModel model)
        {
            return Mapper.Map<GlobalConfigurationViewModel, GlobalConfigurationModel>(model);
        }
        #endregion

        #region EndpointConfiguration
        public static EndpointsConfigurationViewModel ToViewModel(this EndpointsConfigurationModel endpoints)
        {
            return Mapper.Map<EndpointsConfigurationModel, EndpointsConfigurationViewModel>(endpoints);
        }

        public static EndpointsConfigurationModel ToDomainModel(this EndpointsConfigurationViewModel model)
        {
            return Mapper.Map<EndpointsConfigurationViewModel, EndpointsConfigurationModel>(model);
        }
        #endregion
    }
}