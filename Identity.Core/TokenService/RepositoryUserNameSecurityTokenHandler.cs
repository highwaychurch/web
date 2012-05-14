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

namespace Highway.Identity.Core.TokenService
{
    public class RepositoryUserNameSecurityTokenHandler : GenericUserNameSecurityTokenHandler
    {
        public static Func<IUserRepository> UserRepositoryFactoryMethod { get; set; } 

        protected override bool ValidateUserNameCredentialCore(string userName, string password)
        {
            var userRespository = UserRepositoryFactoryMethod();
            return userRespository.ValidateUser(userName, password);
        }
    }
}
