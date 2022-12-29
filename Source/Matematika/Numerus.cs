﻿using System;
using System.Diagnostics;
using System.Globalization;

namespace Shtoockie.Matematika
{
    [DebuggerDisplay("{ToString()}")]
    public readonly struct Numerus
    {
        private const long DecimalPart = 1_000_000L;
        private const float DecimalPartF = 1_000_000F;
        private const double DecimalPartD = 1_000_000D;
        private const decimal DecimalPartM = 1_000_000M;
        private const long TicksPerMicrosecond = TimeSpan.TicksPerSecond / DecimalPart;
        private const int Decimals = 6;

        private static string DecimalSeparator = ",";
        private static char DecimalSeparatorSymbol = ',';

        private static readonly Numerus _zero = new Numerus(0L);
        public static Numerus Zero => _zero;

        private static readonly Numerus _max = new Numerus(long.MaxValue);
        public static Numerus Max => _max;

        public static readonly Numerus _min = new Numerus(long.MinValue);
        public static Numerus Min => _min;

        private readonly long _value;

        #region Constructors

        private Numerus(long numerus)
        {
            _value = numerus;
        }

        #endregion // Constructors

        #region Explicit operators

        public static explicit operator long(Numerus numerus) => numerus._value;
        public static explicit operator Numerus(long numerus) => new Numerus(numerus);

        public static explicit operator int(Numerus numerus) => (int)(numerus._value / DecimalPart);
        public static explicit operator Numerus(int numerus) => new Numerus((long)numerus * DecimalPart);

        #endregion // Explicit operators

        #region Implicit operators

        public static implicit operator Numerus(float numerus) => new Numerus((long)(numerus * DecimalPartF));
        public static implicit operator float(Numerus numerus) => (float)numerus._value / DecimalPartF;
        
        public static implicit operator Numerus(double numerus) => new Numerus((long)(numerus * DecimalPartD));
        public static implicit operator double(Numerus numerus) => (double)numerus._value / DecimalPartD;
        
        public static implicit operator Numerus(decimal numerus) => new Numerus((long)(numerus * DecimalPartM));
        public static implicit operator decimal(Numerus numerus) => (decimal)numerus._value / DecimalPartM;

        public static implicit operator Numerus(DateTime dateTime) => new Numerus(dateTime.Ticks / TicksPerMicrosecond);
        public static implicit operator DateTime(Numerus numerus) => Convert(numerus);

        public static implicit operator Numerus(TimeSpan timeSpan) => new Numerus(timeSpan.Ticks / TicksPerMicrosecond);
        public static implicit operator TimeSpan(Numerus numerus) => TimeSpan.FromTicks(numerus._value * TicksPerMicrosecond);

        public static implicit operator string(Numerus numerus) => numerus.ToString();

        #endregion // Implicit operators

        #region Basic operators

        public override bool Equals(object obj)
        {
            if (obj is Numerus numerus)
            {
                return this._value == numerus._value;
            }

            return false;
        }

        public bool Equals(Numerus numerus)
        {
            return this._value == numerus._value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return (_value / DecimalPart).ToString(CultureInfo.InvariantCulture) + DecimalSeparator + Abs(_value % DecimalPart).ToString(CultureInfo.InvariantCulture).PadLeft(Decimals, '0');
        }

        public static Numerus operator +(Numerus left, Numerus right)
        {
            return new Numerus(left._value + right._value);
        }

        public static Numerus operator -(Numerus left, Numerus right)
        {
            return new Numerus(left._value - right._value);
        }

        public static Numerus operator *(Numerus left, Numerus right)
        {
            return new Numerus((left._value * right._value) / DecimalPart);
        }

        public static Numerus operator /(Numerus left, Numerus right)
        {
            return new Numerus((left._value * DecimalPart) / right._value);
        }

        public static Numerus operator %(Numerus left, Numerus right)
        {
            return new Numerus(left._value % right._value);
        }

        public static Numerus operator ++(Numerus numerus)
        {
            return new Numerus(numerus._value + DecimalPart);
        }

        public static Numerus operator --(Numerus left)
        {
            return new Numerus(left._value - DecimalPart);
        }

        public static Numerus operator -(Numerus left)
        {
            return new Numerus(-left._value);
        }

        public static Numerus operator ~(Numerus numerus)
        {
            return new Numerus(~numerus._value);
        }

        public static Numerus operator &(Numerus left, Numerus right)
        {
            return new Numerus(left._value & right._value);
        }

        public static Numerus operator |(Numerus left, Numerus right)
        {
            return new Numerus(left._value | right._value);
        }

        public static Numerus operator ^(Numerus left, Numerus right)
        {
            return new Numerus(left._value ^ right._value);
        }

        public static Numerus operator >> (Numerus left, int right)
        {
            return new Numerus(left._value >> right);
        }

        public static Numerus operator <<(Numerus left, int right)
        {
            return new Numerus(left._value << right);
        }

        public static bool operator ==(Numerus left, Numerus right)
        {
            return left._value == right._value;
        }

        public static bool operator !=(Numerus left, Numerus right)
        {
            return left._value != right._value;
        }

        public static bool operator >(Numerus left, Numerus right)
        {
            return left._value > right._value;
        }

        public static bool operator >=(Numerus left, Numerus right)
        {
            return left._value >= right._value;
        }

        public static bool operator <(Numerus left, Numerus right)
        {
            return left._value < right._value;
        }

        public static bool operator <=(Numerus left, Numerus right)
        {
            return left._value <= right._value;
        }

        #endregion // Basic operators

#warning rework required
        public Numerus Sqrt()
        {
            //return Math.Sqrt(this);

            if (this._value <= 0L)
            {
                return Numerus.Zero;
            }

            long square = this._value * DecimalPart;
            long root = square << 10; //eanote little bit less than square root of 1L * Decimal part.
            long previous = root;
            long rootDivision = 1L;

            for (int i = 0; i < 44; i++)
            {
                rootDivision = square / root;
                root = rootDivision + root;
                root = root >> 1;

                if (previous <= root)
                {
                    return new Numerus((previous == root) ? root : previous);
                }

                previous = root;
            }

            return new Numerus(root);
        }

        public Numerus Abs()
        {
            return new Numerus(Abs(this._value));
        }

        private static long Abs(long value)
        {
            return (value + (value >> 63)) ^ (value >> 63);
        }

        private static DateTime Convert(Numerus numerus)
        {
            long ticks = numerus._value * TicksPerMicrosecond;

            if (ticks > 0L)
            {
                if (ticks > DateTime.MaxValue.Ticks)
                {
                    return DateTime.MaxValue;
                }

                return new DateTime(ticks);
            }

            return new DateTime(0L);
        }
    }
}
