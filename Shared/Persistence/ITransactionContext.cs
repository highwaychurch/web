using System;
using System.Transactions;

namespace Highway.Shared.Persistence
{
    public interface ITransactionContext
    {
        void BeginTransaction(IsolationLevel isolationLevel);
        void SetAbort();
        void EndTransaction();
        void AddPersistentResource(object resource, Action beforeTransactionCommits);
    }
}