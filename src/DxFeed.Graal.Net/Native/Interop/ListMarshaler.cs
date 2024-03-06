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

internal class ListMarshaler<T> : AbstractMarshaler
    where T : AbstractMarshaler, new()
{
    private static readonly Lazy<ListMarshaler<T>> Instance = new();
    private static readonly T ElementMarshaller = new();

    public static ICustomMarshaler GetInstance(string cookie) =>
        Instance.Value;

    public override unsafe object ConvertNativeToManaged(IntPtr native)
    {
        var list = (ListNative*)native;
        var result = new List<object?>(list->Size);
        for (var i = 0; i < list->Size; ++i)
        {
            result.Add(ElementMarshaller.ConvertNativeToManaged((IntPtr)list->Elements[i]));
        }

        return result;
    }

    public override unsafe IntPtr ConvertManagedToNative(object? managed)
    {
        if (managed is not IEnumerable<object> enumerable)
        {
            throw new ArgumentException("Managed object must be an IEnumerable.", nameof(managed));
        }

        var elements = enumerable as object[] ?? enumerable.ToArray();
        var size = elements.Length;
        var list = (ListNative*)Marshal.AllocHGlobal(sizeof(ListNative));
        list->Size = size;
        list->Elements = (void**)Marshal.AllocHGlobal(IntPtr.Size * size);
        for (var i = 0; i < elements.Length; ++i)
        {
            list->Elements[i] = (void*)ElementMarshaller.ConvertManagedToNative(elements[i]);
        }

        return (IntPtr)list;
    }

    public override unsafe void CleanUpFromManaged(IntPtr ptr)
    {
        if (ptr == IntPtr.Zero)
        {
            return;
        }

        var list = (ListNative*)ptr;
        for (var i = 0; i < list->Size; ++i)
        {
            ElementMarshaller.CleanUpFromManaged((IntPtr)list->Elements[i]);
        }

        Marshal.FreeHGlobal((IntPtr)list->Elements);
        Marshal.FreeHGlobal((IntPtr)list);
    }

    public override void CleanUpFromNative(IntPtr ptr) =>
        CleanUpListFromNative(ptr);

    public override void CleanUpListFromNative(IntPtr ptr) =>
        ElementMarshaller.CleanUpListFromNative(ptr);

    [StructLayout(LayoutKind.Sequential)]
    private unsafe struct ListNative
    {
        public int Size;
        public void** Elements;
    }
}
