// <copyright file="AuthTokenTest.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Auth;

namespace DxFeed.Graal.Net.Tests.Auth;

[TestFixture]
public class AuthTokenTest
{
    [Test]
    public void ValueOf_ShouldReturnAuthToken_WhenValidStringProvided()
    {
        const string scheme = "Basic";
        const string token = "dXNlcjpwYXNzd29yZA==";
        const string str = $"{scheme} {token}";
        var authToken = AuthToken.ValueOf(str);

        Assert.That(authToken, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(authToken.Scheme, Is.EqualTo("Basic"));
            Assert.That(authToken.Value, Is.EqualTo("dXNlcjpwYXNzd29yZA=="));
            Assert.That(authToken.User, Is.EqualTo("user"));
            Assert.That(authToken.Password, Is.EqualTo("password"));
            Assert.That(authToken.HttpAuthorization, Is.EqualTo(str));
            Assert.That(authToken.ToString(), Is.EqualTo(str));
        });
    }

    [Test]
    public void CreateBasicToken_ShouldReturnAuthToken_WhenValidUserPasswordProvided()
    {
        const string scheme = "Basic";
        const string userPassword = "user:password";
        const string token = "dXNlcjpwYXNzd29yZA==";
        const string str = $"{scheme} {token}";
        var authToken = AuthToken.CreateBasicToken(userPassword);

        Assert.That(authToken, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(authToken.Scheme, Is.EqualTo("Basic"));
            Assert.That(authToken.Value, Is.EqualTo(token));
            Assert.That(authToken.User, Is.EqualTo("user"));
            Assert.That(authToken.Password, Is.EqualTo("password"));
            Assert.That(authToken.HttpAuthorization, Is.EqualTo(str));
            Assert.That(authToken.ToString(), Is.EqualTo(str));
        });
    }

    [Test]
    public void CreateBasicToken_ShouldReturnAuthToken_WhenValidUserAndPasswordProvided()
    {
        const string scheme = "Basic";
        const string user = "user";
        const string password = "password";
        const string token = "dXNlcjpwYXNzd29yZA==";
        const string str = $"{scheme} {token}";
        var authToken = AuthToken.CreateBasicToken(user, password);

        Assert.That(authToken, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(authToken.Scheme, Is.EqualTo("Basic"));
            Assert.That(authToken.Value, Is.EqualTo(token));
            Assert.That(authToken.User, Is.EqualTo("user"));
            Assert.That(authToken.Password, Is.EqualTo("password"));
            Assert.That(authToken.HttpAuthorization, Is.EqualTo(str));
            Assert.That(authToken.ToString(), Is.EqualTo(str));
        });
    }

    [Test]
    public void CreateBasicTokenOrNull_ShouldReturnNull_WhenBothUserAndPasswordAreNullOrEmpty()
    {
        var authToken = AuthToken.CreateBasicTokenOrNull(null, null);
        Assert.That(authToken, Is.Null);

        authToken = AuthToken.CreateBasicTokenOrNull("", "");
        Assert.That(authToken, Is.Null);
    }

    [Test]
    public void CreateBasicTokenOrNull_ShouldReturnAuthToken_WhenValidUserAndPasswordProvided()
    {
        const string scheme = "Basic";
        const string user = "user";
        const string password = "password";
        const string token = "dXNlcjpwYXNzd29yZA==";
        const string str = $"{scheme} {token}";
        var authToken = AuthToken.CreateBasicTokenOrNull(user, password);

        Assert.That(authToken, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(authToken.Scheme, Is.EqualTo("Basic"));
            Assert.That(authToken.Value, Is.EqualTo(token));
            Assert.That(authToken.User, Is.EqualTo("user"));
            Assert.That(authToken.Password, Is.EqualTo("password"));
            Assert.That(authToken.HttpAuthorization, Is.EqualTo(str));
            Assert.That(authToken.ToString(), Is.EqualTo(str));
        });
    }

    [Test]
    public void CreateBearerToken_ShouldReturnAuthToken_WhenValidTokenProvided()
    {
        const string scheme = "Bearer";
        const string token = "access_token";
        const string str = $"{scheme} {token}";
        var authToken = AuthToken.CreateBearerToken(token);

        Assert.That(authToken, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(authToken.Scheme, Is.EqualTo("Bearer"));
            Assert.That(authToken.Value, Is.EqualTo(token));
            Assert.That(authToken.User, Is.Null);
            Assert.That(authToken.Password, Is.Null);
            Assert.That(authToken.HttpAuthorization, Is.EqualTo(str));
            Assert.That(authToken.ToString(), Is.EqualTo(str));
        });
    }

    [Test]
    public void CreateBearerTokenOrNull_ShouldReturnNull_WhenTokenIsNullOrEmpty()
    {
        var authToken = AuthToken.CreateBearerTokenOrNull(null);
        Assert.That(authToken, Is.Null);

        authToken = AuthToken.CreateBearerTokenOrNull("");
        Assert.That(authToken, Is.Null);
    }

    [Test]
    public void CreateBearerTokenOrNull_ShouldReturnAuthToken_WhenValidTokenProvided()
    {
        const string scheme = "Bearer";
        const string token = "access_token";
        const string str = $"{scheme} {token}";
        var authToken = AuthToken.CreateBearerTokenOrNull(token);

        Assert.That(authToken, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(authToken.Scheme, Is.EqualTo("Bearer"));
            Assert.That(authToken.Value, Is.EqualTo(token));
            Assert.That(authToken.User, Is.Null);
            Assert.That(authToken.Password, Is.Null);
            Assert.That(authToken.HttpAuthorization, Is.EqualTo(str));
            Assert.That(authToken.ToString(), Is.EqualTo(str));
        });
    }

    [Test]
    public void CreateCustomToken_ShouldReturnAuthToken_WhenValidSchemeAndValueProvided()
    {
        const string scheme = "CustomScheme";
        const string token = "custom_value";
        const string str = $"{scheme} {token}";
        var authToken = AuthToken.CreateCustomToken(scheme, token);

        Assert.That(authToken, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(authToken.Scheme, Is.EqualTo(scheme));
            Assert.That(authToken.Value, Is.EqualTo(token));
            Assert.That(authToken.User, Is.Null);
            Assert.That(authToken.Password, Is.Null);
            Assert.That(authToken.HttpAuthorization, Is.EqualTo(str));
            Assert.That(authToken.ToString(), Is.EqualTo(str));
        });
    }

    [Test]
    public void Equals_ShouldReturnTrue_WhenTokensAreEqual()
    {
        var token1 = AuthToken.CreateBasicToken("user", "password");
        var token2 = AuthToken.CreateBasicToken("user", "password");

        Assert.Multiple(() =>
        {
            Assert.That(token1, Is.EqualTo(token2));
            Assert.That(token1.GetHashCode(), Is.EqualTo(token2.GetHashCode()));
        });
    }

    [Test]
    public void Equals_ShouldReturnFalse_WhenTokensAreNotEqual()
    {
        var token1 = AuthToken.CreateBasicToken("user1", "password1");
        var token2 = AuthToken.CreateBasicToken("user2", "password2");

        Assert.Multiple(() =>
        {
            Assert.That(token1, Is.Not.EqualTo(token2));
            Assert.That(token1.GetHashCode(), Is.Not.EqualTo(token2.GetHashCode()));
        });
    }

    [Test]
    public void ToString_ShouldReturnStringRepresentation()
    {
        var authToken = AuthToken.CreateBasicToken("user", "password");
        const string str = "Basic dXNlcjpwYXNzd29yZA==";

        Assert.That(authToken.ToString(), Is.EqualTo(str));
    }
}
