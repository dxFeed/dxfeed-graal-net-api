// <copyright file="ListMarshaller.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Native.Interop;

internal class ListMarshaller<T> : AbstractMarshaller
    where T : AbstractMarshaller, new()
{
    private static readonly Lazy<ListMarshaller<T>> Instance = new();
    private static readonly T ElementMarshaller = new();

    public static ICustomMarshaler GetInstance(string cookie) =>
        Instance.Value;

    public override object MarshalNativeToManaged(IntPtr native) =>
        throw new NotImplementedException();

    public override unsafe IntPtr MarshalManagedToNative(object? managed)
    {
        if (managed == null)
        {
            return IntPtr.Zero;
        }

        if (managed is not IEnumerable<object> enumerable)
        {
            throw new ArgumentException("Managed object must be an IEnumerable<T>.", nameof(managed));
        }

        var list = AllocateListNative(enumerable);
        return (IntPtr)list;
    }

    public override unsafe void CleanUpNativeData(IntPtr native)
    {
        if (native == IntPtr.Zero)
        {
            return;
        }

        var list = (ListNative*)native;
        for (var i = 0; i < list->Size; ++i)
        {
            ElementMarshaller.CleanUpNativeData((IntPtr)list->Elements[i]);
        }

        Marshal.FreeHGlobal((IntPtr)list->Elements);
        Marshal.FreeHGlobal((IntPtr)list);
    }

    private static unsafe ListNative* AllocateListNative(IEnumerable<object> enumerable)
    {
        var elements = enumerable as object[] ?? enumerable.ToArray();
        var size = elements.Length;
        var list = (ListNative*)Marshal.AllocHGlobal(sizeof(ListNative));
        list->Size = size;
        list->Elements = (void**)Marshal.AllocHGlobal(IntPtr.Size * size);
        for (var i = 0; i < elements.Length; ++i)
        {
            list->Elements[i] = (void*)ElementMarshaller.MarshalManagedToNative(elements[i]);
        }

        return list;
    }

    [StructLayout(LayoutKind.Sequential)]
    private unsafe struct ListNative
    {
        public int Size;
        public void** Elements;
    }
}
