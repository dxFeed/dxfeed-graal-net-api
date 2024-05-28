// <copyright file="Utf8StringMarshaler.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace DxFeed.Graal.Net.Native.Interop;

using System;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// Provides methods for marshaling strings to and from unmanaged UTF-8 null-terminated strings.
/// </summary>
public static class Utf8StringMarshaler
{
    /// <summary>
    /// Converts the given string to an unmanaged UTF-8 null-terminated string.
    /// </summary>
    /// <param name="s">The specified string.</param>
    /// <returns>A pointer to the unmanaged UTF-8 string, or <see cref="IntPtr.Zero"/> if the string is null.</returns>
    public static unsafe IntPtr StringToCoTaskMemUTF8(string? s)
    {
        if (s is null)
        {
            return IntPtr.Zero;
        }

        var nb = Encoding.UTF8.GetMaxByteCount(s.Length);
        var ptr = Marshal.AllocCoTaskMem(checked(nb + 1));
        var pbMem = (byte*)ptr;
        var bytes = Encoding.UTF8.GetBytes(s);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);
        pbMem[bytes.Length] = 0;
        return ptr;
    }

    /// <summary>
    /// Converts an unmanaged UTF-8 null-terminated string to a managed string.
    /// </summary>
    /// <param name="ptr">The pointer to the unmanaged string.</param>
    /// <returns>The managed string, or null if the pointer is <see cref="IntPtr.Zero"/>.</returns>
    public static string? PtrToStringUTF8(IntPtr ptr)
    {
        if (ptr == IntPtr.Zero)
        {
            return null;
        }

        // Find the length of the string
        var length = 0;
        while (Marshal.ReadByte(ptr, length) != 0)
        {
            length++;
        }

        var bytes = new byte[length];
        Marshal.Copy(ptr, bytes, 0, length);
        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// Clears the memory and frees it using <see cref="StringToCoTaskMemUTF8"/>.
    /// </summary>
    /// <param name="s">The pointer to the memory block to be cleared and freed.</param>
    public static unsafe void ZeroFreeCoTaskMemUTF8(IntPtr s)
    {
        if (s == IntPtr.Zero)
        {
            return;
        }

        // Get the length of the string
        var length = 0;
        var ptr = (byte*)s.ToPointer();
        while (*(ptr + length) != 0)
        {
            length++;
        }

        // Clear the memory
        for (var i = 0; i < length; i++)
        {
            *(ptr + i) = 0;
        }

        // Free the memory
        Marshal.FreeCoTaskMem(s);
    }
}
