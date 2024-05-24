// <copyright file="StringNative.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Native.Interop;

/// <summary>
/// A structure that encapsulates an unsafe pointer to a string.
/// String always converted to/from null-terminated UTF-8 string.
/// </summary>
/// <param name="NativeStringPtr">The unsafe pointer to null-terminated UTF-8 string.</param>
[StructLayout(LayoutKind.Sequential)]
internal record struct StringNative(nint NativeStringPtr)
{
    public static implicit operator StringNative(string? value) =>
        ValueOf(value);

    public static implicit operator string?(StringNative value) =>
        value.ToString();

    /// <summary>
    /// Converts the given string into the <see cref="StringNative"/> (unmanaged null-terminated UTF-8 string).
    /// The <see cref="StringNative"/> created by this method, must be release by <see cref="Release"/>.
    /// </summary>
    /// <param name="value">The specified string.</param>
    /// <returns>The <see cref="StringNative"/>.</returns>
    public static StringNative ValueOf(string? value) =>
        new() { NativeStringPtr = Utf8StringMarshaler.StringToCoTaskMemUTF8(value) };

    /// <summary>
    /// Releases all associated resources.
    /// </summary>
    public void Release() =>
        Utf8StringMarshaler.ZeroFreeCoTaskMemUTF8(NativeStringPtr);

    /// <summary>
    /// Allocates a managed <see cref="string"/> and copies all characters
    /// up to the first null character from an unmanaged UTF-8 string into it.
    /// </summary>
    /// <returns>
    /// A managed string that holds a copy of the unmanaged string
    /// if the value of the ptr parameter is not null; otherwise, this method returns null.
    /// </returns>
    public override string? ToString() =>
        Utf8StringMarshaler.PtrToStringUTF8(NativeStringPtr);
}
