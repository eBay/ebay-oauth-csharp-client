# Summary

To make integrations with eBay RESTful APIs easier, eBay provides client
libraries in C# and Java to make it simpler to set up authorization,
reduce the amount of code the application developers have to write to
get OAuth Access Tokens. This library in addition to functioning as a
simple eBay OAuth Client, helps with additional features such as cached
App tokens.

# What is OAuth 2.0

[OAuth 2.0](https://tools.ietf.org/html/rfc6749) is the
industry-standard protocol for authorization to obtain limited access to
an HTTP service. All [eBay RESTful
APIs](https://developer.ebay.com/docs) use the OAuth 2.0 protocol for
authorization. OAuth access tokens verify to eBay that a request is
coming from a valid client and that the application has the user’s
authorization to carry out the requests. Learn more about the [OAuth
Access
Tokens](https://developer.ebay.com/api-docs/static/oauth-tokens.html).

# Supported Languages

eBay OAuth Client is a class library that targets the .NET Standard 2.0.
This library can be used by any .NET implementation that supports 2.0
version of the .NET Standard.

# Add the eBay.OAuth.Client NuGet Package

**Current Version** : 2.0.2

Use of this source code is governed by [Apache-2.0
license](https://opensource.org/licenses/Apache-2.0).If you’re looking
for the latest stable version (2.0.2), you can grab it directly from
NuGet.org.

``` xml
https://www.nuget.org/packages/eBay.OAuth.Client
```

## NuGet Package Manager UI

- In **Solution Explorer**, right-click NuGet in .csproj and choose
  **Add Package**.

- Search for **eBay.OAuth.Client**, select that package in the list, and
  click on **Add Package**

- **Accept** the License prompt

## Package Manager Console

- Use the following command in your project directory, to install the
  **eBay.OAuth.Client** package:

``` xml
Install-Package eBay.OAuth.Client -Version 2.0.2
```

- After the command completes, open the **.csproj** file to see the
  added reference:

``` xml
<ItemGroup>
   <PackageReference Include="eBay.OAuth.Client" Version="2.0.2" />
</ItemGroup>
```

## .NET CLI

- Use the following command in your project directory, to install the
  **eBay.OAuth.Client** package:

``` xml
dotnet add package eBay.OAuth.Client --version 2.0.2
```

- After the command completes, open the **.csproj** file to see the
  added reference:

``` xml
<ItemGroup>
   <PackageReference Include="eBay.OAuth.Client" Version="2.0.2" />
</ItemGroup>
```

## Paket CLI

- Use the following command in your project directory, to install the
  **eBay.OAuth.Client** package:

``` xml
paket add eBay.OAuth.Client --version 2.0.2
```

- After the command completes, open the **.csproj** file to see the
  added reference:

``` xml
<ItemGroup>
   <PackageReference Include="eBay.OAuth.Client" Version="2.0.2" />
</ItemGroup>
```

# Build eBay.OAuth.Client DLL from Source

- After cloning the project, you can build the package from the source
  by executing following script from project directory:

<!-- -->

- **ebay-oauth-csharp-client.dll** will be created at
  ebay-oauth-csharp-client/bin/Debug/net6.0

# Use the eBay.OAuth.Client in the .NET App

- Create a config [YAML](http://yaml.org/) file in your application. The
  config file should contain your eBay applications keys: App Id, Cert
  Id & Dev Id. A sample config file is available at
  <https://github.com/ebay/ebay-oauth-csharp-client/blob/master/Tests/ebay-config-sample.yaml>.
  Learn more about [creating application
  keys](https://developer.ebay.com/api-docs/static/creating-edp-account.html#Register).

``` csharp
name: ebay-config
api.sandbox.ebay.com:
    appid: <appid-from-developer-portal>
    certid: <certid-from-developer-portal>
    devid: <devid-from-developer-portal>
    redirecturi: <redirect_uri-from-developer-portal>
Api.ebay.com:
    appid: <appid-from-developer-portal>
    certid: <certid-from-developer-portal>
    devid: <devid-from-developer-portal>
    redirecturi: <redirect_uri-from-developer-portal>
```

- Once the config file is ready, use following code to load it. It is
  recommended to load the credentials during startup time
  (initialization) to prevent runtime delays.

``` csharp
CredentialUtil.Load(“YAML config file path”);
or
CredentialUtil.Load(System.IO.StreamReader);
```

- Once the credentials are loaded, call any operation on **OAuth2Api**.

  1.  **GetApplicationToken**: Use this operation when the application
      request an access token to access their own resources, not on
      behalf of a user. Learn more about [Client Credentials grant
      flow](https://developer.ebay.com/api-docs/static/oauth-client-credentials-grant.html).

``` csharp
OAuth2Api.GetApplicationToken(OAuthEnvironment environment, IList<String> scopes)
```

1.  **GenerateUserAuthorizationUrl**: Use this operation to get the
    Authorization URL to redirect the user to. Once the user
    authenticates and approves the consent, the callback needs to be
    captured by the redirect URL setup by the app.

``` csharp
OAuth2Api.GenerateUserAuthorizationUrl(OAuthEnvironment environment, IList<String> scopes, String state)
```

1.  **ExchangeCodeForAccessToken**: Use this operation when an
    application exchanges an authorization code for an access token.
    After the user authenticates, approves the consent and returns to
    the application via the redirect URL , the application will get the
    authorization code from the URL and use it to request an access
    token. Learn more about [Authorization Code grant
    flow](https://developer.ebay.com/api-docs/static/oauth-authorization-code-grant.html).

``` csharp
OAuth2Api.ExchangeCodeForAccessToken(OAuthEnvironment environment, String code)
```

1.  **GetAccessToken**: Usually access tokens are short lived. Use this
    operation to update the access token. Learn more about [Using a
    refresh token to update the access
    token](https://developer.ebay.com/api-docs/static/oauth-qref-auth-code-grant.html).

``` csharp
OAuth2Api.GetAccessToken(OAuthEnvironment environment, String refreshToken, IList<String> scopes)
```

# Contribution

Contributions in terms of patches, features, or comments are always
welcome. Refer to [CONTRIBUTING](CONTRIBUTING.adoc) for guidelines.
Submit Github issues for any feature enhancements, bugs, or
documentation problems as well as questions and comments.

# License

Copyright (c) 2018-2019 eBay Inc.

Use of this source code is governed by a Apache 2.0 license that can be
found in the LICENSE file or at
<https://opensource.org/licenses/Apache-2.0>.
