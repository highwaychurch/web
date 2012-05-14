/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Highway.Identity.Web.ViewModels.Administration
{
    public class GlobalConfigurationViewModel
    {
        public string Id { get; set; }

        [Required]
        [DisplayName("Site Name")]
        public string SiteName { get; set; }

        [Required]
        [DisplayName("Issuer URI")]
        public string IssuerUri { get; set; }

        [Required]
        [DisplayName("Issuer Contact Email")]
        public string IssuerContactEmail { get; set; }

        [Required]
        [DisplayName("Default Token Type")]
        public string DefaultTokenType { get; set; }

        [Required]
        [DisplayName("Default Token Lifetime (in hours)")]
        public int DefaultTokenLifetime { get; set; }

        [Required]
        [DisplayName("Maximum Token Lifetime (in hours)")]
        public int MaximumTokenLifetime { get; set; }

        [Required]
        [DisplayName("Single Sign-On Cookie Lifetime (in hours)")]
        public int SsoCookieLifetime { get; set; }

        [Required]
        [DisplayName("Require SSL")]
        public bool RequireSsl { get; set; }

        [Required]
        [DisplayName("Require Encryption")]
        public bool RequireEncryption { get; set; }

        [Required]
        [DisplayName("Enable Client Certificates")]
        public bool EnableClientCertificates { get; set; }

        [Required]
        [DisplayName("Enable Identity Delegation")]
        public bool EnableDelegation { get; set; }

        [Required]
        [DisplayName("Only allow known Realms")]
        public bool AllowKnownRealmsOnly { get; set; }

        [Required]
        [DisplayName("Allow Reply To Parameter")]
        public bool AllowReplyTo { get; set; }

        [Required]
        [DisplayName("Require Reply To within Realm")]
        public bool RequireReplyToWithinRealm { get; set; }

        [Required]
        [DisplayName("Enable Strong Endpoint Identities for WS-Trust SSL Endpoints")]
        public bool EnableStrongEpiForSsl { get; set; }

        [Required]
        [DisplayName("Enable Tracing of Federation Messages (RST, RSTR, issued token)")]
        public bool EnableFederationMessageTracing { get; set; }

        [Required]
        [DisplayName("Enforce 'IdentityServerUsers' role for clients")]
        public bool EnforceUsersGroupMembership { get; set; }

        [Required]
        [DisplayName("Require Sign In Confirmation")]
        public bool RequireSignInConfirmation { get; set; }
    }
}