// <copyright file="MarketEventSymbols.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Helper class to compose and parse symbols for market events.
///
/// <h3>Regional symbols</h3>
///
/// Regional symbol subscription receives events only from a designated exchange, marketplace, or venue
/// instead of receiving composite events from all venues (by default). Regional symbol is composed from a
/// <i>base symbol</i>, ampersand character ('&amp;'), and an exchange code character. For example,
/// <ul>
/// <li>"SPY" is the symbol for composite events for SPDR S&amp;P 500 ETF from all exchanges,</li>
/// <li>"SPY&amp;N" is the symbol for event for SPDR S&amp;P 500 ETF that originate only from NYSE marketplace.</li>
/// </ul>
///
/// <h3>Symbol attributes</h3>
///
/// Market event symbols can have a number of attributes attached to then in curly braces
/// with "&lt;key&gt;=&lt;value&gt;" paris separated by commas. For example,
/// <ul>
/// <li>"SPY{price=bid}" is the market symbol "SPY" with an attribute key "price" set to value "bid".</li>
/// <li>"SPY(=5m,tho=true}" is the market symbol "SPY" with two attributes. One has an empty key and
/// value "5m", while the other has key "tho" and value "true".</li>
/// </ul>
/// The methods in this class always maintain attribute keys in alphabetic order.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/market/MarketEventSymbols.html">Javadoc</a>.
/// </summary>
public static class MarketEventSymbols
{
    private const char ExchangeSeparator = '&';
    private const char AttributesOpen = '{';
    private const char AttributesClose = '}';
    private const char AttributesSeparator = ',';
    private const char AttributeValue = '=';

    /// <summary>
    /// Checks if the specified symbol has the exchange code specification.
    /// The result is <c>false</c> if symbol is <c>null</c>.
    /// </summary>
    /// <param name="symbol">The specified symbol.</param>
    /// <returns>Returns <c>true</c> is the specified symbol has the exchange code specification.</returns>
    public static bool HasExchangeCode(string? symbol) =>
        symbol != null && HasExchangeCodeInternal(symbol, GetLengthWithoutAttributesInternal(symbol));

    /// <summary>
    /// Returns exchange code of the specified symbol or <c>'\0'</c> if none is defined.
    /// The result is <c>'\0'</c> if symbol is <c>null</c>.
    /// </summary>
    /// <param name="symbol">The specified symbol.</param>
    /// <returns>Returns exchange code of the specified symbol or <c>'\0'</c> if none is defined.</returns>
    public static char GetExchangeCode(string symbol) =>
        (char)(HasExchangeCode(symbol)
            ? symbol[GetLengthWithoutAttributesInternal(symbol) - 1]
            : 0);

    /// <summary>
    /// Changes exchange code of the specified symbol or removes it
    /// if new exchange code is <c>'\0'</c>.
    /// The result is <c>null</c> if old symbol is <c>null</c>.
    /// </summary>
    /// <param name="symbol">The old symbol.</param>
    /// <param name="exchangeCode">The new exchange code.</param>
    /// <returns>Returns new symbol with the changed exchange code.</returns>
    public static string? ChangeExchangeCode(string? symbol, char exchangeCode)
    {
        if (symbol == null)
        {
            return exchangeCode == 0
                ? null
                : $"{ExchangeSeparator}{exchangeCode}";
        }

        var i = GetLengthWithoutAttributesInternal(symbol);
        var result = exchangeCode == 0
            ? GetBaseSymbolInternal(symbol, i)
            : GetBaseSymbolInternal(symbol, i) + $"{ExchangeSeparator}{exchangeCode}";
        return i == symbol.Length
            ? result
            : string.Concat(result, symbol.AsSpan(i));
    }

    /// <summary>
    /// Returns base symbol without exchange code and attributes.
    /// The result is <c>null</c> if symbol is <c>null</c>.
    /// </summary>
    /// <param name="symbol">The specified symbol.</param>
    /// <returns>Returns base symbol without exchange code and attributes.</returns>
    public static string? GetBaseSymbol(string? symbol)
    {
        if (symbol == null)
        {
            return null;
        }

        return GetBaseSymbolInternal(symbol, GetLengthWithoutAttributesInternal(symbol));
    }

