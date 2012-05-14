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
    public class EndpointsConfigurationViewModel
    {
        public string Id { get; set; }
        [Required]
        [DisplayName("WS-Federation")]
        public bool WSFederation { get; set; }

        [Required]
        [DisplayName("WS-Trust Message Security")]
        public bool WSTrustMessage { get; set; }

        [Required]
        [DisplayName("WS-Trust MixedMode Security")]
        public bool WSTrustMixed { get; set; }

        [Required]
        [DisplayName("OAuth WRAP")]
        public bool OAuthWRAP { get; set; }

        [Required]
        [DisplayName("OAuth2")]
        public bool OAuth2 { get; set; }

        [Required]
        [DisplayName("JSNotify")]
        public bool JSNotify { get; set; }

        [Required]
        [DisplayName("Simple HTTP GET Endpoint")]
        public bool SimpleHttp { get; set; }

        [Required]
        [DisplayName("Federation Metadata")]
        public bool FederationMetadata { get; set; }

        [Required]
        [DisplayName("HTTP Port")]
        public int HttpPort { get; set; }

        [Required]
        [DisplayName("HTTPS Port")]
        public int HttpsPort { get; set; }
    }
}