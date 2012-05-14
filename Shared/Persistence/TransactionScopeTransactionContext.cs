using System;
using System.Collections.Generic;
using System.Transactions;
using Highway.Shared.Diagnostics;

namespace Highway.Shared.Persistence
{
    public class TransactionScopeTransactionContext : ITransactionContext
    {
        readonly ILog<TransactionScopeTransactionContext> _log;
        TransactionScope _currentTransactionScope;
        bool _abort;
        readonly IDictionary<object, Action> _persistentResources = new Dictionary<object, Action>();

        public TransactionScopeTransactionContext(ILog<TransactionScopeTransactionContext> log)
        {
            _log = log;
        }

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            if (_currentTransactionScope != null)
                throw new InvalidOperationException("A transaction is already active.");

            _log.Debug("Beginning transaction.");

            var options = new TransactionOptions();
            options.IsolationLevel = isolationLevel;
            _currentTransactionScope = new TransactionScope(TransactionScopeOption.Required, options);

            _abort = false;
        }

        public void EndTransaction()
        {
            if (_currentTransactionScope == null)
                throw new InvalidOperationException("There is no active transaction to end.");

            _log.Debug("Ending transaction.");

            try
            {
                if (_abort)
                {
                    _log.Debug("Transaction rolled back.");
                }
                else
                {
                    ExecutePreCommitActions();

                    _currentTransactionScope.Complete();
                    _log.Debug("Transaction committed.");
                }
            }
            finally
            {
                _currentTransactionScope.Dispose();
                _currentTransactionScope = null;
            }
        }

        void ExecutePreCommitActions()
        {
            foreach (var persistentResource in _persistentResources.Keys)
            {
                var preCommitAction = _persistentResources[persistentResource];
                if (preCommitAction != null) preCommitAction();
            }
        }

        public void AddPersistentResource(object resource, Action beforeTransactionCommits)
        {
            if (resource == null) throw new ArgumentNullException("resource");
            if (beforeTransactionCommits == null) throw new ArgumentNullException("beforeTransactionCommits");
            if (_persistentResources.ContainsKey(resource)) throw new ArgumentException("The resource has already been added.", "resource");

            _persistentResources.Add(resource, beforeTransactionCommits);
        }

        public void SetAbort()
        {
            if (_currentTransactionScope == null)
                throw new InvalidOperationException("There is no active transaction to abort.");

            _log.Debug("Abort flag is set.");
            _abort = true;
        }

        public void Dispose()
        {
            if (_currentTransactionScope == null)
                return;

            _log.Error("The active transaction was never comitted nor aborted, now rolling back.");
            _currentTransactionScope = null;
        }
    }
}
