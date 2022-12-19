// <copyright file="InputParameter.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace DxFeed.Graal.Net.Samples;

/// <summary>
///  Simple class describing input parameters
/// </summary>
/// <typeparam name="T">Input parameter type</typeparam>
public class InputParameter<T>
{
    private T _value;

    public InputParameter(T defaultValue) =>
        _value = defaultValue;

    public bool IsSet { get; private set; }

    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            IsSet = true;
        }
    }
}
