// <copyright file="InstrumentProfileReaderNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Native.Interop;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Ipf;

internal class InstrumentProfileReaderNative : JavaHandle
{
    public static string? ResolveSourceUrl(string address) =>
        SafeCall(NativeResolveSourceUrl(CurrentThread, address));

    public static InstrumentProfileReaderNative Create() =>
        SafeCall(NativeCreate(CurrentThread));

    public long GetLastModified() =>
        SafeCall(NativeGetLastModified(CurrentThread, this));

    public bool WasComplete() =>
        SafeCall(NativeWasComplete(CurrentThread, this)) != 0;

    public List<InstrumentProfile> ReadFromFile(string address, string? user, string? password)
    {
        using var result = SafeCall(NativeReadFromFile(CurrentThread, this, address, user, password));
        return result.ToList();
    }

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        ExactSpelling = true,
        BestFitMapping = false,
        ThrowOnUnmappableChar = true,
        EntryPoint = "dxfg_InstrumentProfileReader_resolveSourceURL")]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
    private static extern string? NativeResolveSourceUrl(
        nint thread,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string address);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_InstrumentProfileReader_new")]
    private static extern InstrumentProfileReaderNative NativeCreate(nint thread);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_InstrumentProfileReader_getLastModified")]
    private static extern long NativeGetLastModified(
        nint thread,
        InstrumentProfileReaderNative reader);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_InstrumentProfileReader_wasComplete")]
    private static extern int NativeWasComplete(
        nint thread,
        InstrumentProfileReaderNative reader);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        ExactSpelling = true,
        BestFitMapping = false,
        ThrowOnUnmappableChar = true,
        EntryPoint = "dxfg_InstrumentProfileReader_readFromFile2")]
    private static extern InstrumentProfileListNative NativeReadFromFile(
        nint thread,
        InstrumentProfileReaderNative reader,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string address,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string? user,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string? password);
}
