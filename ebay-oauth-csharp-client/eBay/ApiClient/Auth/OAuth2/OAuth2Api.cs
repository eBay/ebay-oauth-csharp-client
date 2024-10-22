/*
 * *
 *  * Copyright 2019 eBay Inc.
 *  *
 *  * Licensed under the Apache License, Version 2.0 (the "License");
 *  * you may not use this file except in compliance with the License.
 *  * You may obtain a copy of the License at
 *  *
 *  *  http://www.apache.org/licenses/LICENSE-2.0
 *  *
 *  * Unless required by applicable law or agreed to in writing, software
 *  * distributed under the License is distributed on an "AS IS" BASIS,
 *  * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  * See the License for the specific language governing permissions and
 *  * limitations under the License.
 *  *
 */

using System;
using System.Collections.Generic;
using eBay.ApiClient.Auth.OAuth2.Model;
using RestSharp;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using log4net;

namespace eBay.ApiClient.Auth.OAuth2
{
    public class OAuth2Api
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static AppTokenCache appTokenCache = new AppTokenCache();

        private class AppTokenCache
        {
            private Dictionary<String, AppTokenCacheModel> envAppTokenCache = new Dictionary<String, AppTokenCacheModel>();

            public void UpdateValue(OAuthEnvironment environment, OAuthResponse oAuthResponse, DateTime expiresAt)
            {
                AppTokenCacheModel appTokenCacheModel = new AppTokenCacheModel
                {
                    OAuthResponse = oAuthResponse,

                    //Setting a buffer of 5 minutes for refresh
                    ExpiresAt = expiresAt.Subtract(new TimeSpan(0, 5, 0))
                };
                
                //Remove key if it exists
                if(envAppTokenCache.ContainsKey(environment.ConfigIdentifier()))
                {
                    envAppTokenCache.Remove(environment.ConfigIdentifier());
                }
                
                envAppTokenCache.Add(environment.ConfigIdentifier(), appTokenCacheModel);
            }

            public OAuthResponse GetValue(OAuthEnvironment environment)
            {

                AppTokenCacheModel appTokenCacheModel = this.envAppTokenCache.TryGetValue(environment.ConfigIdentifier(), out AppTokenCacheModel value) ? value : null;

                if (appTokenCacheModel != null)
                {
                    if ((appTokenCacheModel.OAuthResponse != null && appTokenCacheModel.OAuthResponse.ErrorMessage != null)
                        || (DateTime.Now.CompareTo(appTokenCacheModel.ExpiresAt) < 0))
                        return appTokenCacheModel.OAuthResponse;
                }
                //Since the value is expired, return null
                return null;
            }
        }

        /*
         * Use this operation to get an OAuth access token using a client credentials grant. 
         * The access token retrieved from this process is called an Application access token. 
         */
        public OAuthResponse GetApplicationToken(OAuthEnvironment environment, IList<String> scopes)
        {

            //Validate request
            ValidateEnvironmentAndScopes(environment, scopes);
            OAuthResponse oAuthResponse = null;

            //Check for token in cache
            if (appTokenCache != null)
            {
                oAuthResponse = appTokenCache.GetValue(environment);
                if (oAuthResponse != null && oAuthResponse.AccessToken != null && oAuthResponse.AccessToken.Token != null)
                {
                    log.Info("Returning token from cache for " + environment.ConfigIdentifier());
                    return oAuthResponse;
                }
            }

            //App token not in cache, fetch it and set into cache
            String formattedScopes = OAuth2Util.FormatScopesForRequest(scopes);

            //Prepare request payload
            Dictionary<String, String> payloadParams = new Dictionary<string, string>
            {
                { Constants.PAYLOAD_GRANT_TYPE, Constants.PAYLOAD_VALUE_CLIENT_CREDENTIALS },
                {Constants.PAYLOAD_SCOPE, formattedScopes}
            };
            String requestPayload = OAuth2Util.CreateRequestPayload(payloadParams);

            oAuthResponse = FetchToken(environment, requestPayload, TokenType.APPLICATION);
            //Update value in cache
            if(oAuthResponse != null && oAuthResponse.AccessToken != null) 
            {
                appTokenCache.UpdateValue(environment, oAuthResponse, oAuthResponse.AccessToken.ExpiresOn);
            }

            return oAuthResponse;
        }

        /*
         * Use this operation to get the Authorization URL to redirect the user to. 
         * Once the user authenticates and approves the consent, the callback need to be 
         * captured by the redirect URL setup by the app 
         */
        public String GenerateUserAuthorizationUrl(OAuthEnvironment environment, IList<String> scopes, String state)
        {

            //Validate request
            ValidateEnvironmentAndScopes(environment, scopes);

            //Get credentials
            CredentialUtil.Credentials credentials = GetCredentials(environment);

            //Format scopes
            String formattedScopes = OAuth2Util.FormatScopesForRequest(scopes);

            //Prepare URL
            StringBuilder sb = new StringBuilder();
            sb.Append(environment.WebEndpoint()).Append("?");

            //Prepare request payload
            Dictionary<String, String> queryParams = new Dictionary<string, string>
            {
                { Constants.PAYLOAD_CLIENT_ID, credentials.Get(CredentialType.APP_ID) },
                {Constants.PAYLOAD_RESPONSE_TYPE, Constants.PAYLOAD_VALUE_CODE},
                {Constants.PAYLOAD_REDIRECT_URI, credentials.Get(CredentialType.REDIRECT_URI)},
                {Constants.PAYLOAD_SCOPE, formattedScopes}
            };

            if (state != null)
            {
                queryParams.Add(Constants.PAYLOAD_STATE, state);
            }

            sb.Append(OAuth2Util.CreateRequestPayload(queryParams));

            log.Debug("Authorization url " + sb);
            return sb.ToString();
        }