    /// <summary>
    /// Changes base symbol while leaving exchange code and attributes intact.
    /// The result is <c>null</c> if old symbol is <c>null</c>.
    /// </summary>
    /// <param name="symbol">The old symbol.</param>
    /// <param name="baseSymbol">The new base symbol.</param>
    /// <returns>
    /// Returns new symbol with new base symbol and old symbol's exchange code and attributes.
    /// </returns>
    public static string ChangeBaseSymbol(string? symbol, string baseSymbol)
    {
        if (symbol == null)
        {
            return baseSymbol;
        }

        var i = GetLengthWithoutAttributesInternal(symbol);
        if (HasExchangeCodeInternal(symbol, i))
        {
            return $"{baseSymbol}{ExchangeSeparator}{symbol[i - 1]}{symbol[i..]}";
        }

        return i == symbol.Length
            ? baseSymbol
            : string.Concat(baseSymbol, symbol.AsSpan(i));
    }

    /// <summary>
    /// Checks if the specified symbol has any attributes.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>Returns <c>true</c> if the specified symbol has any attributes.</returns>
    public static bool HasAttributes(string? symbol) =>
        symbol != null && GetLengthWithoutAttributesInternal(symbol) < symbol.Length;

    /// <summary>
    /// Returns value of the attribute with the specified key.
    /// The result is <c>null</c> if attribute with the specified key is not found.
    /// The result is <c>null</c> if symbol is <c>null</c>.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="key">The attribute key.</param>
    /// <returns>Returns value of the attribute with the specified key.</returns>
    /// <exception cref="ArgumentNullException">If key is null.</exception>
    public static string? GetAttributeStringByKey(string? symbol, string key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (symbol == null)
        {
            return null;
        }

        return GetAttributeInternal(symbol, GetLengthWithoutAttributesInternal(symbol), key);
    }

    /// <summary>
    /// Changes value of one attribute value while leaving exchange code and other attributes intact.
    /// The <c>null</c> symbol is interpreted as empty one by this method.
    /// </summary>
    /// <param name="symbol">The old symbol.</param>
    /// <param name="key">The attribute key.</param>
    /// <param name="value">The attribute value.</param>
    /// <returns>
    /// Returns new symbol with key attribute with the specified value and everything else from the old symbol.
    /// </returns>
    /// <exception cref="ArgumentNullException">If key is null.</exception>
    public static string? ChangeAttributeStringByKey(string? symbol, string? key, string? value)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (symbol == null)
        {
            return value == null
                ? null
                : $"{AttributesOpen}{key}{AttributeValue}{value}{AttributesClose}";
        }

        var i = GetLengthWithoutAttributesInternal(symbol);
        if (i == symbol.Length)
        {
            return value == null
                ? symbol
                : $"{symbol}{AttributesOpen}{key}{AttributeValue}{value}{AttributesClose}";
        }

