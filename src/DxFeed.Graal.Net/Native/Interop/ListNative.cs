// <copyright file="ListNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Native.Interop;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ListNative<TNative>
    where TNative : unmanaged
{
    public int Size;
    public TNative** Elements;

    public static ListNative<TNative>* Create(IEnumerable<TNative> elements) =>
        Create(elements, e =>
        {
            var ptr = (TNative*)Marshal.AllocHGlobal(sizeof(TNative));
            *ptr = e;
            return (nint)ptr;
        });

    public static ListNative<TNative>* Create<TManaged>(IEnumerable<TManaged> elements, Func<TManaged, nint> alloc)
    {
        var enumerable = elements as TManaged[] ?? elements.ToArray();
        var list = (ListNative<TNative>*)Marshal.AllocHGlobal(sizeof(ListNative<TNative>));
        list->Size = enumerable.Length;
        list->Elements = (TNative**)Marshal.AllocHGlobal(sizeof(TNative*) * list->Size);
        var i = 0;
        foreach (var element in enumerable)
        {
            list->Elements[i] = (TNative*)alloc(element);
            ++i;
        }

        return list;
    }

    public static void Release(ListNative<TNative>* list) =>
        Release(list, Marshal.FreeHGlobal);

    public static void Release(ListNative<TNative>* list, Action<nint> release)
    {
        if (list == null)
        {
            return;
        }

        for (var i = 0; i < list->Size; ++i)
        {
            release((nint)list->Elements[i]);
        }

        Marshal.FreeHGlobal((nint)list->Elements);
        Marshal.FreeHGlobal((nint)list);
    }
}
