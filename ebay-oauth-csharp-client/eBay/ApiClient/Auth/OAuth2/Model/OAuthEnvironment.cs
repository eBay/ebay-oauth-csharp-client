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
namespace eBay.ApiClient.Auth.OAuth2.Model
{
public class OAuthEnvironment
    {
        public static readonly OAuthEnvironment PRODUCTION = new OAuthEnvironment("api.ebay.com", "https://auth.ebay.com/oauth2/authorize", "https://api.ebay.com/identity/v1/oauth2/token");
        public static readonly OAuthEnvironment SANDBOX = new OAuthEnvironment("api.sandbox.ebay.com", "https://auth.sandbox.ebay.com/oauth2/authorize", "https://api.sandbox.ebay.com/identity/v1/oauth2/token");

        private readonly String configIdentifier;
        private readonly String webEndpoint;
        private readonly String apiEndpoint;

        private OAuthEnvironment(String configIdentifier, String webEndpoint, String apiEndpoint) {
            this.configIdentifier = configIdentifier;
            this.webEndpoint = webEndpoint;
            this.apiEndpoint = apiEndpoint;
        }

        public String ConfigIdentifier() {
            return configIdentifier;
        }

        public String WebEndpoint() {
            return webEndpoint;
        }

        public String ApiEndpoint() {
            return apiEndpoint;
        }

        /*
         * Lookup by ConfigIdentifier
         */
        public static OAuthEnvironment LookupByConfigIdentifier(String configIdentifier) {
            if(PRODUCTION.ConfigIdentifier().Equals(configIdentifier)) 
            {
                return PRODUCTION;
            } else if (SANDBOX.ConfigIdentifier().Equals(configIdentifier))
            {
                return SANDBOX;
            }
            return null;
        }

    }
}
