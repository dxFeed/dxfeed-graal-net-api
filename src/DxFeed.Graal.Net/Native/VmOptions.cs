// <copyright file="VmOptions.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native;

/// <summary>
/// VmOptions native object.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VmOptions
{
    /// <summary>
    /// The opaque representation of a handle to a Java object given out to unmanaged code.
    /// Clients must not interpret or dereference the value.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    private StringNative javaHome;
    private IntPtr vmOptionsArray;
    private int vmArgsCount;

    /// <summary>
    /// Allocates memory for native struct and fill it with data.
    /// </summary>
    /// <param name="javaHome">Custom JAVA_HOME path.</param>
    /// <returns>The pointer to filled native struct.</returns>
    internal static IntPtr Alloc(string javaHome)
    {
        var s = (VmOptions*)Marshal.AllocHGlobal(sizeof(VmOptions));
        s->javaHome = javaHome;
        s->vmOptionsArray = IntPtr.Zero;
        s->vmArgsCount = 0;
        return (IntPtr)s;
    }

    internal static void Dealloc(nint vmOptions) => Marshal.FreeHGlobal(vmOptions);
}
