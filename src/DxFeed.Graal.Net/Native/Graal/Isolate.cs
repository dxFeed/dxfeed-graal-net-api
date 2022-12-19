// <copyright file="Isolate.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;

namespace DxFeed.Graal.Net.Native.Graal;

// ToDo Add ThreadLocal IsolateThread for detaching thread.
// ToDo Add functionality for create different Isolate.
// ToDo Create a base class with Isolate implementation and inherit all other native class.
internal sealed class Isolate
{
    private static readonly Lazy<Isolate> Lazy = new(() => new Isolate());
    private readonly nint _isolate;

    private Isolate() =>
        _isolate = CreateIsolate();

    public static Isolate Instance =>
        Lazy.Value;

    public nint IsolateThread =>
        AttachThread(_isolate);

    private static nint CreateIsolate()
    {
        ErrorCheck.GraalCall(
            GraalCreateIsolate(0, out var isolate, out _));
        return isolate;
    }

    private static nint AttachThread(nint isolate)
    {
        ErrorCheck.GraalCall(
            GraalAttachThread(isolate, out var thread));
        return thread;
    }

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "graal_create_isolate")]
    private static extern GraalErrorCode GraalCreateIsolate(
        nint param,
        out nint isolate,
        out nint thread);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "graal_attach_thread")]
    private static extern GraalErrorCode GraalAttachThread(
        nint isolate,
        out nint thread);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "graal_get_current_thread")]
    private static extern nint GraalGetCurrentThread(nint isolate);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "graal_get_isolate")]
    private static extern nint GraalGetIsolate(nint thread);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "graal_detach_thread")]
    private static extern int GraalDetachThread(nint thread);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "graal_tear_down_isolate")]
    private static extern int GraalTearDownIsolate(nint thread);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "graal_detach_all_threads_and_tear_down_isolate")]
    private static extern int GraalDetachAllThreadsAndTearDownIsolate(nint thread);
}
