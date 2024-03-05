// <copyright file="StringMarshaller.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Graal;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Interop;

internal class StringMarshaller : AbstractMarshaller
{
    private static readonly Lazy<StringMarshaller> Instance = new();

    public static ICustomMarshaler GetInstance(string cookie) =>
        Instance.Value;

    public override object? MarshalNativeToManaged(IntPtr native)
    {
        if (native == IntPtr.Zero)
        {
            return null;
        }

        RegisterCleanUpActionsForPointer(native, CleanFromNative);
        return Marshal.PtrToStringUTF8(native);
    }

    public override IntPtr MarshalManagedToNative(object? managed)
    {
        if (managed == null)
        {
            return IntPtr.Zero;
        }

        if (managed is not string str)
        {
            throw new ArgumentException("Managed object must be a string.", nameof(managed));
        }

        var native = Marshal.StringToCoTaskMemUTF8(str);
        RegisterCleanUpActionsForPointer(native, CleanFromManaged);
        return native;
    }

    private static void CleanFromNative(IntPtr native) =>
        SafeCall(Import.Release(Isolate.CurrentThread, native));

    private static void CleanFromManaged(IntPtr native) =>
        Marshal.ZeroFreeCoTaskMemUTF8(native);

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_String_release")]
        public static extern int Release(nint thread, nint handle);
    }
}
