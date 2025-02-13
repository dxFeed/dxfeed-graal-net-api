// <copyright file="InstrumentProfileReaderHandle.cs" company="Devexperts LLC">
// Copyright © 2025 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Auth;
using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Native.Auth;
using DxFeed.Graal.Net.Native.Interop;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Ipf;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by marshaler")]
internal sealed class InstrumentProfileReaderHandle : JavaHandle
{
    private static readonly InstrumentProfileMarshaler _marshaler = new();

    public static string? ResolveSourceUrl(string address) =>
        SafeCall(Import.ResolveSourceUrl(CurrentThread, address));

    public static InstrumentProfileReaderHandle Create() =>
        SafeCall(Import.NativeCreate(CurrentThread));

    public long GetLastModified() =>
        SafeCall(Import.GetLastModified(CurrentThread, this));

    public bool WasComplete() =>
        SafeCall(Import.WasComplete(CurrentThread, this)) != 0;

    public unsafe List<InstrumentProfile> ReadFromFile(string address, string? user, string? password)
    {
        SafeCall(Import.ReadFromFile(CurrentThread, this, address, user, password, out var ptr));
        return ConvertToProfiles(ptr);
    }

    public unsafe List<InstrumentProfile> ReadFromFile(string address, AuthToken? authToken)
    {
        var tokenHandle = authToken == null ? new AuthTokenHandle() : authToken.Handle;
        SafeCall(Import.ReadFromFile(CurrentThread, this, address, tokenHandle, out var ptr));
        return ConvertToProfiles(ptr);
    }

    private unsafe List<InstrumentProfile> ConvertToProfiles(ListNative<IntPtr>* handles)
    {
        try
        {
            var profiles = new List<InstrumentProfile>(handles->Size);
            for (var i = 0; i < handles->Size; i++)
            {
                var profile = (InstrumentProfile)_marshaler.ConvertNativeToManaged((IntPtr)handles->Elements[i])!;
                profiles.Add(profile);
            }

            return profiles;
        }
        finally
        {
            SafeCall(Import.ReleaseListWrapper(CurrentThread, handles));
        }
    }

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfileReader_resolveSourceURL")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string? ResolveSourceUrl(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string address);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_InstrumentProfileReader_new")]
        public static extern InstrumentProfileReaderHandle NativeCreate(nint thread);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_InstrumentProfileReader_getLastModified")]
        public static extern long GetLastModified(
            nint thread,
            InstrumentProfileReaderHandle reader);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_InstrumentProfileReader_wasComplete")]
        public static extern int WasComplete(
            nint thread,
            InstrumentProfileReaderHandle reader);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfileReader_readFromFile8")]
        public static extern unsafe int ReadFromFile(
            nint thread,
            InstrumentProfileReaderHandle reader,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string address,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string? user,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string? password,
            out ListNative<IntPtr>* profiles);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfileReader_readFromFile9")]
        public static extern unsafe int ReadFromFile(
            nint thread,
            InstrumentProfileReaderHandle reader,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string address,
            AuthTokenHandle authToken,
            out ListNative<IntPtr>* profiles);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_instrument_profile2_list_free")]
        public static extern unsafe int ReleaseListWrapper(
            nint thread,
            ListNative<IntPtr>* list);
    }
}
