/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using Highway.Identity.Core.Repositories;

namespace Highway.Identity.Core.Helper
{
    public static class Cache
    {
        public static T ReturnFromCache<T>(ICacheRepository cacheRepository, string name, int ttl, Func<T> action)   
            where T: class
        {
            var item = cacheRepository.Get(name) as T;
            if (item == null)
            {
                item = action();
                cacheRepository.Put(name, item, ttl);
            }

            return item;
        }
    }
}
