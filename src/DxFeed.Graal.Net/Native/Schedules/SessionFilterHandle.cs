// <copyright file="SessionFilterHandle.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Interop;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Schedules;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by marshaler")]
internal class SessionFilterHandle : JavaHandle
{
    public static SessionFilterHandle GetInstance(int id) =>
        SafeCall(Import.GetInstance(CurrentThread, id));

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_SessionFilter_getInstance")]
        public static extern SessionFilterHandle GetInstance(nint thread, int id);
    }
}
