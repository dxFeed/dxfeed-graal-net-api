// <copyright file="StringMarshaler.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Graal;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Interop;

internal sealed class StringMarshaler : AbstractMarshaler
{
    private static readonly Lazy<StringMarshaler> Instance = new();

    public static ICustomMarshaler GetInstance(string cookie) =>
        Instance.Value;

    public override object? ConvertNativeToManaged(IntPtr native) =>
        Utf8StringMarshaler.PtrToStringUTF8(native);

    public override IntPtr ConvertManagedToNative(object? managed)
    {
        if (managed is not string str)
        {
            throw new ArgumentException("Managed object must be a string.", nameof(managed));
        }

        return Utf8StringMarshaler.StringToCoTaskMemUTF8(str);
    }

    public override void CleanUpFromManaged(IntPtr ptr) =>
        Utf8StringMarshaler.ZeroFreeCoTaskMemUTF8(ptr);

    public override void CleanUpFromNative(IntPtr ptr) =>
        SafeCall(Import.Release(Isolate.CurrentThread, ptr));

    public override void CleanUpListFromNative(IntPtr ptr) =>
        SafeCall(Import.ReleaseList(Isolate.CurrentThread, ptr));

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_String_release")]
        public static extern int Release(nint thread, nint handle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_CList_String_release")]
        public static extern int ReleaseList(nint thread, nint handle);
    }
}
