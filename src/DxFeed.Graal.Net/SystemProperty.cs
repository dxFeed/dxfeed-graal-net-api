// <copyright file="SystemProperty.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Interop;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net;

/// <summary>
/// Native wrapper over the Java <c>java.lang.System</c> class, contains work with property getter/setter methods.
/// In Java world, these properties can be set by passing the <c>"-Dprop=value"</c> argument in command line
/// or calls <c>java.lang.System.setProperty(String key, String value)</c>.
/// The location of the imported functions is in the header files <c>"dxfg_system.h"</c>.
/// </summary>
/// <example><c>-Ddxfeed.address="demo.dxfeed.com:7400"</c>.</example>
public static class SystemProperty
{
    /// <summary>
    /// Sets the system property indicated by the specified key.
    /// </summary>
    /// <param name="key">The name of the system property.</param>
    /// <param name="value">The value of the system property.</param>
    public static void SetProperty(string key, string value) =>
        SafeCall(Import.SystemSetProperty(Isolate.CurrentThread, key, value));

    /// <summary>
    /// Sets the system properties from the provided key-value collection.
    /// </summary>
    /// <param name="properties">The key-value collection.</param>
    public static void SetProperties(IReadOnlyDictionary<string, string> properties)
    {
        foreach (var property in properties)
        {
            SetProperty(property.Key, property.Value);
        }
    }

    /// <summary>
    /// Gets the system property indicated by the specified key.
    /// </summary>
    /// <param name="key">The name of the system property.</param>
    /// <returns>
    /// The string value of the system property, or <c>null</c> if there is no property with that key.
    /// </returns>
    public static string? GetProperty(string key) =>
        Import.SystemGetProperty(Isolate.CurrentThread, key);

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_system_set_property")]
        public static extern int SystemSetProperty(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string key,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_system_get_property")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string? SystemGetProperty(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string key);
    }
}
