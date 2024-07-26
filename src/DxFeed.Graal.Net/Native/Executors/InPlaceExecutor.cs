// <copyright file="InPlaceExecutor.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Executors;

internal class InPlaceExecutor : JavaHandle
{
    public static InPlaceExecutor Create() =>
        ErrorCheck.SafeCall(Import.Create(CurrentThread));

    public void ProcessAllPendingTasks() =>
        ErrorCheck.SafeCall(Import.ProcessAllPendingTasks(CurrentThread, this));

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_ExecutorBaseOnConcurrentLinkedQueue_new")]
        public static extern InPlaceExecutor Create(nint thread);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_ExecutorBaseOnConcurrentLinkedQueue_processAllPendingTasks")]
        public static extern InPlaceExecutor ProcessAllPendingTasks(nint thread, InPlaceExecutor executor);
    }
}
