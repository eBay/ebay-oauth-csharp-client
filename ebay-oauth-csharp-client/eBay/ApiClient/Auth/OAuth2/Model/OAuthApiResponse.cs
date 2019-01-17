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
using Newtonsoft.Json;
using RestSharp.Deserializers;

namespace eBay.ApiClient.Auth.OAuth2.Model
{
    public class OAuthApiResponse
    {
        [JsonProperty(PropertyName = "access_token")]
        public String AccessToken { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty(PropertyName = "refresh_token")]
        public String RefreshToken { get; set; }

        [JsonProperty(PropertyName = "refresh_token_expires_in")]
        public int RefreshTokenExpiresIn { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public String TokenType { get; set; }

        [JsonProperty(PropertyName = "errorMessage")]
        public String ErrorMessage { get; set; }
    }
}