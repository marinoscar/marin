﻿using Luval.Media.Gallery.Entities;
using Microsoft.AspNetCore.Http;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Luval.Web.Common
{
    public class MicrosoftGraphCodeFlowHelper
    {

        private string _clientId;
        private string _clientSecrent;
        private int _state;
        private string _scopes;
        private HttpRequest _httpRequest;
        private string _apiUrl = "https://graph.microsoft.com/v1.0/";

        public MicrosoftGraphCodeFlowHelper(string clientId, string clientSecret, HttpRequest httpRequest) : this(clientId, clientSecret, "openid offline_access User.Read", httpRequest, 1983)
        {

        }

        public MicrosoftGraphCodeFlowHelper(string clientId, string clientSecret, string scopes, HttpRequest httpRequest) : this(clientId, clientSecret, scopes, httpRequest, 1983)
        {

        }

        public MicrosoftGraphCodeFlowHelper(string clientId, string clientSecret, string scopes, HttpRequest httpRequest, int state)
        {
            _clientId = clientId;
            _clientSecrent = clientSecret;
            _state = state;
            _scopes = scopes;
            _httpRequest = httpRequest;
        }



        public string GetCodeAuthorizationUrl(string returnPath)
        {
            var url = string.Format("https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id={0}&response_type=code&redirect_uri={1}&response_mode=query&scope={2}&state={3}"
                ,_clientId, GetReturnUrl(returnPath), HttpUtility.HtmlEncode(_scopes), _state);
            return url;
        }

        private string GetReturnUrl(string returnPath)
        {
            return string.Format("{0}://{1}/{2}", _httpRequest.Scheme, _httpRequest.Host, returnPath);
        }

        public async Task<IRestResponse<GraphTokenResponse>> GetCodeAuthorizationAsync(string code, string returnPath, CancellationToken cancellationToken)
        {
            var client = new RestClient("https://login.microsoftonline.com/common/oauth2/v2.0/");
            var request = new RestRequest("token");
            var returnUrl = GetReturnUrl(returnPath);
            request.AddParameter("client_id", _clientId, ParameterType.GetOrPost);
            request.AddParameter("scope", _scopes, ParameterType.GetOrPost);
            request.AddParameter("redirect_uri", returnUrl, ParameterType.GetOrPost);
            request.AddParameter("grant_type", "authorization_code", ParameterType.GetOrPost);
            request.AddParameter("client_secret", _clientSecrent, ParameterType.GetOrPost);
            request.AddParameter("code", code, ParameterType.GetOrPost);
            return await client.ExecutePostAsync<GraphTokenResponse>(request, cancellationToken);
        }

        public async Task<IRestResponse<GraphPrincipal>> GetPrincipalAsync(string authKey, CancellationToken cancellationToken)
        {
            var client = new RestClient(_apiUrl);
            var request = new RestRequest("me");
            request.AddHeader("Authorization", string.Format("Bearer {0}", authKey));
            return await client.ExecuteGetAsync<GraphPrincipal>(request, cancellationToken);
        }


        public async Task<GraphTokenResponse> GetTokenResponseAsync(string code, string returnPath, CancellationToken cancellationToken)
        {
            var tokenResponse = await GetCodeAuthorizationAsync(code, returnPath, cancellationToken);
            if (!tokenResponse.IsSuccessful) throw new HttpRequestException(string.Format("Unable to obtain token with error message: ", tokenResponse.StatusDescription), null, tokenResponse.StatusCode);
            var auth = tokenResponse.Data;
            var principalResponse = await GetPrincipalAsync(auth.AccessToken, cancellationToken);
            if(!principalResponse.IsSuccessful) throw new HttpRequestException(string.Format("Unable to obtain principal information: ", principalResponse.ErrorMessage), null, principalResponse.StatusCode);
            auth.UserPrincipalDisplayName = principalResponse.Data.DisplayName;
            auth.UserPrincipalName = principalResponse.Data.UserPrincipalName;
            auth.UserPrincipalMail = principalResponse.Data.Mail;
            return auth;
        }




    }
}
