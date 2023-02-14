// <copyright file="SystemPropertyNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Graal;

namespace DxFeed.Graal.Net.Native;

/// <summary>
/// Native wrapper over the Java <c>java.lang.System</c> class, contains work with property getter/setter methods.
/// In Java world, these properties can be set by passing the <c>"-Dprop=value"</c> argument in command line
/// or calls <c>java.lang.System.setProperty(String key, String value)</c>.
/// The location of the imported functions is in the header files <c>"dxfg_system.h"</c>.
/// </summary>
/// <example><c>-Ddxfeed.address="demo.dxfeed.com:7400"</c>.</example>
internal static class SystemPropertyNative
{
    /// <summary>
    /// Sets the system property indicated by the specified key.
    /// </summary>
    /// <param name="key">The name of the system property.</param>
    /// <param name="value">The value of the system property.</param>
    public static void SetProperty(string key, string value) =>
        Import.SystemSetProperty(GetCurrentThread(), key, value);

    /// <summary>
    /// Gets the system property indicated by the specified key.
    /// </summary>
    /// <param name="key">The name of the system property.</param>
    /// <returns>
    /// The string value of the system property, or <c>null</c> if there is no property with that key.
    /// </returns>
    public static string? GetProperty(string key) =>
        Import.SystemGetProperty(GetCurrentThread(), key);

    private static nint GetCurrentThread() =>
        Isolate.CurrentThread;

    /// <summary>
    /// Contains imported functions from native code.
    /// </summary>
    private static class Import
    {
        public static void SystemSetProperty(nint thread, string key, string value) =>
            ErrorCheck.NativeCall(thread, NativeSystemSetProperty(thread, key, value));

        public static string? SystemGetProperty(nint thread, string key)
        {
            var valuePtr = NativeSystemGetProperty(thread, key);
            if (valuePtr == 0)
            {
                return null;
            }

            try
            {
                var value = Marshal.PtrToStringUTF8(valuePtr);
                return value;
            }
            finally
            {
                NativeSystemReleaseProperty(thread, valuePtr);
            }
        }

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_system_set_property")]
        private static extern int NativeSystemSetProperty(
            nint thread,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string key,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_system_get_property")]
        private static extern nint NativeSystemGetProperty(
            nint thread,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string key);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_system_release_property")]
        private static extern nint NativeSystemReleaseProperty(
            nint thread,
            nint value);
    }
}
