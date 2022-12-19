// <copyright file="StringUtilNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using System.Text;

namespace DxFeed.Graal.Net.Native.Utils;

// ToDo Use ArrayPool for temp buffer.
internal static class StringUtilNative
{
    public static nint NativeFromString(string managedString, Encoding encoding)
    {
        var length = encoding.GetByteCount(managedString);
        var buffer = new byte[length + 1];
        encoding.GetBytes(managedString, 0, managedString.Length, buffer, 0);
        var nativeString = Marshal.AllocHGlobal(buffer.Length);
        Marshal.Copy(buffer, 0, nativeString, buffer.Length);
        return nativeString;
    }

    public static string StringFromNative(nint nativeString, Encoding encoding)
    {
        var length = 0;
        while (Marshal.ReadByte(nativeString, length) != 0)
        {
            ++length;
        }

        var buffer = new byte[length];
        Marshal.Copy(nativeString, buffer, 0, buffer.Length);
        return encoding.GetString(buffer);
    }
}
