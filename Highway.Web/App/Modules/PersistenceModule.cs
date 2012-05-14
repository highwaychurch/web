using Autofac;
using Autofac.Integration.Mvc;
using Highway.Shared.Persistence;
using Raven.Client;
using Raven.Client.Document;

namespace Highway.Web.App.Modules
{
    public class PersistenceModule : Shared.Mvc.PersistenceModule
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

            // TODO: Look at async!
            //builder.Register(c => c.Resolve<IDocumentStore>().OpenAsyncSession())
            //    .As<IAsyncDocumentSession>()
            //    .OnActivated(e => e.Context.Resolve<ITransactionContext>().AddPersistentResource(e.Instance, e.Instance.SaveChangesAsync))
            //    .InstancePerHttpRequest();
        }
    }
}
