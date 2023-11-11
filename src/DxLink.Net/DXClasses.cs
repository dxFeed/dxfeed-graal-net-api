// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

// This file is a compilation of DxFeed classes from:  https://github.com/dxFeed/dxfeed-graal-net-api
// that are also used for DxLink.

using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;


namespace DxLink.Net
{

    //
    // Summary:
    //     Quote event is a snapshot of the best bid and ask prices, and other fields that
    //     change with each quote. It represents the most recent information that is available
    //     about the best quote on the market at any given moment of time.
    //     For more details see Javadoc.
    public class DXQuote : MarketEvent
    {
        //
        // Summary:
        //     Maximum allowed sequence value. DxFeed.Graal.Net.Events.Market.Quote.Sequence
        public const int MaxSequence = 4194303;

        private long _bidTime;

        private long _askTime;

        //
        // Summary:
        //     Gets or sets sequence number of this quote to distinguish quotes that have the
        //     same DxFeed.Graal.Net.Events.Market.Quote.Time. This sequence number does not
        //     have to be unique and does not need to be sequential. Sequence can range from
        //     0 to DxFeed.Graal.Net.Events.Market.Quote.MaxSequence.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     If sequence out of range.
        public int Sequence
        {
            get
            {
                return TimeMillisSequence & 0x3FFFFF;
            }
            set
            {
                if (value < 0 || value > 4194303)
                {
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 2);
                    defaultInterpolatedStringHandler.AppendLiteral("Sequence(");
                    defaultInterpolatedStringHandler.AppendFormatted(value);
                    defaultInterpolatedStringHandler.AppendLiteral(") is < 0 or > MaxSequence(");
                    defaultInterpolatedStringHandler.AppendFormatted(4194303);
                    defaultInterpolatedStringHandler.AppendLiteral(")");
                    throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), "value");
                }

