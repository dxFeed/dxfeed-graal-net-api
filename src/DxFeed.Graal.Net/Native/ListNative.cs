// <copyright file="ListNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Native;

// ToDo Create wrapper implements dispose pattern.
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ListNative<T>
    where T : unmanaged
{
    public int Size;
    public T** Elements;

    public static ListNative<T>* CreateNative(IEnumerable<T> elements)
    {
        var enumerable = elements as T[] ?? elements.ToArray();
        var list = (ListNative<T>*)Marshal.AllocHGlobal(sizeof(ListNative<T>));
        list->Size = enumerable.Length;
        list->Elements = (T**)Marshal.AllocHGlobal(sizeof(T*) * list->Size);
        var i = 0;
        foreach (var element in enumerable)
        {
            list->Elements[i] = (T*)Marshal.AllocHGlobal(sizeof(T));
            *list->Elements[i] = element;
            ++i;
        }

        return list;
    }

    public static void ReleaseNative(ListNative<T>* list)
    {
        if (list == null)
        {
            return;
        }

        for (var i = 0; i < list->Size; ++i)
        {
            Marshal.FreeHGlobal((nint)list->Elements[i]);
        }

        Marshal.FreeHGlobal((nint)list->Elements);
        Marshal.FreeHGlobal((nint)list);
    }
}
