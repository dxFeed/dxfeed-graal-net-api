// <copyright file="MathUtil.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// A collection of static utility methods for mathematics.
/// <br/>
/// Porting Java class <c>java.lang.Math</c> and <c>com.devexperts.util.MathUtil</c>.
/// </summary>
public static class MathUtil
{
    /// <summary>
    /// The bit representation -0.0 (negative zero).
    /// </summary>
    private static readonly long NegativeZeroBits =
        BitConverter.DoubleToInt64Bits(-0.0);

    /// <summary>
    /// Method like a <see cref="Math.Abs(int)"/>, but not throws <see cref="OverflowException"/> exception,
    /// when argument the argument is equal to the value of <see cref="int.MinValue"/>.
    /// Returns the absolute value of an int value.
    /// If the argument is not negative, the argument is returned.
    /// If the argument is negative, the negation of the argument is returned.
    /// Note that if the argument is equal to the value of <see cref="int.MinValue"/>,
    /// the most negative representable int value, the result is that same value, which is negative.
    /// </summary>
    /// <param name="a">The argument whose absolute value is to be determined.</param>
    /// <returns>The absolute value of the argument.</returns>
    public static int Abs(int a) =>
        a < 0 ? -a : a;

    /// <summary>
    /// Returns quotient according to number theory - i.e. when remainder is zero or positive.
    /// </summary>
    /// <param name="a">The dividend.</param>
    /// <param name="b">The divisor.</param>
    /// <returns>The quotient according to number theory.</returns>
    public static int Div(int a, int b)
    {
        if (a >= 0)
        {
            return a / b;
        }

        if (b >= 0)
        {
            return ((a + 1) / b) - 1;
        }

        return ((a + 1) / b) + 1;
    }

    /// <summary>
    /// Returns the largest (closest to positive infinity) <see cref="long"/> value that is less than
    /// or equal to the algebraic quotient.
    /// There is one special case, if the dividend is the <see cref="long"/>.<see cref="long.MinValue"/>
    /// and the divisor is <c>-1</c>,
    /// then integer overflow occurs and the result is equal to the <see cref="long"/>.<see cref="long.MinValue"/>.
    /// Normal integer division operates under the round to zero rounding mode (truncation).
    /// This operation instead acts under the round toward negative infinity (floor) rounding mode.
    /// The floor rounding mode gives different results than truncation when the exact result is negative.
    /// </summary>
    /// <param name="x">The dividend.</param>
    /// <param name="y">The divisor.</param>
    /// <returns>
    /// The largest (closest to positive infinity) <see cref="long"/> value that is less than
    /// or equal to the algebraic quotient.
    /// </returns>
    public static long FloorDiv(long x, long y)
    {
        var r = x / y;

        // If the signs are different and modulo not zero, round down.
        if ((x ^ y) < 0 && r * y != x)
        {
            --r;
        }

        return r;
    }

    /// <summary>
    /// Returns the floor modulus of the int arguments.
    /// </summary>
    /// <param name="x">The dividend.</param>
    /// <param name="y">The divisor.</param>
    /// <returns>
    /// The floor modulus: <code>x - (FloorDiv(x, y) * y)</code>
    /// </returns>
    public static long FloorMod(long x, long y) =>
        x - (FloorDiv(x, y) * y);

    /// <summary>
    /// Checks if the specified number is a power of two.
    /// </summary>
    /// <param name="x">The specified number.</param>
    /// <returns>Returns <c>true</c> if x represents a power of two.</returns>
    public static bool IsPowerOfTwo(long x) =>
        x > 0 && (x & (x - 1)) == 0;

    /// <summary>
    /// Checks if the specified number is a -0.0 (negative zero).
    /// </summary>
    /// <param name="x">The specified number.</param>
    /// <returns>Returns <c>true</c> if x is equals -0.0.</returns>
    public static bool IsNegativeZero(double x) =>
        BitConverter.DoubleToInt64Bits(x) == NegativeZeroBits;
}