                TimeMillisSequence = (TimeMillisSequence & -4194304) | value;
            }
        }

        //
        // Summary:
        //     Gets time of the last bid or ask change. Time is measured in milliseconds between
        //     the current time and midnight, January 1, 1970 UTC.
        public long Time => MathUtil.FloorDiv(Math.Max(_bidTime, _askTime), 1000L) * 1000 + ((uint)TimeMillisSequence >> 22);

        //
        // Summary:
        //     Gets time of the last bid or ask change in nanoseconds. Time is measured in nanoseconds
        //     between the current time and midnight, January 1, 1970 UTC.
        public long TimeNanos => TimeNanosUtil.GetNanosFromMillisAndNanoPart(Time, TimeNanoPart);

        //
        // Summary:
        //     Gets or sets microseconds and nanoseconds part of time of the last bid or ask
        //     change. This method changes DxFeed.Graal.Net.Events.Market.Quote.TimeNanos result.
        public int TimeNanoPart { get; set; }

        //
        // Summary:
        //     Gets or sets time of the last bid change. Time is measured in milliseconds between
        //     the current time and midnight, January 1, 1970 UTC. This time is always transmitted
        //     with seconds precision, so the result of this method is usually a multiple of
        //     1000.
        //     You can set the actual millisecond-precision time here to publish event and the
        //     millisecond part will make the time of this quote even precise up to a millisecond.
        public long BidTime
        {
            get
            {
                return _bidTime;
            }
            set
            {
                _bidTime = value;
                RecomputeTimeMillisPart();
            }
        }

        //
        // Summary:
        //     Gets or sets bid exchange code.
        public char BidExchangeCode { get; set; }

        //
        // Summary:
        //     Gets or sets bid price.
        public double BidPrice { get; set; } = double.NaN;


        //
        // Summary:
        //     Gets or sets bid size.
        public double BidSize { get; set; } = double.NaN;


        //
        // Summary:
        //     Gets or sets time of the last ask change. Time is measured in milliseconds between
        //     the current time and midnight, January 1, 1970 UTC. This time is always transmitted
        //     with seconds precision, so the result of this method is usually a multiple of
        //     1000.
        //     You can set the actual millisecond-precision time here to publish event and the
        //     millisecond part will make the time of this quote even precise up to a millisecond.
        public long AskTime
        {
            get
            {
                return _askTime;
            }
            set
            {
                _askTime = value;
                RecomputeTimeMillisPart();
            }
        }

        //
        // Summary:
        //     Gets or sets ask exchange code.
        public char AskExchangeCode { get; set; }

        //
        // Summary:
        //     Gets or sets ask price.
        public double AskPrice { get; set; } = double.NaN;


        //
        // Summary:
        //     Gets or sets ask size.
        public double AskSize { get; set; } = double.NaN;


        //
        // Summary:
        //     Gets or sets time millis sequence. Do not sets this value directly. Change DxFeed.Graal.Net.Events.Market.Quote.Sequence
        //     and/or DxFeed.Graal.Net.Events.Market.Quote.Time.
        internal int TimeMillisSequence { get; set; }

        //
        // Summary:
        //     Initializes a new instance of the DxFeed.Graal.Net.Events.Market.Quote class.
        public DXQuote()
        {
        }

        //
        // Summary:
        //     Initializes a new instance of the DxFeed.Graal.Net.Events.Market.Quote class
        //     with the specified event symbol.
        //
        // Parameters:
        //   eventSymbol:
        //     The specified event symbol.
        public DXQuote(string? eventSymbol)
            : base(eventSymbol)
        {
        }

        //
        // Summary:
        //     Returns string representation of this quote event.
        //
        // Returns:
        //     The string representation.
        //public override string ToString()
        //{
        //    return "Quote{" + StringUtil.EncodeNullableString(base.EventSymbol) + ", eventTime=" + TimeFormat.Local.WithMillis().WithTimeZone().FormatFromMillis(base.EventTime) + ", time=" + TimeFormat.Local.WithMillis().WithTimeZone().FormatFromMillis(Time) + ", timeNanoPart=" + TimeNanoPart + ", sequence=" + Sequence + ", bidTime=" + TimeFormat.Local.WithTimeZone().FormatFromMillis(BidTime) + ", bidExchange=" + StringUtil.EncodeChar(BidExchangeCode) + ", bidPrice=" + BidPrice + ", bidSize=" + BidSize + ", askTime=" + TimeFormat.Local.WithTimeZone().FormatFromMillis(AskTime) + ", askExchange=" + StringUtil.EncodeChar(AskExchangeCode) + ", askPrice=" + AskPrice + ", askSize=" + AskSize + "}";
        //}

        private void RecomputeTimeMillisPart()
        {
            TimeMillisSequence = (TimeUtil.GetMillisFromTime(Math.Max(_askTime, _bidTime)) << 22) | Sequence;
        }
    }

    //
    // Summary:
    //     Trade event is a snapshot of the price and size of the last trade during regular
    //     trading hours and an overall day volume and day turnover. It represents the most
    //     recent information that is available about the regular last trade on the market
    //     at any given moment of time.
    //     For more details see Javadoc.
    public class DXTrade : TradeBase
    {
        //
        // Summary:
        //     Initializes a new instance of the DxFeed.Graal.Net.Events.Market.Trade class.
        public DXTrade()
        {
        }

        //
        // Summary:
        //     Initializes a new instance of the DxFeed.Graal.Net.Events.Market.Trade class
        //     with the specified event symbol.
        //
        // Parameters:
        //   eventSymbol:
        //     The specified event symbol.
        public DXTrade(string? eventSymbol)
            : base(eventSymbol)
        {
        }

        //
        // Summary:
        //     Returns string representation of this trade event.
        //
        // Returns:
        //     The string representation.
        //public override string ToString()
        //{
        //    return "Trade{" + BaseFieldsToString() + "}";
        //}
    }

    //
    // Summary:
    //     Greeks event is a snapshot of the option price, Black-Scholes volatility and
    //     greeks. It represents the most recent information that is available about the
    //     corresponding values on the market at any given moment of time.
    //     For more details see Javadoc.
    public class DXGreeks : MarketEvent
    {
        //
        // Summary:
        //     Maximum allowed sequence value. DxFeed.Graal.Net.Events.Options.Greeks.Sequence
        public const int MaxSequence = 4194303;

        public IndexedEventSource EventSource => IndexedEventSource.DEFAULT;

        public int EventFlags { get; set; }

        //
        // Summary:
        //     Gets or sets unique per-symbol index of this event. The index is composed of
        //     DxFeed.Graal.Net.Events.Options.Greeks.Time and DxFeed.Graal.Net.Events.Options.Greeks.Sequence,
        //     invocation of this method changes time and sequence. Do not use this method directly.
        //     Change DxFeed.Graal.Net.Events.Options.Greeks.Time and/or DxFeed.Graal.Net.Events.Options.Greeks.Sequence.
        public long Index { get; set; }

        //
        // Summary:
        //     Gets or sets timestamp of the event in milliseconds. Time is measured in milliseconds
        //     between the current time and midnight, January 1, 1970 UTC.
        public long Time
        {
            get
            {
                return (Index >> 32) * 1000 + ((Index >> 22) & 0x3FF);
            }
            set
            {
                Index = ((long)TimeUtil.GetSecondsFromTime(value) << 32) | ((long)TimeUtil.GetMillisFromTime(value) << 22) | (uint)Sequence;
            }
        }

        //
        // Summary:
        //     Gets or sets sequence number of this event to distinguish events that have the
        //     same DxFeed.Graal.Net.Events.Options.Greeks.Time. This sequence number does not
        //     have to be unique and does not need to be sequential. Sequence can range from
        //     0 to DxFeed.Graal.Net.Events.Options.Greeks.MaxSequence.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     If sequence out of range.
        public int Sequence
        {
            get
            {
                return (int)Index & 0x3FFFFF;
            }
            set
            {
                if (value < 0 || value > 4194303)
                {
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 2);
                    defaultInterpolatedStringHandler.AppendLiteral("Sequence(");
                    defaultInterpolatedStringHandler.AppendFormatted(value);
                    defaultInterpolatedStringHandler.AppendLiteral(") is < 0 or > MaxSequence(");
                    defaultInterpolatedStringHandler.AppendFormatted(4194303);
                    defaultInterpolatedStringHandler.AppendLiteral(")");
                    throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), "value");
                }

                Index = (Index & -4194304) | (uint)value;
            }
        }

        //
        // Summary:
        //     Gets or sets option market price.
        public double Price { get; set; } = double.NaN;


        //
        // Summary:
        //     Gets or sets Black-Scholes implied volatility of the option.
        public double Volatility { get; set; } = double.NaN;


        //
        // Summary:
        //     Gets or sets option delta. Delta is the first derivative of an option price by
        //     an underlying price.
        public double Delta { get; set; } = double.NaN;


        //
        // Summary:
        //     Gets or sets option gamma. Gamma is the second derivative of an option price
        //     by an underlying price.
        public double Gamma { get; set; } = double.NaN;


        //
        // Summary:
        //     Gets or sets option theta. Theta is the first derivative of an option price by
        //     a number of days to expiration.
        public double Theta { get; set; } = double.NaN;


        //
        // Summary:
        //     Gets or sets option rho. Rho is the first derivative of an option price by percentage
        //     interest rate.
        public double Rho { get; set; } = double.NaN;


        //
        // Summary:
        //     Gets or sets vega. Vega is the first derivative of an option price by percentage
        //     volatility.
        public double Vega { get; set; } = double.NaN;


        //
        // Summary:
        //     Initializes a new instance of the DxFeed.Graal.Net.Events.Options.Greeks class.
        public DXGreeks()
        {
        }

        //
        // Summary:
        //     Initializes a new instance of the DxFeed.Graal.Net.Events.Options.Greeks class
        //     with the specified event symbol.
        //
        // Parameters:
        //   eventSymbol:
        //     The specified event symbol.
        public DXGreeks(string? eventSymbol)
            : base(eventSymbol)
        {
        }

        //
        // Summary:
        //     Returns string representation of this greeks event.
        //
        // Returns:
        //     The string representation.
        public override string ToString()
        {
            return "Greeks{" + StringUtil.EncodeNullableString(EventSymbol) + ", eventTime=" + TimeFormat.Local.WithMillis().WithTimeZone().FormatFromMillis(EventTime) + ", eventFlags=0x" + EventFlags.ToString("x", CultureInfo.InvariantCulture) + ", time=" + TimeFormat.Local.WithMillis().WithTimeZone().FormatFromMillis(Time) + ", sequence=" + Sequence + ", price=" + Price + ", volatility=" + Volatility + ", delta=" + Delta + ", gamma=" + Gamma + ", theta=" + Theta + ", rho=" + Rho + ", vega=" + Vega + "}";
        }
    }

    #region Underlying Classes

    //
    // Summary:
    //     Provides utility methods for manipulating System.Enum.
    public static class EnumUtil
    {
        //
        // Summary:
        //     Returns an enum constant of the specified enum type with the specified value.
        //
        // Parameters:
        //   value:
        //     The specified value.
        //
        // Type parameters:
        //   T:
        //     The specified enum type.
        //
        // Returns:
        //     An enum constant of the specified enum type with the specified value.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     If the specified enum type does not have a constant with the specified value.
        public static T ValueOf<T>(int value) where T : Enum
        {
            Type typeFromHandle = typeof(T);
            if (Enum.IsDefined(typeFromHandle, value))
            {
                return (T)Enum.ToObject(typeFromHandle, value);
            }

            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 2);
            defaultInterpolatedStringHandler.AppendFormatted(typeFromHandle);
            defaultInterpolatedStringHandler.AppendLiteral(" has no value(");
            defaultInterpolatedStringHandler.AppendFormatted(value);
            defaultInterpolatedStringHandler.AppendLiteral(")");
            throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), "value");
        }

        //
        // Summary:
        //     Returns an enum constant of the specified enum type with the specified value,
        //     or a default value if the specified enum type does not have a constant with the
        //     specified value.
        //
        // Parameters:
        //   value:
        //     The specified value.
        //
        //   defaultValue:
        //     The default enum value.
        //
        // Type parameters:
        //   T:
        //     The specified enum type.
        //
        // Returns:
        //     The enum constant of the specified enum type with the specified value or default
        //     value, if specified enum type has no constant with the specified value.
        public static T ValueOf<T>(int value, T defaultValue) where T : Enum
        {
            Type typeFromHandle = typeof(T);
            if (Enum.IsDefined(typeFromHandle, value))
            {
                return (T)Enum.ToObject(typeFromHandle, value);
            }

            return defaultValue;
        }

        //
        // Summary:
        //     Gets the number of values for the specified enum type.
        //
        // Type parameters:
        //   T:
        //     The specified enum type.
        //
        // Returns:
        //     Returns the number of values of the specified enum type.
        public static int GetCountValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Length;
        }

        //
        // Summary:
        //     Creates an array containing elements of the specified enum type T, of the specified
        //     length. If the length is greater than the number of enum values, the remaining
        //     elements are filled with a default value, otherwise array are truncated.
        //
        // Parameters:
        //   defaultValue:
        //     The default value that will fill the elements of an array if its size is greater
        //     than the number of enum values.
        //
        //   length:
        //     The length of result array, must be power of 2.
        //
        // Type parameters:
        //   T:
        //     The specified enum type.
        //
        // Returns:
        //     The created array.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     If length is not power of 2.
        //
        // Remarks:
        //     The elements of the array are sorted by the binary values of the enumeration
        //     constants (that is, by their unsigned magnitude).
        public static T[] BuildEnumBitMaskArrayByValue<T>(T defaultValue, int length) where T : Enum
        {
            if (!MathUtil.IsPowerOfTwo(length))
            {
                throw new ArgumentException("Length must be power of 2", "length");
            }

            T[] array = (T[])Enum.GetValues(typeof(T));
            T[] array2 = new T[length];
            Array.Fill(array2, defaultValue);
            Array.Copy(array, array2, Math.Min(array.Length, length));
            return array2;
        }

        //
        // Summary:
        //     Creates an array containing elements of the specified enum type T, where the
        //     length of the array is rounded to the nearest power of two, which is greater
        //     than or equal to the number of enum values. If the calculated length is greater
        //     than the number of enum values, the remaining elements are filled with a default
        //     value. The idea is to quickly convert an int value to an enum value, simply by
        //     array index. But the size of the array is limited by a bit mask, so if the number
        //     of enum values is not a multiple of a power of two, you need to expand the array
        //     and fill in new elements with a default value.
        //
        // Parameters:
        //   defaultValue:
        //     The default value that will fill the elements of an array if its size is greater
        //     than the number of enum values.
        //
        // Type parameters:
        //   T:
        //     The specified enum type.
        //
        // Returns:
        //     The created array.
        //
        // Remarks:
        //     The elements of the array are sorted by the binary values of the enumeration
        //     constants (that is, by their unsigned magnitude).
        public static T[] BuildEnumBitMaskArrayByValue<T>(T defaultValue) where T : Enum
        {
            return BuildEnumBitMaskArrayByValue(defaultValue, (int)BitOperations.RoundUpToPowerOf2((uint)GetCountValues<T>()));
        }
    }

    //
    // Summary:
    //     Source identifier for DxFeed.Graal.Net.Events.IIndexedEvent.
    //     For more details see Javadoc.
    public class IndexedEventSource
    {
        //
        // Summary:
        //     The default source with zero identifier for all events that do not support multiple
        //     sources.
        public static readonly IndexedEventSource DEFAULT = new IndexedEventSource(0, "DEFAULT");

        //
        // Summary:
        //     Gets a source identifier. Source identifier is non-negative.
        public int Id { get; }

        //
        // Summary:
        //     Gets a name of identifier.
        public string Name { get; }

        //
        // Summary:
        //     Initializes a new instance of the DxFeed.Graal.Net.Events.IndexedEventSource
        //     class.
        //
        // Parameters:
        //   id:
        //     The identifier.
        //
        //   name:
        //     The name of identifier.
        public IndexedEventSource(int id, string name)
        {
            Id = id;
            Name = name;
        }

        //
        // Summary:
        //     Returns a string representation of the object.
        //
        // Returns:
        //     A string representation of the object.
        public override string ToString()
        {
            return Name;
        }

        //
        // Summary:
        //     Indicates whether some other indexed event source has the same id.
        //
        // Parameters:
        //   obj:
        //     The object to compare with the current object.
        //
        // Returns:
        //     true if the specified object is equal to the current object; otherwise, false.
        public override bool Equals(object? obj)
        {
            if (obj != this)
            {
                IndexedEventSource indexedEventSource = obj as IndexedEventSource;
                if (indexedEventSource != null)
                {
                    return Id == indexedEventSource.Id;
                }

                return false;
            }

            return true;
        }

        //
        // Summary:
        //     Returns a hash code value for this object. The result of this method is equal
        //     to DxFeed.Graal.Net.Events.IndexedEventSource.Id.
        //
        // Returns:
        //     A hash code value for this object.
        public override int GetHashCode()
        {
            return Id;
        }
    }

    //
    // Summary:
    //     Direction of the price movement. For example tick direction for last trade price.
    //     For more details see Javadoc.
    public enum Direction
    {
        //
        // Summary:
        //     Direction is undefined, unknown or inapplicable. It includes cases with undefined
        //     price value or when direction computation was not performed.
        Undefined,
        //
        // Summary:
        //     Current price is lower than previous price.
        Down,
        //
        // Summary:
        //     Current price is the same as previous price and is lower than the last known
        //     price of different value.
        ZeroDown,
        //
        // Summary:
        //     Current price is equal to the only known price value suitable for price direction
        //     computation. Unlike DxFeed.Graal.Net.Events.Market.Direction.Undefined the DxFeed.Graal.Net.Events.Market.Direction.Zero
        //     direction implies that current price is defined and direction computation was
        //     duly performed but has failed to detect any upward or downward price movement.
        //     It is also reported for cases when price sequence was broken and direction computation
        //     was restarted anew.
        Zero,
        //
        // Summary:
        //     Current price is the same as previous price and is higher than the last known
        //     price of different value.
        ZeroUp,
        //
        // Summary:
        //     Current price is higher than previous price.
        Up
    }
    //
    // Summary:
    //     Class extension for DxFeed.Graal.Net.Events.Market.Direction enum.
    internal static class DirectionExt
    {
        private static readonly Direction[] Values = EnumUtil.BuildEnumBitMaskArrayByValue(Direction.Undefined);

        //
        // Summary:
        //     Returns an enum constant of the DxFeed.Graal.Net.Events.Market.Direction by integer
        //     code bit pattern.
        //
        // Parameters:
        //   value:
        //     The specified value.
        //
        // Returns:
        //     The enum constant of the specified enum type with the specified value.
        public static Direction ValueOf(int value)
        {
            return Values[value];
        }
    }
    //
    // Summary:
    //     A collection of utility methods for bitwise operations.
    //     Porting Java class com.dxfeed.event.market.Util.
    public static class BitUtil
    {
        //
        // Summary:
        //     Extracts bits from the specified value.
        //
        // Parameters:
        //   value:
        //     The specified value.
        //
        //   mask:
        //     The bit mask.
        //
        //   shift:
        //     The bit shift.
        //
        // Returns:
        //     The extracted bits.
        public static int GetBits(int value, int mask, int shift)
        {
            return (value >> shift) & mask;
        }

        //
        // Summary:
        //     Sets bits to the specified value.
        //
        // Parameters:
        //   value:
        //     The specified value.
        //
        //   mask:
        //     The bit mask.
        //
        //   shift:
        //     The bit shift.
        //
        //   bits:
        //     The bits set.
        //
        // Returns:
        //     Returns a value with bits set.
        public static int SetBits(int value, int mask, int shift, int bits)
        {
            return (value & ~(mask << shift)) | ((bits & mask) << shift);
        }
    }
    //
    // Summary:
    //     Base class for common fields of DxFeed.Graal.Net.Events.Market.Trade and DxFeed.Graal.Net.Events.Market.TradeETH
    //     events. Trade events represent the most recent information that is available
    //     about the last trade on the market at any given moment of time.
    //     For more details see Javadoc.
    public abstract class TradeBase : MarketEvent
    {
        //
        // Summary:
        //     Maximum allowed sequence value. DxFeed.Graal.Net.Events.Market.TradeBase.Sequence
        public const int MaxSequence = 4194303;

        private const int DirectionMask = 7;

        private const int DirectionShift = 1;

        private const int Eth = 1;

        //
        // Summary:
        //     Gets or sets time and sequence of last trade packaged into single long value.
        //     Do not set this property directly. Sets DxFeed.Graal.Net.Events.Market.TradeBase.Time
        //     and/or DxFeed.Graal.Net.Events.Market.TradeBase.Sequence.
        public long TimeSequence { get; set; }

        //
        // Summary:
        //     Gets or sets time of the last trade. Time is measured in milliseconds between
        //     the current time and midnight, January 1, 1970 UTC.
        public long Time
        {
            get
            {
                return (TimeSequence >> 32) * 1000 + ((TimeSequence >> 22) & 0x3FF);
            }
            set
            {
                TimeSequence = ((long)TimeUtil.GetSecondsFromTime(value) << 32) | ((long)TimeUtil.GetMillisFromTime(value) << 22) | (uint)Sequence;
            }
        }

        public DateTime RealTime { get; set; }

        //
        // Summary:
        //     Gets or sets time of the last trade in nanoseconds. Time is measured in nanoseconds
        //     between the current time and midnight, January 1, 1970 UTC.
        public long TimeNanos
        {
            get
            {
                return TimeNanosUtil.GetNanosFromMillisAndNanoPart(Time, TimeNanoPart);
            }
            set
            {
                Time = TimeNanosUtil.GetNanoPartFromNanos(value);
                TimeNanoPart = TimeNanosUtil.GetNanoPartFromNanos(value);
            }
        }

        //
        // Summary:
        //     Gets or sets microseconds and nanoseconds time part of the last trade.
        public int TimeNanoPart { get; set; }

        //
        // Summary:
        //     Gets or sets sequence number of the last trade to distinguish trades that have
        //     the same DxFeed.Graal.Net.Events.Market.TradeBase.Time. This sequence number
        //     does not have to be unique and does not need to be sequential. Sequence can range
        //     from 0 to DxFeed.Graal.Net.Events.Market.TradeBase.MaxSequence.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     If sequence out of range.
        public int Sequence
        {
            get
            {
                return (int)(TimeSequence & 0x3FFFFF);
            }
            set
            {
                if (value < 0 || value > 4194303)
                {
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 2);
                    defaultInterpolatedStringHandler.AppendLiteral("Sequence(");
                    defaultInterpolatedStringHandler.AppendFormatted(value);
                    defaultInterpolatedStringHandler.AppendLiteral(") is < 0 or > MaxSequence(");
                    defaultInterpolatedStringHandler.AppendFormatted(4194303);
                    defaultInterpolatedStringHandler.AppendLiteral(")");
                    throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), "value");
                }

                TimeSequence = (TimeSequence & -4194304) | (uint)value;
            }
        }

        //
        // Summary:
        //     Gets or sets exchange code of the last trade.
        public char ExchangeCode { get; set; }

        //
        // Summary:
        //     Gets or sets price of the last trade.
        public double Price { get; set; } = double.NaN;


        //
        // Summary:
        //     Gets or sets size of this last trade event as floating number with fractions.
        public double Size { get; set; } = double.NaN;


        //
        // Summary:
        //     Gets or sets identifier of the current trading day. Identifier of the day is
        //     the number of days passed since January 1, 1970.
        public int DayId { get; set; }

        //
        // Summary:
        //     Gets or sets total volume traded for a day as floating number with fractions.
        public double DayVolume { get; set; } = double.NaN;


        //
        // Summary:
        //     Gets or sets total turnover traded for a day. Day VWAP can be computed with DxFeed.Graal.Net.Events.Market.TradeBase.DayTurnover
        //     / DxFeed.Graal.Net.Events.Market.TradeBase.DayVolume.
        public double DayTurnover { get; set; } = double.NaN;


        //
        // Summary:
        //     Gets or sets tick direction of the last trade.
        public Direction TickDirection
        {
            get
            {
                return DirectionExt.ValueOf(BitUtil.GetBits(Flags, 7, 1));
            }
            set
            {
                Flags = BitUtil.SetBits(Flags, 7, 1, (int)value);
            }
        }

        //
        // Summary:
        //     Gets or sets a value indicating whether last trade was in extended trading hours.
        public bool IsExtendedTradingHours
        {
            get
            {
                return (Flags & 1) != 0;
            }
            set
            {
                Flags = (value ? (Flags | 1) : (Flags & -2));
            }
        }

        //
        // Summary:
        //     Gets or sets change of the last trade.
        public double Change { get; set; } = double.NaN;


        //
        // Summary:
        //     Gets or sets implementation-specific flags. Do not use this method directly.
        internal int Flags { get; set; }

        //
        // Summary:
        //     Initializes a new instance of the DxFeed.Graal.Net.Events.Market.TradeBase class.
        protected TradeBase()
        {
        }

        //
        // Summary:
        //     Initializes a new instance of the DxFeed.Graal.Net.Events.Market.TradeBase class
        //     with the specified event symbol.
        //
        // Parameters:
        //   eventSymbol:
        //     The specified event symbol.
        protected TradeBase(string? eventSymbol)
            : base(eventSymbol)
        {
        }

        //
        // Summary:
        //     Returns string representation of this base trade event's.
        //
        // Returns:
        //     The string representation.
        //public string ToString()
        //{
        //    return GetType().Name + "{" + BaseFieldsToString() + "}";
        //}

        //
        // Summary:
        //     Returns string representation of this trade fields.
        //
        // Returns:
        //     The string representation.
        protected string BaseFieldsToString()
        {
            return StringUtil.EncodeNullableString(EventSymbol) + ", eventTime=" + TimeFormat.Local.WithMillis().WithTimeZone().FormatFromMillis(EventTime) + ", time=" + TimeFormat.Local.WithMillis().WithTimeZone().FormatFromMillis(Time) + ", timeNanoPart=" + TimeNanoPart + ", sequence=" + Sequence + ", exchange=" + StringUtil.EncodeChar(ExchangeCode) + ", price=" + Price + ", change=" + Change + ", size=" + Size + ", day=" + DayUtil.GetYearMonthDayByDayId(DayId) + ", dayVolume=" + DayVolume + ", dayTurnover=" + DayTurnover + ", direction=" + TickDirection.ToString() + ", ETH=" + IsExtendedTradingHours;
        }
    }

    //
    // Summary:
    //     A collection of static utility methods for manipulation of System.Int32 day id,
    //     that is the number of days since Unix epoch of January 1, 1970.
    //     Porting Java class com.devexperts.util.DayUtil.
    public static class DayUtil
    {
        private static readonly int[] DayOfYear = new int[14]
        {
            0, 0, 31, 59, 90, 120, 151, 181, 212, 243,
            273, 304, 334, 365
        };

        //
        // Summary:
        //     Returns day identifier for specified year, month and day in Gregorian calendar.
        //     The day identifier is defined as the number of days since Unix epoch of January
        //     1, 1970. Month must be between 1 and 12 inclusive. Year and day might take arbitrary
        //     values assuming proleptic Gregorian calendar. The value returned by this method
        //     for an arbitrary day value always satisfies the following equality:
        //     GetDayIdByYearMonthDay(year, month, day) == GetDayIdByYearMonthDay(year, month,
        //     0) + day
        //
        // Parameters:
        //   year:
        //     The year.
        //
        //   month:
        //     The month between 1 and 12 inclusive.
        //
        //   day:
        //     The dat.
        //
        // Returns:
        //     The day id.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     f the month is less than 1 or greater than 12.
        public static int GetDayIdByYearMonthDay(int year, int month, int day)
        {
            if (month < 1 || month > 12)
            {
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
                defaultInterpolatedStringHandler.AppendLiteral("Invalid month ");
                defaultInterpolatedStringHandler.AppendFormatted(month);
                throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), "month");
            }

            int num = DayOfYear[month] + day - 1;
            if (month > 2 && year % 4 == 0 && (year % 100 != 0 || year % 400 == 0))
            {
                num++;
            }

            return year * 365 + MathUtil.Div(year - 1, 4) - MathUtil.Div(year - 1, 100) + MathUtil.Div(year - 1, 400) + num - 719527;
        }

        //
        // Summary:
        //     Returns day identifier for specified yyyymmdd integer in Gregorian calendar.
        //     The day identifier is defined as the number of days since Unix epoch of January
        //     1, 1970. The yyyymmdd integer is equal to yearSign * (abs(year) * 10000 + month
        //     * 100 + day), where year, month, and day are in Gregorian calendar, month is
        //     between 1 and 12 inclusive, and day is counted from 1.
        //
        // Parameters:
        //   yyyymmdd:
        //     The yyyymmdd integer in Gregorian calendar.
        //
        // Returns:
        //     The day id.
        public static int GetDayIdByYearMonthDay(int yyyymmdd)
        {
            if (yyyymmdd < 0)
            {
                return GetDayIdByYearMonthDay(-(-yyyymmdd / 10000), -yyyymmdd / 100 % 100, -yyyymmdd % 100);
            }

            return GetDayIdByYearMonthDay(yyyymmdd / 10000, yyyymmdd / 100 % 100, yyyymmdd % 100);
        }

        //
        // Summary:
        //     Gets yyyymmdd integer in Gregorian calendar for a specified day identifier. The
        //     day identifier is defined as the number of days since Unix epoch of January 1,
        //     1970. The result is equal to:
        //     yearSign * (abs(year) * 10000 + month * 100 + day)
        //     where year, month, and day are in Gregorian calendar, month is between 1 and
        //     12 inclusive, and day is counted from 1.
        //
        // Parameters:
        //   dayId:
        //     A number of whole days since Unix epoch of January 1, 1970.
        //
        // Returns:
        //     The yyyymmdd integer in Gregorian calendar.
        public static int GetYearMonthDayByDayId(int dayId)
        {
            int num = dayId + 2472632;
            int num2 = MathUtil.Div(num, 146097);
            int num3 = num - num2 * 146097;
            int num4 = (num3 / 36524 + 1) * 3 / 4;
            int num5 = num3 - num4 * 36524;
            int num6 = num5 / 1461;
            int num7 = num5 - num6 * 1461;
            int num8 = (num7 / 365 + 1) * 3 / 4;
            int num9 = num7 - num8 * 365;
            int num10 = num2 * 400 + num4 * 100 + num6 * 4 + num8;
            int num11 = (num9 * 5 + 308) / 153 - 2;
            int num12 = num9 - (num11 + 4) * 153 / 5 + 122;
            int num13 = num10 - 4800 + (num11 + 2) / 12;
            int num14 = (num11 + 2) % 12 + 1;
            int num15 = num12 + 1;
            int num16 = MathUtil.Abs(num13) * 10000 + num14 * 100 + num15;
            if (num13 < 0)
            {
                return -num16;
            }

            return num16;
        }
    }

    //
    // Summary:
    //     A collection of static utility methods for mathematics.
    //     Porting Java class java.lang.Math and com.devexperts.util.MathUtil.
    public static class MathUtil
    {
        //
        // Summary:
        //     The bit representation -0.0 (negative zero).
        private static readonly long NegativeZeroBits = BitConverter.DoubleToInt64Bits(-0.0);

        //
        // Summary:
        //     Method like a System.Math.Abs(System.Int32), but not throws System.OverflowException
        //     exception, when argument the argument is equal to the value of System.Int32.MinValue.
        //     Returns the absolute value of an int value. If the argument is not negative,
        //     the argument is returned. If the argument is negative, the negation of the argument
        //     is returned. Note that if the argument is equal to the value of System.Int32.MinValue,
        //     the most negative representable int value, the result is that same value, which
        //     is negative.
        //
        // Parameters:
        //   a:
        //     The argument whose absolute value is to be determined.
        //
        // Returns:
        //     The absolute value of the argument.
        public static int Abs(int a)
        {
            if (a >= 0)
            {
                return a;
            }

            return -a;
        }

        //
        // Summary:
        //     Returns quotient according to number theory - i.e. when remainder is zero or
        //     positive.
        //
        // Parameters:
        //   a:
        //     The dividend.
        //
        //   b:
        //     The divisor.
        //
        // Returns:
        //     The quotient according to number theory.
        public static int Div(int a, int b)
        {
            if (a >= 0)
            {
                return a / b;
            }

            if (b >= 0)
            {
                return (a + 1) / b - 1;
            }

            return (a + 1) / b + 1;
        }

        //
        // Summary:
        //     Returns the largest (closest to positive infinity) System.Int64 value that is
        //     less than or equal to the algebraic quotient. There is one special case, if the
        //     dividend is the System.Int64.System.Int64.MinValue and the divisor is -1, then
        //     integer overflow occurs and the result is equal to the System.Int64.System.Int64.MinValue.
        //     Normal integer division operates under the round to zero rounding mode (truncation).
        //     This operation instead acts under the round toward negative infinity (floor)
        //     rounding mode. The floor rounding mode gives different results than truncation
        //     when the exact result is negative.
        //
        // Parameters:
        //   x:
        //     The dividend.
        //
        //   y:
        //     The divisor.
        //
        // Returns:
        //     The largest (closest to positive infinity) System.Int64 value that is less than
        //     or equal to the algebraic quotient.
        public static long FloorDiv(long x, long y)
        {
            long num = x / y;
            if ((x ^ y) < 0 && num * y != x)
            {
                num--;
            }

            return num;
        }

        //
        // Summary:
        //     Returns the floor modulus of the int arguments.
        //
        // Parameters:
        //   x:
        //     The dividend.
        //
        //   y:
        //     The divisor.
        //
        // Returns:
        //     The floor modulus:
        //     x - (FloorDiv(x, y) * y)
        public static long FloorMod(long x, long y)
        {
            return x - FloorDiv(x, y) * y;
        }

        //
        // Summary:
        //     Checks if the specified number is a power of two.
        //
        // Parameters:
        //   x:
        //     The specified number.
        //
        // Returns:
        //     Returns true if x represents a power of two.
        public static bool IsPowerOfTwo(long x)
        {
            if (x > 0)
            {
                return (x & (x - 1)) == 0;
            }

            return false;
        }

        //
        // Summary:
        //     Checks if the specified number is a -0.0 (negative zero).
        //
        // Parameters:
        //   x:
        //     The specified number.
        //
        // Returns:
        //     Returns true if x is equals -0.0.
        public static bool IsNegativeZero(double x)
        {
            return BitConverter.DoubleToInt64Bits(x) == NegativeZeroBits;
        }
    }

    public static class StringUtil
    {
        //
        // Summary:
        //     Encodes specified nullable string. If string equals null, returns "null" string;
        //     otherwise returns specified string.
        //
        // Parameters:
        //   s:
        //     The specified string.
        //
        // Returns:
        //     Return specified string or "null" string.
        public static string EncodeNullableString(string? s)
        {
            return s ?? "null";
        }

        //
        // Summary:
        //     Encodes char to string. If the value of char falls within the range of printable
        //     ASCII characters [32-126], then returns a string containing that character, otherwise
        //     return unicode number "(\uffff)". For zero char returns "\0".
        //
        // Parameters:
        //   c:
        //     The char.
        //
        // Returns:
        //     Returns the encoded string.
        public static string EncodeChar(char c)
        {
            if (c >= ' ' && c <= '~')
            {
                return c.ToString();
            }

            if (c != 0)
            {
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 1);
                defaultInterpolatedStringHandler.AppendLiteral("\\u");
                defaultInterpolatedStringHandler.AppendFormatted((c + 65536).ToString("x", CultureInfo.InvariantCulture).AsSpan(1));
                return defaultInterpolatedStringHandler.ToStringAndClear();
            }

            return "\\0";
        }

        //
        // Summary:
        //     Check that the specified char fits in the bit mask.
        //
        // Parameters:
        //   c:
        //     The char.
        //
        //   mask:
        //     The bit mask.
        //
        //   name:
        //     The char name. Used in the exception message.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     If the specified char dont fits in the mask.
        public static void CheckChar(char c, int mask, string name)
        {
            if ((c & ~mask) != 0)
            {
                ThrowInvalidChar(c, name);
            }
        }

        private static void ThrowInvalidChar(char c, string name)
        {
            throw new ArgumentException("Invalid " + name + ": " + EncodeChar(c));
        }
    }

    //
    // Summary:
    //     A collection of static utility methods for manipulation of time measured in milliseconds
    //     since Unix epoch.
    //     Porting Java class com.devexperts.util.TimeUtil.
    public static class TimeUtil
    {
        //
        // Summary:
        //     Number of milliseconds in a second.
        public const long Second = 1000L;

        //
        // Summary:
        //     Number of milliseconds in a minute.
        public const long Minute = 60000L;

        //
        // Summary:
        //     Number of milliseconds in an hour.
        public const long Hour = 3600000L;

        //
        // Summary:
        //     Number of milliseconds in a day.
        public const long Day = 86400000L;

        //
        // Summary:
        //     Returns correct number of seconds with proper handling negative values and overflows.
        //     Idea is that number of milliseconds shall be within [0..999] interval so that
        //     the following equation always holds:
        //     GetSecondsFromTime(timeMillis) * 1000L + GetMillisFromTime(timeMillis) == timeMillis
        //     as as long the time in seconds fits into System.Int32. DxFeed.Graal.Net.Utils.TimeUtil.GetMillisFromTime(System.Int64)
        //
        // Parameters:
        //   timeMillis:
        //     The time measured in milliseconds since Unix epoch.
        //
        // Returns:
        //     The number of seconds.
        public static int GetSecondsFromTime(long timeMillis)
        {
            if (timeMillis < 0)
            {
                return (int)Math.Max((timeMillis + 1) / 1000 - 1, -2147483648L);
            }

            return (int)Math.Min(timeMillis / 1000, 2147483647L);
        }

        //
        // Summary:
        //     Returns correct number of milliseconds with proper handling negative values.
        //     Idea is that number of milliseconds shall be within [0..999] interval so that
        //     the following equation always holds:
        //     GetSecondsFromTime(timeMillis) * 1000L + GetMillisFromTime(timeMillis) == timeMillis
        //     as as long the time in seconds fits into System.Int32. DxFeed.Graal.Net.Utils.TimeUtil.GetSecondsFromTime(System.Int64)
        //
        // Parameters:
        //   timeMillis:
        //     The time measured in milliseconds since Unix epoch.
        //
        // Returns:
        //     The number of milliseconds.
        public static int GetMillisFromTime(long timeMillis)
        {
            return (int)MathUtil.FloorMod(timeMillis, 1000L);
        }
    }

    //
    // Summary:
    //     Abstract base class for all market events. All market events are objects that
    //     extend this class. Market event classes are simple beans with setter and getter
    //     methods for their properties and minimal business logic. All market events have
    //     DxFeed.Graal.Net.Events.Market.MarketEvent.EventSymbol property that is defined
    //     by this class.
    //     For more details see Javadoc.
    public abstract class MarketEvent
    {
        public string? EventSymbol { get; set; }

        public long EventTime { get; set; }

        //
        // Summary:
        //     Initializes a new instance of the DxFeed.Graal.Net.Events.Market.MarketEvent
        //     class.
        protected MarketEvent()
        {
        }

        //
        // Summary:
        //     Initializes a new instance of the DxFeed.Graal.Net.Events.Market.MarketEvent
        //     class with the specified event symbol.
        //
        // Parameters:
        //   eventSymbol:
        //     The event symbol.
        protected MarketEvent(string? eventSymbol)
        {
            EventSymbol = eventSymbol;
        }
    }

    //
    // Summary:
    //     A collection of static utility methods for manipulation of time measured in nanoseconds
    //     since Unix epoch.
    //     Porting Java class com.dxfeed.event.impl.TimeNanosUtil.
    public static class TimeNanosUtil
    {
        //
        // Summary:
        //     Number of nanoseconds in millisecond.
        private const long NanosInMillis = 1000000L;

        //
        // Summary:
        //     Returns time measured in nanoseconds since Unix epoch from the time in milliseconds
        //     and its nano part. The result of this method is timeMillis * 1_000_000 + timeNanoPart.
        //
        // Parameters:
        //   timeMillis:
        //     The time in milliseconds since Unix epoch.
        //
        //   timeNanoPart:
        //     The nanoseconds part that shall lie within [0..999999] interval.
        //
        // Returns:
        //     The time measured in nanoseconds since Unix epoch.
        public static long GetNanosFromMillisAndNanoPart(long timeMillis, int timeNanoPart)
        {
            return timeMillis * 1000000 + timeNanoPart;
        }

        //
        // Summary:
        //     Returns time measured in milliseconds since Unix epoch from the time in nanoseconds.
        //     Idea is that nano part of time shall be within [0..999999] interval so that the
        //     following equation always holds:
        //     GetMillisFromNanos(timeNanos) * 1_000_000 + GetNanoPartFromNanos(timeNanos) ==
        //     timeNanos
        //     DxFeed.Graal.Net.Utils.TimeNanosUtil.GetNanoPartFromNanos(System.Int64)
        //
        // Parameters:
        //   timeNanos:
        //     The time measured in nanoseconds since Unix epoch.
        //
        // Returns:
        //     The time measured in milliseconds since Unix epoch.
        public static long GetMillisFromNanos(long timeNanos)
        {
            return MathUtil.FloorDiv(timeNanos, 1000000L);
        }

        //
        // Summary:
        //     Returns nano part of time. Idea is that nano part of time shall be within [0..999999]
        //     interval so that the following equation always holds:
        //     GetMillisFromNanos(timeNanos) * 1_000_000 + GetNanoPartFromNanos(timeNanos) ==
        //     timeNanos
        //     DxFeed.Graal.Net.Utils.TimeNanosUtil.GetMillisFromNanos(System.Int64)
        //
        // Parameters:
        //   timeNanos:
        //     The time measured in nanoseconds since Unix epoch.
        //
        // Returns:
        //     The time measured in milliseconds since Unix epoch.
        public static int GetNanoPartFromNanos(long timeNanos)
        {
            return (int)MathUtil.FloorMod(timeNanos, 1000000L);
        }
    }

    //
    // Summary:
    //     Utility class for parsing and formatting dates and times in ISO-compatible format.
    public class TimeFormat
    {
        //
        // Summary:
        //     Default format string. Example: 20090615-134530.
        private const string DefaultFormat = "yyyyMMdd-HHmmss";

        //
        // Summary:
        //     Format string for only date representation. Example: 20090615.
        private const string OnlyDateFormat = "yyyyMMdd";

        //
        // Summary:
        //     Format string with milliseconds.
        private const string WithMillisFormat = ".fff";

        //
        // Summary:
        //     Format string with TimeZone.
        private const string WithTimeZoneFormat = "zzz";

        //
        // Summary:
        //     Full ISO format string. Example: 2009-06-15T13:45:30.0000000Z.
        private const string FullIsoFormat = "o";

        //
        // Summary:
        //     Sortable date/time format string. Example: 2009-06-15T13:45:30.
        private const string SortableFormat = "s";

        //
        // Summary:
        //     Universal format string. Example: 2009-06-15 13:45:30Z.
        private const string UniversalFormat = "u";

        //
        // Summary:
        //     List all available format.
        private static readonly List<string> AvailableFormats = new List<string>
        {
            "yyyyMMdd-HHmmss", "yyyyMMdd-HHmmss.fff", "yyyyMMdd-HHmmsszzz", "yyyyMMdd-HHmmssZ", "yyyyMMdd-HHmmss.fffzzz", "yyyyMMdd-HHmmss.fffZ", "yyyyMMdd", "yyyyMMddzzz", "yyyyMMddZ", "o",
            "s", "u"
        };

        //
        // Summary:
        //     Lazy initialization of the DxFeed.Graal.Net.Utils.TimeFormat with Local Time
        //     Zone.
        private static readonly Lazy<TimeFormat> LocalTimeZoneFormat = new Lazy<TimeFormat>(() => Create(() => TimeZoneInfo.Local));

        //
        // Summary:
        //     Lazy initialization of the DxFeed.Graal.Net.Utils.TimeFormat with UTC Time Zone.
        private static readonly Lazy<TimeFormat> UtcTimeZoneFormat = new Lazy<TimeFormat>(() => Create(() => TimeZoneInfo.Utc));

        private readonly Func<TimeZoneInfo> _timeZone;

        private readonly TimeFormat _withMillis;

        private readonly TimeFormat _withTimeZone;

        private readonly TimeFormat _asOnlyDate;

        private readonly TimeFormat _asFullIso;

        private readonly string _formatString;

        //
        // Summary:
        //     Gets instance DxFeed.Graal.Net.Utils.TimeFormat with System.TimeZoneInfo.System.TimeZoneInfo.Local.
        public static TimeFormat Local => LocalTimeZoneFormat.Value;

        //
        // Summary:
        //     Gets instance DxFeed.Graal.Net.Utils.TimeFormat with System.TimeZoneInfo.System.TimeZoneInfo.Utc.
        public static TimeFormat Utc => UtcTimeZoneFormat.Value;

        private TimeFormat(Func<TimeZoneInfo> timeZone, TimeFormat? withMillis, TimeFormat? withTimeZone, TimeFormat? asOnlyDate, TimeFormat? asFullIso)
        {
            _timeZone = timeZone;
            _withMillis = withMillis ?? this;
            _withTimeZone = withTimeZone ?? this;
            _asOnlyDate = asOnlyDate ?? this;
            _asFullIso = asFullIso ?? this;
            _formatString = CreateFormatString();
        }

        //
        // Summary:
        //     Creates a new instance of DxFeed.Graal.Net.Utils.TimeFormat with a specified
        //     System.TimeZoneInfo and DxFeed.Graal.Net.Utils.TimeFormat.DefaultFormat.
        //
        // Parameters:
        //   timeZone:
        //     The specified System.TimeZoneInfo.
        //
        // Returns:
        //     Returns new instance DxFeed.Graal.Net.Utils.TimeFormat.
        public static TimeFormat Create(TimeZoneInfo timeZone)
        {
            TimeZoneInfo timeZone2 = timeZone;
            return Create(() => timeZone2);
        }

        //
        // Summary:
        //     Creates a new instance of DxFeed.Graal.Net.Utils.TimeFormat with a specified
        //     System.TimeZoneInfo encapsulates in System.Func`1 and DxFeed.Graal.Net.Utils.TimeFormat.DefaultFormat.
        //     The System.TimeZoneInfo is made as a func because, you should always access the
        //     local time zone through the TimeZoneInfo.Local (same for TimeZoneInfo.Utc) property
        //     rather than assigning the local time zone to a TimeZoneInfo object variable.
        //
        // Parameters:
        //   timeZone:
        //     The specified System.TimeZoneInfo encapsulates in System.Func`1.
        //
        // Returns:
        //     Returns new instance DxFeed.Graal.Net.Utils.TimeFormat.
        public static TimeFormat Create(Func<TimeZoneInfo> timeZone)
        {
            TimeFormat asFullIso = new TimeFormat(timeZone, null, null, null, null);
            TimeFormat asOnlyDate = new TimeFormat(timeZone, null, null, null, asFullIso);
            TimeFormat timeFormat = new TimeFormat(timeZone, null, null, asOnlyDate, asFullIso);
            TimeFormat withTimeZone = new TimeFormat(timeZone, timeFormat, null, asOnlyDate, asFullIso);
            TimeFormat withMillis = new TimeFormat(timeZone, null, timeFormat, asOnlyDate, asFullIso);
            return new TimeFormat(timeZone, withMillis, withTimeZone, asOnlyDate, asFullIso);
        }

        //
        // Summary:
        //     Converts the specified string representation of a date and time to its System.DateTimeOffset
        //     in current System.TimeZoneInfo and System.Globalization.CultureInfo.InvariantCulture.
        //     If no time zone is specified in the parsed string, the string is assumed to denote
        //     a local time, and converted to current System.TimeZoneInfo.
        //     It accepts the following formats.
        //     0 is parsed as zero time in UTC. <long-value-in-milliseconds> The value in milliseconds
        //     since Unix epoch since Unix epoch. It should be positive and have at least 9
        //     digits (otherwise it could not be distinguished from date in format 'yyyymmdd').
        //     Each date since 1970-01-03 can be represented in this form. <date>[<time>][<timezone>]
        //     If time is missing it is supposed to be '00:00:00'. <date> is one of: yyyy-MM-dd
        //     yyyyMMdd <time> is one of: HH:mm:ss[.sss] HHmmss[.sss] <timezone> is one of:
        //     [+-]HH:mm [+-]HHmm Z for UTC.
        //
        // Parameters:
        //   value:
        //     The input value for parse.
        //
        // Returns:
        //     Returns System.DateTimeOffset parsed from input value.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     If input value has wrong format.
        public DateTimeOffset Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Cannot parse date-time from empty or null string", "value");
            }

            value = value.Trim();
            if (value.Equals("0", StringComparison.Ordinal))
            {
                return ConvertDateTimeToCurrentTimeZone(DateTimeOffset.FromUnixTimeMilliseconds(0L));
            }

            DateTimeOffset result;
            foreach (string availableFormat in AvailableFormats)
            {
                if (DateTimeOffset.TryParseExact(value, availableFormat, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out result))
                {
                    return ConvertDateTimeToCurrentTimeZone(result);
                }
            }

            if (DateTimeOffset.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out result))
            {
                return ConvertDateTimeToCurrentTimeZone(result);
            }

            if (long.TryParse(value, out var result2))
            {
                return ConvertDateTimeToCurrentTimeZone(DateTimeOffset.FromUnixTimeMilliseconds(result2));
            }

            throw new ArgumentException("Cannot parse date-time from input string: \"" + value + "\"");
        }

        //
        // Summary:
        //     Adds milliseconds to the current format string.
        //
        // Returns:
        //     Returns DxFeed.Graal.Net.Utils.TimeFormat.
        public TimeFormat WithMillis()
        {
            return _withMillis;
        }

        //
        // Summary:
        //     Adds Time Zone to the current format string.
        //
        // Returns:
        //     Returns DxFeed.Graal.Net.Utils.TimeFormat.
        public TimeFormat WithTimeZone()
        {
            return _withTimeZone;
        }

        //
        // Summary:
        //     Sets the current format as Only Date.
        //
        // Returns:
        //     Returns DxFeed.Graal.Net.Utils.TimeFormat.
        public TimeFormat AsOnlyDate()
        {
            return _asOnlyDate;
        }

        //
        // Summary:
        //     Sets the current format as Full Iso.
        //
        // Returns:
        //     Returns DxFeed.Graal.Net.Utils.TimeFormat.
        public TimeFormat AsFullIso()
        {
            return _asFullIso;
        }

        //
        // Summary:
        //     Converts the value in seconds since Unix epoch to its equivalent string representation
        //     using the current format DxFeed.Graal.Net.Utils.TimeFormat.CreateFormatString,
        //     current System.TimeZoneInfo and System.Globalization.CultureInfo.InvariantCulture.
        //
        // Parameters:
        //   timeSeconds:
        //     The time measured in seconds since Unix epoch.
        //
        // Returns:
        //     The string representation of the date, or "0" if timeSeconds is 0.
        public string FormatFromSeconds(long timeSeconds)
        {
            return FormatFromMillis(timeSeconds * 1000);
        }

        //
        // Summary:
        //     Converts the value in milliseconds since Unix epoch to its equivalent string
        //     representation using the current format DxFeed.Graal.Net.Utils.TimeFormat.CreateFormatString,
        //     current System.TimeZoneInfo and System.Globalization.CultureInfo.InvariantCulture.
        //
        // Parameters:
        //   timeMillis:
        //     The time measured in milliseconds since Unix epoch.
        //
        // Returns:
        //     The string representation of the date, or "0" if timeMillis is 0.
        public string FormatFromMillis(long timeMillis)
        {
            if (timeMillis == 0L)
            {
                return "0";
            }

            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timeMillis);
            return Format(dateTimeOffset);
        }

        //
        // Summary:
        //     Converts the value of the specified System.DateTimeOffset object to its equivalent
        //     string representation using the current format DxFeed.Graal.Net.Utils.TimeFormat.CreateFormatString,
        //     current System.TimeZoneInfo and System.Globalization.CultureInfo.InvariantCulture.
        //
        // Parameters:
        //   dateTimeOffset:
        //     The System.DateTimeOffset object.
        //
        // Returns:
        //     The string representation.
        public string Format(DateTimeOffset dateTimeOffset)
        {
            dateTimeOffset = ConvertDateTimeToCurrentTimeZone(dateTimeOffset);
            return dateTimeOffset.ToString(_formatString, CultureInfo.InvariantCulture);
        }

        //
        // Summary:
        //     Converts specified System.DateTimeOffset to System.DateTimeOffset in current
        //     Time Zone.
        //
        // Parameters:
        //   dateTimeOffset:
        //     The specified System.DateTimeOffset.
        //
        // Returns:
        //     Returns System.DateTimeOffset in current Time Zone.
        private DateTimeOffset ConvertDateTimeToCurrentTimeZone(DateTimeOffset dateTimeOffset)
        {
            return TimeZoneInfo.ConvertTime(dateTimeOffset, _timeZone());
        }

        //
        // Summary:
        //     Creates a format string for the System.DateTimeOffset.System.DateTimeOffset.ToString
        //     method.
        //
        // Returns:
        //     Returns format string.
        private string CreateFormatString()
        {
            if (_asFullIso == this)
            {
                return "o";
            }

            if (_asOnlyDate == this)
            {
                return "yyyyMMdd";
            }

            string text = ((_withMillis == this) ? ".fff" : string.Empty);
            string text2 = ((_withTimeZone == this) ? "zzz" : string.Empty);
            return "yyyyMMdd-HHmmss" + text + text2;
        }
    }
    #endregion

}
