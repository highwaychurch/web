/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

namespace Highway.Identity.Core.Repositories
{
    public interface ICacheRepository
    {
        void Put(string name, object value, int ttl);
        object Get(string name);
        void Invalidate(string name);
    }
}
