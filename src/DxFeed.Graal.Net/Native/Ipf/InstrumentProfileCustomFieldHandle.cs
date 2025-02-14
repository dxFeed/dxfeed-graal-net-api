// <copyright file="InstrumentProfileCustomFieldHandle.cs" company="Devexperts LLC">
// Copyright © 2025 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Interop;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Ipf;

internal class InstrumentProfileCustomFieldHandle : JavaHandle
{
    private static readonly StringMarshaler _marshaler = new();

    public static InstrumentProfileCustomFieldHandle Create()
    {
        SafeCall(Import.New(CurrentThread, out var handle));
        return handle;
    }

    public static InstrumentProfileCustomFieldHandle Create(InstrumentProfileCustomFieldHandle handle)
    {
        SafeCall(Import.New(CurrentThread, handle, out var newHandle));
        return newHandle;
    }

    public static InstrumentProfileCustomFieldHandle Clone(IntPtr handle)
    {
        SafeCall(Import.CloneHandle(CurrentThread, handle, out var newHandle));
        return newHandle;
    }

    public string GetField(string name)
    {
        SafeCall(Import.GetField(CurrentThread, this, name, out var value));
        return value;
    }

    public void SetField(string name, string value) =>
        SafeCall(Import.SetField(CurrentThread, this, name, value));

    public double GetNumericField(string name)
    {
        SafeCall(Import.GetNumericField(CurrentThread, this, name, out var value));
        return value;
    }

    public void SetNumericField(string name, double value) =>
        SafeCall(Import.SetNumericField(CurrentThread, this, name, value));

    public int GetDateField(string name)
    {
        SafeCall(Import.GetDateField(CurrentThread, this, name, out var value));
        return value;
    }

    public void SetDateField(string name, int value) =>
        SafeCall(Import.SetDateField(CurrentThread, this, name, value));

    public unsafe bool AddNonEmptyCustomFieldNames(ICollection<string> targetFieldNames)
    {
        SafeCall(Import.GetNonEmptyFieldsNames(CurrentThread, this, out var ptr));
        var fields = ConvertToStringList(ptr);

        if (fields.Count == 0)
        {
            return false;
        }

        if (targetFieldNames is ISet<string> set)
        {
            var updated = false;
            foreach (var field in fields)
            {
                if (set.Add(field))
                {
                    updated = true;
                }
            }

            return updated;
        }

        foreach (var field in fields)
        {
            targetFieldNames.Add(field);
        }

        return true;
    }

    private static unsafe List<string> ConvertToStringList(ListNative<IntPtr>* handles)
    {
        try
        {
            var venues = new List<string>(handles->Size);
            for (var i = 0; i < handles->Size; i++)
            {
                var profile = (string)_marshaler.ConvertNativeToManaged((IntPtr)handles->Elements[i])!;
                venues.Add(profile);
            }

            return venues;
        }
        finally
        {
            SafeCall(Import.ReleaseList(CurrentThread, (IntPtr)handles));
        }
    }

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfileCustomFields_new")]
        public static extern int New(
            nint thread,
            out InstrumentProfileCustomFieldHandle handle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfileCustomFields_new3")]
        public static extern int New(
            nint thread,
            InstrumentProfileCustomFieldHandle handle,
            out InstrumentProfileCustomFieldHandle newHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfileCustomFields_getField")]
        public static extern int GetField(
            nint thread,
            InstrumentProfileCustomFieldHandle handle,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string name,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            out string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfileCustomFields_setField")]
        public static extern int SetField(
            nint thread,
            InstrumentProfileCustomFieldHandle handle,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string name,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfileCustomFields_getNumericField")]
        public static extern int GetNumericField(
            nint thread,
            InstrumentProfileCustomFieldHandle handle,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string name,
            out double value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfileCustomFields_setNumericField")]
        public static extern int SetNumericField(
            nint thread,
            InstrumentProfileCustomFieldHandle handle,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string name,
            double value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfileCustomFields_getDateField")]
        public static extern int GetDateField(
            nint thread,
            InstrumentProfileCustomFieldHandle handle,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string name,
            out int value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfileCustomFields_setDateField")]
        public static extern int SetDateField(
            nint thread,
            InstrumentProfileCustomFieldHandle handle,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string name,
            int value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfileCustomFields_getNonEmptyFieldNames")]
        public static extern unsafe int GetNonEmptyFieldsNames(
            nint thread,
            InstrumentProfileCustomFieldHandle handle,
            out ListNative<IntPtr>* fieldNames);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_JavaObjectHandler_clone")]
        public static extern int CloneHandle(
            nint thread,
            IntPtr handle,
            out InstrumentProfileCustomFieldHandle newHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_CList_String_release")]
        public static extern int ReleaseList(nint thread, nint handle);
    }
}
