// <copyright file="AbstractMarshaller.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Native.Interop;

/// <summary>
/// Provides a base implementation for custom marshalling between managed and native code.
/// This abstract class implements the <see cref="ICustomMarshaler"/> interface, allowing for
/// custom logic in marshalling data to and from native code. Implementations must define
/// how to marshal specific types by overriding the abstract methods.
/// </summary>
#pragma warning disable CA1725
#pragma warning disable CS8766
#pragma warning disable S927
internal abstract class AbstractMarshaller : ICustomMarshaler
{
    private readonly ConcurrentDictionary<IntPtr, Action<IntPtr>> cleanUpActions = new();

    /// <inheritdoc/>
    public abstract object? MarshalNativeToManaged(IntPtr native);

    /// <inheritdoc/>
    public abstract IntPtr MarshalManagedToNative(object? managed);

    /// <inheritdoc/>
    public virtual void CleanUpNativeData(IntPtr native)
    {
        if (cleanUpActions.TryRemove(native, out var cleanUp))
        {
            cleanUp(native);
        }
    }

    /// <inheritdoc/>
    public virtual void CleanUpManagedData(object managed)
    {
        // Typically, a cleanup for a managed object is not required.
        // If necessary, inheritors can override this method.
    }

    /// <inheritdoc/>
    public virtual int GetNativeDataSize() => -1;

    /// <summary>
    /// Registers a cleanup action for a specific native pointer. This allows for custom cleanup logic
    /// to be executed when the native data associated with the pointer needs to be released.
    /// </summary>
    /// <param name="pointer">The native pointer for which to register the cleanup action.</param>
    /// <param name="action">The action to execute for cleaning up the native data.</param>
    protected void RegisterCleanUpActionsForPointer(IntPtr pointer, Action<IntPtr> action) =>
        cleanUpActions.TryAdd(pointer, action);
}
