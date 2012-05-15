using System;
using System.Web;
using Hammock;
using Hammock.Authentication.OAuth;

namespace F1OAuthTest.Integration.FellowshipOne
{
    public class F1AuthorizationHelper
    {
        private Token _existingRequestToken;
        private Token _existingAccessToken;
        private readonly HttpResponseBase _response;
        private readonly string _churchCode;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;

        public F1AuthorizationHelper(HttpRequestBase request, HttpResponseBase response, string churchCode, string consumerKey, string consumerSecret)
        {
            _response = response;
            _churchCode = churchCode;
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;

            request.Cookies.TryGetFromCookie("F1RequestToken", out _existingRequestToken);
            request.Cookies.TryGetFromCookie("F1AccessToken", out _existingAccessToken);
        }

        public Token ExistingRequsetToken
        {
            get { return _existingRequestToken; }
        }

        public Token ExistingAccessToken
        {
            get { return _existingAccessToken; }
        }

        public Token RequestRequestToken()
        {
            var creds = new OAuthCredentials
            {
                Type = OAuthType.RequestToken,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                ConsumerKey = _consumerKey,
                ConsumerSecret = _consumerSecret
            };

            var client = new RestClient
            {
                Authority = string.Format(URL.F1BaseUrl, _churchCode),
                VersionPath = "v1",
                Credentials = creds
            };

            var request = new RestRequest
            {
                Path = "Tokens/RequestToken"
            };

            var response = client.Request(request);

            var queryString = HttpUtility.ParseQueryString(response.Content);
            _existingRequestToken = new Token(queryString["oauth_token"], queryString["oauth_token_secret"]);
            _response.Cookies.SaveToCookie("F1RequestToken", _existingRequestToken);
            return _existingRequestToken;
        }

        public Token RequestAccessToken()
        {
            if (ExistingRequsetToken == null) throw new InvalidOperationException("Cannot request an Access token until you have requested a Request token.");

            var creds = new OAuthCredentials
            {
                Type = OAuthType.AccessToken,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                ConsumerKey = _consumerKey,
                ConsumerSecret = _consumerSecret,
                Token = ExistingRequsetToken.Value,
                TokenSecret = ExistingRequsetToken.Secret
            };

            var client = new RestClient
            {
                Authority = string.Format(URL.F1BaseUrl, _churchCode),
                VersionPath = "v1",
                Credentials = creds
            };

            var request = new RestRequest
            {
                Path = "Tokens/AccessToken"
            };

            var response = client.Request(request);

            var queryString = HttpUtility.ParseQueryString(response.Content);
            _existingAccessToken = new Token(queryString["oauth_token"], queryString["oauth_token_secret"]);
            _response.Cookies.SaveToCookie("F1AccessToken", _existingAccessToken);
            return _existingAccessToken;
        }
    }
}