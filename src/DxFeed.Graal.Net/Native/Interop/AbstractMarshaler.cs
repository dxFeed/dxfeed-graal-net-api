// <copyright file="AbstractMarshaler.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Native.Interop;

#pragma warning disable CS8766
#pragma warning disable CA1725
#pragma warning disable S927

/// <summary>
/// Provides an abstract base for implementing custom marshalling logic between managed and native code.
/// This abstract class implements the <see cref="ICustomMarshaler"/> interface, allowing for
/// custom logic in marshalling data to and from native code.
/// This class facilitates the registration of cleanup actions for resources and defines abstract methods
/// for converting between managed and native types.
/// </summary>
/// <example>
/// <code>
/// internal class MyMarshaler : AbstractMarshaler
/// {
///    private static readonly Lazy&lt;MyMarshaler&gt; Instance = new();
///    public static ICustomMarshaler GetInstance(string cookie) => Instance.Value;
///    public override object? ConvertNativeToManaged(IntPtr native) =>
///        throw new NotImplementedException();
///    public override IntPtr ConvertManagedToNative(object? managed) =>
///        throw new NotImplementedException();
///    public override void CleanUpFromManaged(IntPtr ptr) =>
///        throw new NotImplementedException();
///    public override void CleanUpFromNative(IntPtr ptr) =>
///         throw new NotImplementedException();
///    public override void CleanUpListFromNative(IntPtr ptr) =>
///         throw new NotImplementedException();
/// }
/// </code>
/// </example>
internal abstract class AbstractMarshaler : ICustomMarshaler
{
    private readonly ConcurrentDictionary<IntPtr, Action<IntPtr>> cleanUpActions = new();

    /// <summary>
    /// Converts the unmanaged data to managed data and registering a cleanup action.
    /// This method is usually called directly by the marshaler.
    /// </summary>
    /// <param name="native">The native pointer to convert.</param>
    /// <returns>A managed object corresponding to the native pointer.</returns>
    public object? MarshalNativeToManaged(IntPtr native)
    {
        if (native == IntPtr.Zero)
        {
            return null;
        }

        cleanUpActions.TryAdd(native, CleanUpFromNative);
        return ConvertNativeToManaged(native);
    }

    /// <summary>
    /// Converts the managed data to unmanaged data and registering a cleanup action.
    /// This method is usually called directly by the marshaler.
    /// </summary>
    /// <param name="managed">The managed object to convert.</param>
    /// <returns>A native pointer corresponding to the managed object.</returns>
    public IntPtr MarshalManagedToNative(object? managed)
    {
        if (managed == null)
        {
            return IntPtr.Zero;
        }

        var ptr = ConvertManagedToNative(managed);
        cleanUpActions.TryAdd(ptr, CleanUpFromManaged);
        return ptr;
    }

    /// <summary>
    /// Cleans up native data using a previously registered action.
    /// </summary>
    /// <param name="native">The native pointer.</param>
    public void CleanUpNativeData(IntPtr native)
    {
        if (cleanUpActions.TryRemove(native, out var cleanUp))
        {
            cleanUp(native);
        }
    }

    /// <summary>
    /// Placeholder for cleaning up managed data, if necessary.
    /// </summary>
    /// <param name="managed">The managed data to clean up.</param>
    public virtual void CleanUpManagedData(object managed)
    {
        // Typically, a cleanup for a managed object is not required.
        // If necessary, inheritors can override this method.
    }

    /// <summary>
    /// Returns the size of the native data to be marshaled.
    /// </summary>
    /// <returns> <c>-1</c> to indicate the managed type this marshaler handles is not a value type.</returns>
    public int GetNativeDataSize() => -1;

    /// <summary>
    /// Converts a native pointer to a managed object.
    /// The <see cref="IntPtr"/> passed to this method must be released using
    /// the <see cref="CleanUpFromNative"/> or <see cref="CleanUpListFromNative"/> method.
    /// </summary>
    /// <param name="native">The native pointer to convert.</param>
    /// <returns>A managed object corresponding to the native pointer.</returns>
    public abstract object? ConvertNativeToManaged(IntPtr native);

    /// <summary>
    /// Converts a managed object to a native pointer.
    /// The <see cref="IntPtr"/> returned by this method must be released using
    /// the <see cref="CleanUpFromManaged"/> method.
    /// </summary>
    /// <param name="managed">The managed object to convert.</param>
    /// <returns>A native pointer corresponding to the managed object.</returns>
    public abstract IntPtr ConvertManagedToNative(object? managed);

    /// <summary>
    /// Cleans up native data converted from managed data.
    /// </summary>
    /// <param name="ptr">The native pointer to clean up.</param>
    public abstract void CleanUpFromManaged(IntPtr ptr);

    /// <summary>
    /// Cleans up native data obtained directly from unmanaged code.
    /// </summary>
    /// <param name="ptr">The native pointer to clean up.</param>
    public abstract void CleanUpFromNative(IntPtr ptr);

    /// <summary>
    /// Cleans up native data obtained directly from unmanaged code and representing the native list.
    /// </summary>
    /// <param name="ptr">The native pointer representing the native list to clean up.</param>
    public abstract void CleanUpListFromNative(IntPtr ptr);
}
