// <copyright file="JavaObjectHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Native;

/// <summary>
/// Represents Java object.
/// All structures representing Java objects must include this structure.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly unsafe struct JavaObjectHandle
{
    /// <summary>
    /// The opaque representation of a handle to a Java object given out to unmanaged code.
    /// Clients must not interpret or dereference the value.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public readonly void* OpaqueHandle;
}
