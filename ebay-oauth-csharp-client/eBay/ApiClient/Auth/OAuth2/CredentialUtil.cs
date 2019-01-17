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
using System.IO;
using eBay.ApiClient.Auth.OAuth2.Model;
using YamlDotNet.RepresentationModel;
using log4net;
using System.Collections.Concurrent;

namespace eBay.ApiClient.Auth.OAuth2
{
    public static class CredentialUtil {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static readonly ConcurrentDictionary<String, Credentials> envCredentials = new ConcurrentDictionary<String, Credentials>();

        public class Credentials {
            private readonly Dictionary<CredentialType, String> credentialTypeLookup = new Dictionary<CredentialType, String>();

            public Credentials(YamlMappingNode keyValuePairs)
            {

                foreach (var keyValuePair in keyValuePairs.Children)
                {
                    CredentialType credentialType = CredentialType.LookupByConfigIdentifier(keyValuePair.Key.ToString());
                    if (credentialType != null)
                    {
                        credentialTypeLookup.Add(credentialType, keyValuePair.Value.ToString());
                    }
                }
            }

            public String Get(CredentialType credentialType)
            {
                return credentialTypeLookup[credentialType];
            }
        }

        /*
         * Loading StreamReader
         */
        public static void Load(String yamlFile)
        {
            //Stream the input file
            StreamReader streamReader = new StreamReader(yamlFile);
            Load(streamReader);
        }

        /*
         * Loading YAML file
         */
        public static void Load(StreamReader streamReader)
        {
           
            //Load the stream
            YamlStream yaml = new YamlStream();
            yaml.Load(streamReader);

            // Parse the stream
            var rootNode = (YamlMappingNode)yaml.Documents[0].RootNode;
            foreach (var firstLevelNode in rootNode.Children)
            {
                OAuthEnvironment environment = OAuthEnvironment.LookupByConfigIdentifier(((YamlScalarNode)firstLevelNode.Key).Value);
                if (environment == null)
                {
                    continue;
                }

                foreach (var node in firstLevelNode.Value.AllNodes)
                {
                    if (node is YamlMappingNode)
                    {
                        Credentials credentials = new Credentials((YamlMappingNode)node);
                        envCredentials[environment.ConfigIdentifier()] = credentials;
                    }

                }

            }
            log.Info("Loaded configuration for eBay oAuth Token");

        }

        /*
         * Get Credentials based on Environment
         */
        public static Credentials GetCredentials(OAuthEnvironment environment) {

            return envCredentials.TryGetValue(environment.ConfigIdentifier(), out Credentials credentials) ? credentials : null;
        }

    }
}
