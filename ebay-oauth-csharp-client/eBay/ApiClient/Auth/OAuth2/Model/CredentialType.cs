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
public class CredentialType
    {

        public static readonly CredentialType DEV_ID = new CredentialType(Constants.DEV_ID);
        public static readonly CredentialType APP_ID = new CredentialType(Constants.APP_ID);
        public static readonly CredentialType CERT_ID = new CredentialType(Constants.CERT_ID);
        public static readonly CredentialType REDIRECT_URI = new CredentialType(Constants.REDIRECT_URI);


        private readonly String configIdentifier;

        private CredentialType(String configIdentifier) 
        {
            this.configIdentifier = configIdentifier;
        }

        public String ConfigIdentifier()
        {
            return configIdentifier;
        }

        /*
         * Lookup CredentialType by config identifier
         */ 
        public static CredentialType LookupByConfigIdentifier(String configIdentifier) 
        {
            if(Constants.DEV_ID.Equals(configIdentifier)) 
            {
                return DEV_ID;
            } else if (Constants.APP_ID.Equals(configIdentifier))
            {
                return APP_ID;
            } else if (Constants.CERT_ID.Equals(configIdentifier))
            {
                return CERT_ID;
            } else if (Constants.REDIRECT_URI.Equals(configIdentifier))
            {
                return REDIRECT_URI;
            }
            return null;
        }

    }
}
