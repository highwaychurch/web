using System;

namespace Highway.Shared.Persistence
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NoTransactionAttribute : Attribute
    {
    }
}