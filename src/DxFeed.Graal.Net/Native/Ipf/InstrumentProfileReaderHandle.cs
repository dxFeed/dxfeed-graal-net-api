// <copyright file="InstrumentProfileReaderHandle.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
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
        var ptr = SafeCall(Import.ReadFromFile(CurrentThread, this, address, user, password));
        return ConvertToProfiles(ptr);
    }

    public unsafe List<InstrumentProfile> ReadFromFile(string address, AuthToken? authToken)
    {
        var tokenHandle = authToken == null ? new AuthTokenHandle() : authToken.Handle;
        var ptr = SafeCall(Import.ReadFromFile(CurrentThread, this, address, tokenHandle));
        return ConvertToProfiles(ptr);
    }

    private unsafe List<InstrumentProfile> ConvertToProfiles(ListNative<IntPtr>* handles)
    {
        try
        {
            var profiles = new List<InstrumentProfile>();
            for (var i = 0; i < handles->Size; i++)
            {
                profiles.Add(new InstrumentProfile(new InstrumentProfileHandle((IntPtr)handles->Elements[i])));
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
            EntryPoint = "dxfg_InstrumentProfileReader_readFromFile2")]
        public static extern unsafe ListNative<IntPtr>* ReadFromFile(
            nint thread,
            InstrumentProfileReaderHandle reader,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string address,
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
            EntryPoint = "dxfg_InstrumentProfileReader_readFromFile3")]
        public static extern unsafe ListNative<IntPtr>* ReadFromFile(
            nint thread,
            InstrumentProfileReaderHandle reader,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string address,
            AuthTokenHandle authToken);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_CList_InstrumentProfile_wrapper_release")]
        public static extern unsafe ListNative<IntPtr>* ReleaseListWrapper(
            nint thread,
            ListNative<IntPtr>* list);
    }
}
