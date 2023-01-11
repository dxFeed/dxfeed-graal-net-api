// <copyright file="StateChangeListenerFunc.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Api;
using static DxFeed.Graal.Net.Api.DXEndpoint;

namespace DxFeed.Graal.Net.Native.Endpoint;

/// <summary>
/// Function pointer to the endpoint state change listener.
/// Called when the state of the endpoint changes.
/// </summary>
/// <param name="thread">The pointer to a run-time data structure for the thread.</param>
/// <param name="oldState">The old endpoint <see cref="State"/>. Represents as <c>int</c> in native code.</param>
/// <param name="newState">The new endpoint <see cref="State"/>. Represents as <c>int</c> in native code.</param>
/// <param name="userData">The pointer to user data. Actually not used.</param>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate void StateChangeListenerFunc(nint thread, State oldState, State newState, nint userData);
