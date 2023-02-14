// <copyright file="SafeHandleZeroIsInvalid.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Native.Interop;

/// <summary>
/// Provides a base class for SafeHandle implementations in which the value of zero an invalid handle.
/// </summary>
internal abstract class SafeHandleZeroIsInvalid : SafeHandle
{
    protected SafeHandleZeroIsInvalid()
        : base((nint)0, true)
    {
    }

    protected SafeHandleZeroIsInvalid(bool ownsHandle)
        : base((nint)0, ownsHandle)
    {
    }

    /// <inheritdoc />
    public override bool IsInvalid =>
        handle == (nint)0;
}
