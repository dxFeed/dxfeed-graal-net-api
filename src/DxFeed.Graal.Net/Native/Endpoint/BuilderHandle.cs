// <copyright file="BuilderHandle.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Interop;
using static DxFeed.Graal.Net.Api.DXEndpoint;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Endpoint;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by marshaler")]
internal sealed class BuilderHandle : JavaHandle
{
    public static BuilderHandle Create() =>
        SafeCall(Import.New(CurrentThread));

    public void WithRole(Role role) =>
        SafeCall(Import.WithRole(CurrentThread, this, role));

    public void WithProperty(string key, string value) =>
        SafeCall(Import.WithProperty(CurrentThread, this, key, value));

    public bool SupportsProperty(string key) =>
        SafeCall(Import.SupportsProperty(CurrentThread, this, key)) != 0;

    public DXEndpointHandle Build() =>
        SafeCall(Import.Build(CurrentThread, this));

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_newBuilder")]
        public static extern BuilderHandle New(nint thread);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_Builder_withRole")]
        public static extern int WithRole(
            nint thread,
            BuilderHandle builder,
            Role role);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_DXEndpoint_Builder_withProperty")]
        public static extern int WithProperty(
            nint thread,
            BuilderHandle builder,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string key,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_DXEndpoint_Builder_supportsProperty")]
        public static extern int SupportsProperty(
            nint thread,
            BuilderHandle builder,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string key);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_Builder_build")]
        public static extern DXEndpointHandle Build(
            nint thread,
            BuilderHandle builder);
    }
}
