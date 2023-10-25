// <copyright file="InstrumentProfileListNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Ipf;

internal class InstrumentProfileListNative : JavaSafeHandle
{
    public unsafe List<InstrumentProfile> ToList()
    {
        var result = new List<InstrumentProfile>();
        var profiles = (ListNative<InstrumentProfileNative>*)handle;
        for (var i = 0; i < profiles->Size; i++)
        {
            result.Add(InstrumentProfileMapper.Convert(profiles->Elements[i]));
        }

        return result;
    }

    protected override bool ReleaseHandle()
    {
        try
        {
            ErrorCheck.NativeCall(CurrentThread, NativeRelease(CurrentThread, handle));
            handle = (IntPtr)0;
            return true;
        }
        catch (Exception e)
        {
            // ToDo Add a log entry.
            Console.Error.WriteLine($"Exception in {GetType().Name} when releasing resource: {e}");
        }

        return false;
    }

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        ExactSpelling = true,
        BestFitMapping = false,
        ThrowOnUnmappableChar = true,
        EntryPoint = "dxfg_CList_InstrumentProfile_release")]
    private static extern int NativeRelease(
        nint thread,
        nint instrumentProfileList);
}