        return value == null
            ? RemoveAttributeInternal(symbol, i, key)
            : AddAttributeInternal(symbol, i, key, value);
    }

    /// <summary>
    /// Removes one attribute with the specified key while leaving exchange code and other attributes intact.
    /// The result is <c>null</c> if symbol is <c>null</c>.
    /// </summary>
    /// <param name="symbol">The old symbol.</param>
    /// <param name="key">The attribute key.</param>
    /// <returns>Returns new symbol without the specified key and everything else from the old symbol.</returns>
    /// <exception cref="ArgumentNullException">If key is null.</exception>
    public static string? RemoveAttributeStringByKey(string? symbol, string? key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (symbol == null)
        {
            return null;
        }

        return RemoveAttributeInternal(symbol, GetLengthWithoutAttributesInternal(symbol), key);
    }

    private static bool HasExchangeCodeInternal(string symbol, int length) =>
        length >= 2 && symbol[length - 2] == ExchangeSeparator;

    private static string GetBaseSymbolInternal(string symbol, int length) =>
        HasExchangeCodeInternal(symbol, length) ? symbol[..(length - 2)] : symbol[..length];

    private static bool HasAttributesInternal(string symbol, int length)
    {
        if (length >= 3 && symbol[length - 1] == AttributesClose)
        {
            var i = symbol.LastIndexOf(AttributesOpen, length - 2);
            return i >= 0 && i < length - 1;
        }

        return false;
    }

    private static int GetLengthWithoutAttributesInternal(string symbol)
    {
        var length = symbol.Length;
        return HasAttributesInternal(symbol, length) ? symbol.LastIndexOf(AttributesOpen) : length;
    }

    private static string? GetKeyInternal(string symbol, int i)
    {
        var val = symbol.IndexOf(AttributeValue, i);
        return val < 0 ? null : symbol.Substring(i, val - i);
    }

    private static int GetNextKeyInternal(string symbol, int i)
    {
        var val = symbol.IndexOf(AttributeValue, i) + 1;
        var sep = symbol.IndexOf(AttributesSeparator, val);
        return sep < 0 ? symbol.Length : sep + 1;
    }

    private static string GetValueInternal(string symbol, int i, int j)
    {
        var startPos = symbol.IndexOf(AttributeValue, i) + 1;
        var endPos = j - 1;
        return symbol.Substring(startPos, endPos - startPos);
    }

    private static string DropKeyAndValueInternal(string symbol, int length, int i, int j)
    {
        string result;
        if (j == symbol.Length)
        {
            result = i == length + 1
                ? symbol[..length]
                : string.Concat(symbol.AsSpan(0, i - 1), symbol.AsSpan(j - 1));
        }
        else
        {
            result = string.Concat(symbol.AsSpan(0, i), symbol.AsSpan(j));
        }

        return result;
    }

    private static string? GetAttributeInternal(string symbol, int length, string key)
    {
        if (length == symbol.Length)
        {
            return null;
        }

        var i = length + 1;
        while (i < symbol.Length)
        {
            var cur = GetKeyInternal(symbol, i);
            if (cur == null)
            {
                break;
            }

            var j = GetNextKeyInternal(symbol, i);
            if (key.Equals(cur, StringComparison.Ordinal))
            {
                return GetValueInternal(symbol, i, j);
            }

            i = j;
        }

        return null;
    }

    private static string RemoveAttributeInternal(string symbol, int length, string key)
    {
        if (length == symbol.Length)
        {
            return symbol;
        }

        var i = length + 1;
        while (i < symbol.Length)
        {
            var cur = GetKeyInternal(symbol, i);
            if (cur == null)
            {
                break;
            }

            var j = GetNextKeyInternal(symbol, i);
            if (key.Equals(cur, StringComparison.Ordinal))
            {
                symbol = DropKeyAndValueInternal(symbol, length, i, j);
            }
            else
            {
                i = j;
            }
        }

        return symbol;
    }

    private static string AddAttributeInternal(string symbol, int length, string key, string value)
    {
        if (length == symbol.Length)
        {
            return $"{symbol}{AttributesOpen}{key}{AttributeValue}{value}{AttributesClose}";
        }

        var i = length + 1;
        var added = false;
        while (i < symbol.Length)
        {
            var cur = GetKeyInternal(symbol, i);
            if (cur == null)
            {
                break;
            }

            var j = GetNextKeyInternal(symbol, i);
            var cmp = string.Compare(cur, key, StringComparison.Ordinal);
            switch (cmp)
            {
                case 0 when added:
                    // Drop, since we've already added this key.
                    symbol = DropKeyAndValueInternal(symbol, length, i, j);
                    break;
                case 0:
                    // Replace value.
                    symbol = symbol[..i] + $"{key}{AttributeValue}{value}" + symbol[(j - 1)..];
                    added = true;
                    i += key.Length + value.Length + 2;
                    break;
                case > 0 when !added:
                    // Insert value here.
                    symbol = symbol[..i] + $"{key}{AttributeValue}{value}{AttributesSeparator}" + symbol[i..];
                    added = true;
                    i += key.Length + value.Length + 2;
                    break;
                default:
                    i = j;
                    break;
            }
        }

        // Change this condition so that it does not always evaluate to 'false'. False positive.
#pragma warning disable S2583
        return added
            ? symbol
            : symbol[..(i - 1)] + $"{AttributesSeparator}{key}{AttributeValue}{value}" + symbol[(i - 1)..];
#pragma warning restore S2583
    }
}
