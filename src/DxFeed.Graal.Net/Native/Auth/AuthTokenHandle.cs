// <copyright file="AuthTokenHandle.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Auth;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by marshaler")]
internal sealed class AuthTokenHandle : JavaHandle
{
    public static AuthTokenHandle ValueOf(string str) =>
        ErrorCheck.SafeCall(Import.ValueOf(CurrentThread, str));

    public static AuthTokenHandle CreateBasicToken(string userPassword) =>
        ErrorCheck.SafeCall(Import.CreateBasicToken(CurrentThread, userPassword));

    public static AuthTokenHandle CreateBasicToken(string user, string password) =>
        ErrorCheck.SafeCall(Import.CreateBasicToken(CurrentThread, user, password));

    public static AuthTokenHandle? CreateBasicTokenOrNull(string? user, string? password)
    {
        var handle = ErrorCheck.SafeCall(Import.CreateBasicTokenOrNull(CurrentThread, user, password));
        return handle.IsInvalid ? null : handle;
    }

    public static AuthTokenHandle CreateBearerToken(string token) =>
        ErrorCheck.SafeCall(Import.CreateBearerToken(CurrentThread, token));

    public static AuthTokenHandle? CreateBearerTokenOrNull(string? token)
    {
        var handle = ErrorCheck.SafeCall(Import.CreateBearerTokenOrNull(CurrentThread, token));
        return handle.IsInvalid ? null : handle;
    }

    public static AuthTokenHandle CreateCustomToken(string scheme, string value) =>
        ErrorCheck.SafeCall(Import.CreateCustomToken(CurrentThread, scheme, value));

    public string GetHttpAuthorization() =>
        ErrorCheck.SafeCall(Import.GetHttpAuthorization(CurrentThread, this));

    public string? GetUser() =>
        ErrorCheck.SafeCall(Import.GetUser(CurrentThread, this));

    public string? GetPassword() =>
        ErrorCheck.SafeCall(Import.GetPassword(CurrentThread, this));

    public string GetScheme() =>
        ErrorCheck.SafeCall(Import.GetScheme(CurrentThread, this));

    public string GetValue() =>
        ErrorCheck.SafeCall(Import.GetValue(CurrentThread, this));

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_AuthToken_valueOf")]
        public static extern AuthTokenHandle ValueOf(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string str);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_AuthToken_createBasicToken")]
        public static extern AuthTokenHandle CreateBasicToken(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string userPassword);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_AuthToken_createBasicToken2")]
        public static extern AuthTokenHandle CreateBasicToken(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string user,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string password);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_AuthToken_createBasicTokenOrNull")]
        public static extern AuthTokenHandle CreateBasicTokenOrNull(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string? user,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string? password);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_AuthToken_createBearerToken")]
        public static extern AuthTokenHandle CreateBearerToken(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string? token);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_AuthToken_createBearerTokenOrNull")]
        public static extern AuthTokenHandle CreateBearerTokenOrNull(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string? token);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_AuthToken_createCustomToken")]
        public static extern AuthTokenHandle CreateCustomToken(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string scheme,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_AuthToken_getHttpAuthorization")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetHttpAuthorization(nint thread, AuthTokenHandle authToken);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_AuthToken_getUser")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string? GetUser(nint thread, AuthTokenHandle authToken);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_AuthToken_getPassword")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string? GetPassword(nint thread, AuthTokenHandle authToken);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_AuthToken_getScheme")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetScheme(nint thread, AuthTokenHandle authToken);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_AuthToken_getValue")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetValue(nint thread, AuthTokenHandle authToken);
    }
}
