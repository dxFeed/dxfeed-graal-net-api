// <copyright file="AuthToken.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Native.Auth;
using DxFeed.Graal.Net.Native.ErrorHandling;

namespace DxFeed.Graal.Net.Auth;

/// <summary>
/// The <c>AuthToken</c> class represents an authorization token and encapsulates information
/// about the authorization scheme and its associated value.
///
/// <p/>An AuthToken consists of the following components:
/// <ul>
///   <li>Scheme - The authorization scheme (e.g., "Basic" or "Bearer").</li>
///   <li>
///     Value - The encoded value, which is scheme-dependent (e.g., an access token per RFC6750
///     or Base64-encoded "user:password" per RFC2617).
///   </li>
///   <li>
///     String representation - A string that combines the scheme and value in the format: [scheme + " " + value].
///   </li>
/// </ul>
/// </summary>
public class AuthToken
{
    /// <summary>
    /// The Basic Authentication Scheme.
    /// </summary>
    public const string BasicScheme = "Basic";

    /// <summary>
    /// The Bearer Authentication (token authentication) Scheme.
    /// </summary>
    public const string BearerScheme = "Bearer";

    private readonly AuthTokenHandle handle;

    private AuthToken(AuthTokenHandle handle) =>
        this.handle = handle;

    /// <summary>
    /// Gets the HTTP authorization header value.
    /// </summary>
    public string HttpAuthorization =>
        handle.GetHttpAuthorization();

    /// <summary>
    /// Gets the username or <c>null</c> if it is not known or applicable.
    /// </summary>
    public string? User =>
        handle.GetUser();

    /// <summary>
    /// Gets the password or <c>null</c> if it is not known or applicable.
    /// </summary>
    public string? Password =>
        handle.GetPassword();

    /// <summary>
    /// Gets the authentication scheme.
    /// </summary>
    public string Scheme =>
        handle.GetScheme();

    /// <summary>
    /// Gets the access token for RFC6750 or the Base64-encoded "username:password" for RFC2617.
    /// </summary>
    public string Value =>
        handle.GetValue();

    /// <summary>
    /// Constructs an <see cref="AuthToken"/> from the specified string.
    /// </summary>
    /// <param name="str">The string with space-separated scheme and value.</param>
    /// <returns>The constructed <see cref="AuthToken"/>.</returns>
    /// <exception cref="JavaException">
    /// If the string is malformed, or if the scheme is "Basic" but the format does not comply with RFC2617.
    /// </exception>
    public static AuthToken ValueOf(string str) =>
        new(AuthTokenHandle.ValueOf(str));

    /// <summary>
    /// Constructs an <see cref="AuthToken"/> with the specified username and password per RFC2617.
    /// Username and password can be empty.
    /// </summary>
    /// <param name="userPassword">
    /// The string containing the username and password in the format <c>username:password</c>.
    /// </param>
    /// <returns>The constructed <see cref="AuthToken"/>.</returns>
    /// <exception cref="JavaException">If the userPassword is malformed.</exception>
    public static AuthToken CreateBasicToken(string userPassword) =>
        new(AuthTokenHandle.CreateBasicToken(userPassword));

    /// <summary>
    /// Constructs an <see cref="AuthToken"/> with the specified username and password per RFC2617.
    /// Username and password can be empty.
    /// </summary>
    /// <param name="user">The username/</param>
    /// <param name="password">The password.</param>
    /// <returns>The constructed <see cref="AuthToken"/>.</returns>
    public static AuthToken CreateBasicToken(string user, string password) =>
        new(AuthTokenHandle.CreateBasicToken(user, password));

    /// <summary>
    /// Constructs an <see cref="AuthToken"/> with the specified username and password per RFC2617.
    /// If both the username and password are empty or <c>null</c>, returns <c>null</c>.
    /// </summary>
    /// <param name="user">The username.</param>
    /// <param name="password">The password.</param>
    /// <returns>The constructed <see cref="AuthToken"/> or <c>null</c>.</returns>
    public static AuthToken? CreateBasicTokenOrNull(string? user, string? password)
    {
        var handle = AuthTokenHandle.CreateBasicTokenOrNull(user, password);
        return handle == null ? null : new AuthToken(handle);
    }

    /// <summary>
    /// Constructs an <see cref="AuthToken"/> with the specified bearer token per RFC6750.
    /// </summary>
    /// <param name="token">The access token.</param>
    /// <returns>The constructed <see cref="AuthToken"/>.</returns>
    /// <exception cref="JavaException">If the token is empty.</exception>
    public static AuthToken CreateBearerToken(string token) =>
        new(AuthTokenHandle.CreateBearerToken(token));

    /// <summary>
    /// Constructs an <see cref="AuthToken"/> with the specified bearer token per RFC6750.
    /// </summary>
    /// <param name="token">The access token.</param>
    /// <returns>The constructed <see cref="AuthToken"/> or <c>null</c>.</returns>
    public static AuthToken? CreateBearerTokenOrNull(string? token)
    {
        var handle = AuthTokenHandle.CreateBearerTokenOrNull(token);
        return handle == null ? null : new AuthToken(handle);
    }

    /// <summary>
    /// Constructs an <see cref="AuthToken"/> with a custom scheme and value.
    /// </summary>
    /// <param name="scheme">The custom scheme.</param>
    /// <param name="value">The custom value.</param>
    /// <returns>The constructed <see cref="AuthToken"/>.</returns>
    /// <exception cref="JavaException">If the scheme or value is empty.</exception>
    public static AuthToken CreateCustomToken(string scheme, string value) =>
        new(AuthTokenHandle.CreateCustomToken(scheme, value));

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) =>
        obj is AuthToken token && handle.Equals(token.handle);

    /// <summary>
    /// Returns a hash code value for this object.
    /// </summary>
    /// <returns>A hash code value for this object.</returns>
    public override int GetHashCode() =>
        handle.GetHashCode();

    /// <summary>
    /// Returns string representation of this token.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        handle.ToString();
}
