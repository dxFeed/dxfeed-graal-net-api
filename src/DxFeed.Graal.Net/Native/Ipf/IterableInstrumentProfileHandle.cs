// <copyright file="IterableInstrumentProfileHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Ipf;

internal sealed class IterableInstrumentProfileHandle : JavaHandle
{
    public IterableInstrumentProfileHandle()
    {
    }

    public IterableInstrumentProfileHandle(IntPtr handle, bool isOwnHandle = true)
        : base(handle, isOwnHandle)
    {
    }

    public List<InstrumentProfile> ToList()
    {
        var list = new List<InstrumentProfile>();
        while (HasNext())
        {
            list.Add(Next()!);
        }

        return list;
    }

    public bool HasNext() =>
        ErrorCheck.SafeCall(NativeHasNext(CurrentThread, this)) != 0;

    public InstrumentProfile? Next()
    {
        var i = NativeNext(CurrentThread, this);
        return i != null ? new InstrumentProfile(i) : null;
    }

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        ExactSpelling = true,
        BestFitMapping = false,
        ThrowOnUnmappableChar = true,
        EntryPoint = "dxfg_Iterable_InstrumentProfile_hasNext")]
    private static extern int NativeHasNext(
        nint thread,
        IterableInstrumentProfileHandle iterable);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        ExactSpelling = true,
        BestFitMapping = false,
        ThrowOnUnmappableChar = true,
        EntryPoint = "dxfg_Iterable_InstrumentProfile_next")]
    private static extern InstrumentProfileHandle? NativeNext(
        nint thread,
        IterableInstrumentProfileHandle iterable);
}
