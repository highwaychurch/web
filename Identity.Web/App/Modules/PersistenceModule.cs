using Autofac;
using Autofac.Integration.Mvc;
using Highway.Identity.Core.Repositories;
using Highway.Identity.Core.Repositories.Raven;
using Highway.Shared.Mvc;
using Highway.Shared.Persistence;
using Raven.Client;
using Raven.Client.Document;

namespace Highway.Identity.Web.App.Modules
{
    public class PersistenceModule : PersistenceModuleBase
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // Start the document store
            var documentStore = new DocumentStore {ConnectionStringName = "RavenDB"};
            documentStore.Initialize();
            builder.RegisterInstance(documentStore).As<IDocumentStore>().SingleInstance();

            builder.Register(c => c.Resolve<IDocumentStore>().OpenSession())
                .As<IDocumentSession>()
                .OnActivated(e => e.Context.Resolve<ITransactionContext>().AddPersistentResource(e.Instance, e.Instance.SaveChanges))
                .InstancePerHttpRequest();

            builder.RegisterType<ConfigurationRepository>().As<IConfigurationRepository>().InstancePerHttpRequest();
            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerHttpRequest();
            builder.RegisterType<RelyingPartyRepository>().As<IRelyingPartyRepository>().InstancePerHttpRequest();
            builder.RegisterType<MemoryCacheRepository>().As<ICacheRepository>().InstancePerHttpRequest();
            builder.RegisterType<ClientCertificateRepository>().As<IClientCertificateRepository>().InstancePerHttpRequest();
            builder.RegisterType<DelegationRepository>().As<IDelegationRepository>().InstancePerHttpRequest();

            // TODO: Look at async!
            //builder.Register(c => c.Resolve<IDocumentStore>().OpenAsyncSession())
            //    .As<IAsyncDocumentSession>()
            //    .OnActivated(e => e.Context.Resolve<ITransactionContext>().AddPersistentResource(e.Instance, e.Instance.SaveChangesAsync))
            //    .InstancePerHttpRequest();
        }
    }
}
