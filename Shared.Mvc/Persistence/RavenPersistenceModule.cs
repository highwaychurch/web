using Autofac;
using Autofac.Integration.Mvc;
using Raven.Client;
using Raven.Client.Document;

namespace Highway.Shared.Mvc.Persistence
{
    public class RavenPersistenceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // Start the document store
            var documentStore = new DocumentStore {ConnectionStringName = "RavenDB"};
            documentStore.Conventions.IdentityPartsSeparator = "-"; // make RavenDB identities MVC friendly
            documentStore.Initialize();
            builder.RegisterInstance(documentStore).As<IDocumentStore>()
                .SingleInstance();

            builder.Register(c => c.Resolve<IDocumentStore>().OpenSession()).As<IDocumentSession>()
                .InstancePerHttpRequest();

            builder.Register(c => c.Resolve<IDocumentStore>().OpenAsyncSession()).As<IAsyncDocumentSession>()
                .InstancePerHttpRequest();
        }
    }
}