        /*
         * Use this operation to get the refresh and access tokens.
         */
        public OAuthResponse ExchangeCodeForAccessToken(OAuthEnvironment environment, String code)
        {

            //Validate request
            ValidateInput("Environment", environment);
            ValidateInput("Code", code);

            //Get credentials
            CredentialUtil.Credentials credentials = GetCredentials(environment);

            // Create request payload
            Dictionary<String, String> payloadParams = new Dictionary<string, string>
            {
                { Constants.PAYLOAD_GRANT_TYPE, Constants.PAYLOAD_VALUE_AUTHORIZATION_CODE },
                {Constants.PAYLOAD_REDIRECT_URI, credentials.Get(CredentialType.REDIRECT_URI)},
                {Constants.PAYLOAD_CODE, code}
            };
            String requestPayload = OAuth2Util.CreateRequestPayload(payloadParams);
           
            OAuthResponse oAuthResponse = FetchToken(environment, requestPayload, TokenType.USER);
            return oAuthResponse;
        }

        /*
         * Use this operation to update the access token if it has expired
         */
        public OAuthResponse GetAccessToken(OAuthEnvironment environment, String refreshToken, IList<String> scopes)
        {

            //Validate request
            ValidateEnvironmentAndScopes(environment, scopes);
            ValidateInput("RefreshToken", refreshToken);

            //Get credentials
            CredentialUtil.Credentials credentials = GetCredentials(environment);

            //Format scopes
            String formattedScopes = OAuth2Util.FormatScopesForRequest(scopes);

            // Create request payload
            Dictionary<String, String> payloadParams = new Dictionary<string, string>
            {
                { Constants.PAYLOAD_GRANT_TYPE, Constants.PAYLOAD_VALUE_REFRESH_TOKEN },
                {Constants.PAYLOAD_REFRESH_TOKEN, refreshToken},
                {Constants.PAYLOAD_SCOPE, formattedScopes}
            };
            String requestPayload = OAuth2Util.CreateRequestPayload(payloadParams);

            OAuthResponse oAuthResponse = FetchToken(environment, requestPayload, TokenType.USER);
            return oAuthResponse;
        }

        private void ValidateEnvironmentAndScopes(OAuthEnvironment environment, IList<String> scopes)
        {
            ValidateInput("Environment", environment);
            ValidateScopes(scopes);
        }

        private void ValidateInput(String key, Object value)
        {
            if (value == null)
            {
                throw new ArgumentException(key + " can't be null");
            }
        }

        private void ValidateScopes(IList<String> scopes)
        {
            if (scopes == null || scopes.Count == 0)
            {
                throw new ArgumentException("Scopes can't be null/empty");
            }
        }

        private OAuthResponse FetchToken(OAuthEnvironment environment, String requestPayload, TokenType tokenType)
        {
            //Get credentials
            CredentialUtil.Credentials credentials = GetCredentials(environment);

            //Initialize client
            RestClient client = new RestClient(environment.ApiEndpoint());

            //Create request
            RestRequest request = new RestRequest();

            // Request method
            request.Method = Method.Post;

            //Add headers
            request.AddHeader(Constants.HEADER_AUTHORIZATION, OAuth2Util.CreateAuthorizationHeader(credentials));

            //Set request payload
            request.AddParameter(Constants.HEADER_CONTENT_TYPE, requestPayload, ParameterType.RequestBody);


            //Call the API
            RestResponse response = client.Execute(request);

            //Parse response
            OAuthResponse oAuthResponse = HandleApiResponse(response, tokenType);

            return oAuthResponse;
        }


        private CredentialUtil.Credentials GetCredentials(OAuthEnvironment environment)
        {
            CredentialUtil.Credentials credentials = CredentialUtil.GetCredentials(environment);
            if (credentials == null)
            {
                throw new ArgumentException("Credentials have not been loaded for " + environment.ConfigIdentifier());
            }
            return credentials;
        }


        public OAuthResponse HandleApiResponse(RestResponse response, TokenType tokenType)
        {
            OAuthResponse oAuthResponse = new OAuthResponse();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                oAuthResponse.ErrorMessage = response.Content;
                log.Error("Error in fetching the token. Error:" + oAuthResponse.ErrorMessage);
            }
            else
            {
                OAuthApiResponse apiResponse = JsonConvert.DeserializeObject<OAuthApiResponse>(response.Content);

                //Set AccessToken
                OAuthToken accessToken = new OAuthToken
                {
                    Token = apiResponse.AccessToken,
                    ExpiresOn = DateTime.Now.Add(new TimeSpan(0, 0, apiResponse.ExpiresIn)),
                    TokenType = tokenType
                };
                oAuthResponse.AccessToken = accessToken;

                //Set Refresh Token
                if (apiResponse.RefreshToken != null)
                {
                    OAuthToken refreshToken = new OAuthToken
                    {
                        Token = apiResponse.RefreshToken,
                        ExpiresOn = DateTime.Now.Add(new TimeSpan(0, 0, apiResponse.RefreshTokenExpiresIn)),
                    };
                    oAuthResponse.RefreshToken = refreshToken;
                }
            }
            log.Info("Fetched the token successfully from API");
            return oAuthResponse;
        }
    }

}
