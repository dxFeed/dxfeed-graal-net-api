// <copyright file="StringNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Native.Interop;

/// <summary>
/// Structure that encapsulates an unsafe pointer to a string.
/// </summary>
/// <param name="NativeStringPtr">The unsafe pointer to string.</param>
[StructLayout(LayoutKind.Sequential)]
internal readonly record struct StringNative(nint NativeStringPtr)
{
    /// <summary>
    /// Create string from unsafe null-terminated UTF-8 string pointer.
    /// </summary>
    /// <returns>The created string.</returns>
    public override string? ToString() =>
        Marshal.PtrToStringUTF8(NativeStringPtr);
}
