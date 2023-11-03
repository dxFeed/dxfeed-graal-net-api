// <copyright file="SafeHandleZeroIsInvalid.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Native.Interop;

/// <summary>
/// Represents an abstract base class for classes that interact with native handles.
/// In the context of this class, a handle with a value of <c>zero</c> is considered invalid or uninitialized.
/// The validity of the handle can be determined using the <see cref="IsInvalid"/> property.
/// </summary>
internal abstract class SafeHandleZeroIsInvalid : SafeHandle
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SafeHandleZeroIsInvalid"/> class.
    /// By default, this class assumes ownership of the handle.
    /// </summary>
    /// <param name="isOwnHandle">
    /// <c>true</c> to reliably let release the handle during the finalization phase; otherwise, <c>false</c>.
    /// </param>
    protected SafeHandleZeroIsInvalid(bool isOwnHandle = true)
        : base(IntPtr.Zero, isOwnHandle)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeHandleZeroIsInvalid"/> class with a specified handle value.
    /// By its default setting, this class assumes ownership of the handle.
    /// </summary>
    /// <param name="handle">The handle value to set.</param>
    /// <param name="isOwnHandle">
    /// <c>true</c> to reliably let release the handle during the finalization phase; otherwise, <c>false</c>.
    /// </param>
    protected SafeHandleZeroIsInvalid(IntPtr handle, bool isOwnHandle = true)
        : base(IntPtr.Zero, isOwnHandle) =>
        SetHandle(handle);

    /// <summary>
    /// Gets a value indicating whether the handle is invalid.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the handle is considered invalid (i.e., has a value of zero); otherwise, <c>false</c>.
    /// </returns>
    public override bool IsInvalid =>
        handle == IntPtr.Zero;
}
