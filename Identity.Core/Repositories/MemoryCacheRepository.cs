/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Runtime.Caching;

namespace Highway.Identity.Core.Repositories
{
    public class MemoryCacheRepository : ICacheRepository
    {
        static MemoryCache _cache = new MemoryCache("Highway.Identity.Caching");
        
        public void Put(string name, object value, int ttl)
        {
            Tracing.Tracing.Verbose(String.Format("Adding {0} to cache", name));
            _cache.Add(name, value, DateTimeOffset.Now.AddHours(ttl));
        }

        public object Get(string name)
        {
            var item = _cache.Get(name);
            Tracing.Tracing.Verbose(String.Format("Fetching {0} from cache: {1}", name, item == null ? "miss" : "hit"));

            return item;
        }

        public void Invalidate(string name)
        {
            Tracing.Tracing.Verbose(String.Format("Invalidating {0} in cache", name));
            _cache.Remove(name);
        }
    }
}
