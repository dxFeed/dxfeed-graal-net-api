// <copyright file="IndexedEventSourceNative.cs" company="Devexperts LLC">
// Copyright © 2026 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Native.Interop;

/// <summary>
/// A wrapper structure for the dxfg_indexed_event_source_t Graal Native SDK struct.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct IndexedEventSourceNative
{
    /// <summary>
    /// The source's type (dxfg_indexed_event_source_type_t).
    /// </summary>
    public IndexedEventSourceTypeNative Type;

    /// <summary>
    /// The source id.
    /// </summary>
    public int Id;

    /// <summary>
    /// The source name.
    /// </summary>
    public StringNative Name;
}
