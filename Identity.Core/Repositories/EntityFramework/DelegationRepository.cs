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
using Highway.Identity.Core.Models;
using Highway.Identity.Core.Repositories.EntityFramework.EntityModel;

namespace Highway.Identity.Core.Repositories.EntityFramework
{    
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DelegationRepository : IDelegationRepository
    {
        #region Runtime
        public bool IsDelegationAllowed(string userName, string realm)
        {
            using (var entities = IdentityServerConfigurationContext.Get())
            {
                var record = (from entry in entities.Delegation
                              where entry.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) &&
                                    entry.Realm.Equals(realm, StringComparison.OrdinalIgnoreCase)
                              select entry).FirstOrDefault();

                return (record != null);
            }
        }
        #endregion

        #region Management
        public bool SupportsWriteAccess
        {
            get { return true; }
        }

        public IEnumerable<string> GetAllUsers(int pageIndex, int pageSize)
        {
            using (var entities = IdentityServerConfigurationContext.Get())
            {
                var users =
                    (from user in entities.Delegation
                     orderby user.UserName
                     select user.UserName)
                    .Distinct();

                if (pageIndex != -1 && pageSize != -1)
                {
                    users = users.Skip(pageIndex * pageSize).Take(pageSize);
                }

                return users.ToList();
            }
        }

        public IEnumerable<DelegationModel> GetDelegationSettingsForUser(string userName)
        {
            using (var entities = IdentityServerConfigurationContext.Get())
            {
                var settings =
                     from record in entities.Delegation
                     where record.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
                     select record;

                return settings.ToList().ToDomainModel();
            }
        }

        public void Add(DelegationModel model)
        {
            using (var entities = IdentityServerConfigurationContext.Get())
            {
                var entity = new DelegationEntity
                {
                    UserName = model.UserName,
                    Realm = model.Realm.AbsoluteUri,
                    Description = model.Description
                };

                entities.Delegation.Add(entity);
                entities.SaveChanges();
            }
        }

        public void Delete(DelegationModel model)
        {
            using (var entities = IdentityServerConfigurationContext.Get())
            {
                var record =
                    (from entry in entities.Delegation
                     where entry.UserName.Equals(model.UserName, StringComparison.OrdinalIgnoreCase) &&
                           entry.Realm.Equals(model.Realm.AbsoluteUri, StringComparison.OrdinalIgnoreCase)
                     select entry)
                    .Single();

                entities.Delegation.Remove(record);
                entities.SaveChanges();
            }
        }
        #endregion
    }
}
