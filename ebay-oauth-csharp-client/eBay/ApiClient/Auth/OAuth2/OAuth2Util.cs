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
using System.Text;
using System.Collections.Generic;
using eBay.ApiClient.Auth.OAuth2.Model;

namespace eBay.ApiClient.Auth.OAuth2
{
    public static class OAuth2Util
    {

        /*
         * Format scopes for request
         */
        public static String FormatScopesForRequest(IList<String> scopes) {
            String scopesForRequest = null;
            if(scopes == null || scopes.Count ==0) {
                return scopesForRequest;
            }

            foreach(String scope in scopes) {
                scopesForRequest = scopesForRequest == null ? scope : scopesForRequest + "+" + scope;
            }
            return scopesForRequest;
        } 

        /*
         * Create Base64 encoded Authorization header value
         */
        public static String CreateAuthorizationHeader(CredentialUtil.Credentials credentials) {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(credentials.Get(CredentialType.APP_ID)).Append(Constants.CREDENTIAL_DELIMITER);
            stringBuilder.Append(credentials.Get(CredentialType.CERT_ID));
            var plainTextBytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
            string encodedText = Convert.ToBase64String(plainTextBytes);
            return Constants.HEADER_PREFIX_BASIC + encodedText;
        }

        /*
         * Create request payload for input parameters and values
         */
        public static String CreateRequestPayload(Dictionary<String, String> payloadParams)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<String, String> entry in payloadParams)
            {
                if (sb.Length > 0)
                {
                    sb.Append(Constants.PAYLOAD_PARAM_DELIMITER);
                }
                sb.Append(entry.Key).Append(Constants.PAYLOAD_VALUE_DELIMITER).Append(entry.Value);

            }
            return sb.ToString();
        }
    }
}