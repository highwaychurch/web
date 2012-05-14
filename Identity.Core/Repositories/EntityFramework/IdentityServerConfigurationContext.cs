/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Highway.Identity.Core.Repositories.EntityFramework.EntityModel;

namespace Highway.Identity.Core.Repositories.EntityFramework
{    
    public class IdentityServerConfigurationContext : DbContext
    {
        public DbSet<GlobalConfigurationEntity> Global { get; set; }

        public DbSet<CertificateEntity> Certificates { get; set; }

        public DbSet<ClientCertificateEntity> ClientCertificates { get; set; }

        public DbSet<DelegationEntity> Delegation { get; set; }

        public DbSet<RelyingPartyEntity> RelyingParties { get; set; }

        public DbSet<EntityModel.EndpointsConfigurationEntity> Endpoints { get; set; }

        public static Func<IdentityServerConfigurationContext> FactoryMethod { get; set; }

        public IdentityServerConfigurationContext()
        {            
        }

        public IdentityServerConfigurationContext(DbConnection conn) : base(conn, true)
        {
        }

        public IdentityServerConfigurationContext(IDatabaseInitializer<IdentityServerConfigurationContext> initializer)
        {
            Database.SetInitializer(initializer);
        }

        public static IdentityServerConfigurationContext Get()
        {
            if (FactoryMethod != null) return FactoryMethod();

            var cs = ConfigurationManager.ConnectionStrings["IdentityServerConfiguration"].ConnectionString;
            var conn = Database.DefaultConnectionFactory.CreateConnection(cs);
            return new IdentityServerConfigurationContext(conn);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
