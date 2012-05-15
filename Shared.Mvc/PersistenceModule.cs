using System.Web.Mvc;
using Autofac;
using Highway.Shared.Mvc.Persistence;
using Highway.Shared.Persistence;

namespace Highway.Shared.Mvc
{
    public class PersistenceModuleBase : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<TransactedActionFilter>().As<IActionFilter>();
            builder.RegisterType<TransactionScopeTransactionContext>().As<ITransactionContext>().InstancePerLifetimeScope();
        }
    }
}
