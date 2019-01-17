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
using Xunit;
using System.Collections.Generic;
using eBay.ApiClient.Auth.OAuth2.Model;
using System.IO;

namespace eBay.ApiClient.Auth.OAuth2
{
    public class OAuth2UtilTest
    {

        [Fact]
        public void FormatScopesForRequest_Success()
        {
            IList<String> scopes = new List<String>()
            {
                "https://api.ebay.com/oauth/api_scope/buy.marketing",
                "https://api.ebay.com/oauth/api_scope"
            };

            String formattedScopes = OAuth2Util.FormatScopesForRequest(scopes);
            Assert.Equal("https://api.ebay.com/oauth/api_scope/buy.marketing+https://api.ebay.com/oauth/api_scope", formattedScopes);
        }

        [Fact]
        public void FormatScopesForRequest_NullScopes()
        {
            String formattedScopes = OAuth2Util.FormatScopesForRequest(null);
            Assert.Null(formattedScopes);
        }

        [Fact]
        public void CreateAuthorizationHeader_Success() {
            String path = @"../../../ebay-config-sample.yaml";
            CredentialUtil.Load(path);
            CredentialUtil.Credentials credentials = CredentialUtil.GetCredentials(OAuthEnvironment.PRODUCTION);
            String authorizationHeader = OAuth2Util.CreateAuthorizationHeader(credentials);
            Assert.NotNull(authorizationHeader);
            Boolean headerStartsWithBasic = authorizationHeader.StartsWith("Basic ", StringComparison.Ordinal);
            Assert.True(headerStartsWithBasic);
        }

        [Fact]
        public void CreateRequestPayload_Success()
        {
            Dictionary<String, String> payloadParams = new Dictionary<string, string>
            {
                { Constants.PAYLOAD_GRANT_TYPE, Constants.PAYLOAD_VALUE_AUTHORIZATION_CODE },
                {Constants.PAYLOAD_REDIRECT_URI, "TestURI" },
                {Constants.PAYLOAD_CODE, "TestCode"}
            };
            String requestPayload = OAuth2Util.CreateRequestPayload(payloadParams);
            Assert.NotNull(requestPayload);
            Assert.Equal("grant_type=authorization_code&redirect_uri=TestURI&code=TestCode", requestPayload);
        }

    }
}