using System;
using System.Globalization;

namespace Shtookie.Matematika
{
    public readonly struct Number
    {
        private const long IntegerPart = 1_000_000L;
        private const long DecimalPart = 1_000L;
        private const float DecimalPartF = 1_000F;
        private const double DecimalPartD = 1_000D;
        private const decimal DecimalPartM = 1_000M;
        private const int Decimals = 3;
        private const long MaxNumber = 1_000_000_000L;
        private const long MinNumber = -1_000_000_000L;

        private static string DecimalSeparator = ".";
        private static char DecimalSeparatorSymbol = '.';

        private static readonly Number _zero = new Number(0L, true);
        public static Number Zero => _zero;

        private static readonly Number _max = new Number(MaxNumber, true);
        public static Number Max => _max;

        public static readonly Number _min = new Number(MinNumber, true);
        public static Number Min => _min;

        private readonly long _value;

        #region Constructors

        private Number(long number, bool isCorrectionRequired)
        {
            _value = isCorrectionRequired ? CorrectValue(number) : number;
        }

        public Number(int number)
        {
            _value = CorrectValue(number * DecimalPart);
        }

        public Number(float number)
        {
            _value = CorrectValue((long)(number * DecimalPartF));
        }

        public Number(double number)
        {
            _value = CorrectValue((long)(number * DecimalPartD));
        }

        public Number(decimal number)
        {
            _value = CorrectValue((long)(number * DecimalPartM));
        }

        #endregion // Constructors

        #region Implicit operators

        public static implicit operator Number(long number) => new Number(number, true);
        public static implicit operator long(Number number) => number._value;

        public static implicit operator Number(int number) => new Number(number);
        public static implicit operator Number(float number) => new Number(number);
        public static implicit operator Number(double number) => new Number(number);
        public static implicit operator Number(decimal number) => new Number(number);

        public static implicit operator int(Number number) => (int)(number._value / DecimalPart);
        public static implicit operator float(Number number) => (float)number._value / DecimalPartF;
        public static implicit operator double(Number number) => (double)number._value / DecimalPartD;
        public static implicit operator decimal(Number number) => (decimal)number._value / DecimalPartM;

        public static implicit operator string(Number number) => number.ToString();

        public static implicit operator Number(DateTime dateTime) => new Number(dateTime.Ticks / TimeSpan.TicksPerMillisecond, false);
        public static implicit operator DateTime(Number number) => Convert(number);

        public static implicit operator Number(TimeSpan timeSpan) => new Number(timeSpan.Ticks / TimeSpan.TicksPerMillisecond, false);
        public static implicit operator TimeSpan(Number number) => TimeSpan.FromTicks(number._value * TimeSpan.TicksPerMillisecond);

        #endregion // Implicit operators

        #region Basic operators

        public override bool Equals(object obj)
        {
            if (obj is Number number)
            {
                return this._value == number._value;
            }

            return false;
        }

        public bool Equals(Number number)
        {
            return this._value == number._value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return (_value / DecimalPart).ToString(CultureInfo.InvariantCulture) + DecimalSeparator + Abs(_value % DecimalPart).ToString(CultureInfo.InvariantCulture).PadLeft(Decimals, '0'); ;
        }

        public static Number operator +(Number left, Number right)
        {
            return new Number(left._value + right._value, false);
        }

        public static Number operator -(Number left, Number right)
        {
            return new Number(left._value - right._value, false);
        }

        public static Number operator *(Number left, Number right)
        {
            return new Number((left._value * right._value) / DecimalPart, false);
        }

        public static Number operator /(Number left, Number right)
        {
            return new Number((left._value * DecimalPart) / right._value, false);
        }

        public static Number operator %(Number left, Number right)
        {
            return new Number(left._value % right._value, false);
        }

        public static Number operator ++(Number number)
        {
            return new Number(number._value + DecimalPart, false);
        }

        public static Number operator --(Number left)
        {
            return new Number(left._value - DecimalPart, false);
        }

        public static Number operator -(Number left)
        {
            return new Number(-left._value, false);
        }

        public static Number operator ~(Number number)
        {
            return new Number(~number._value, false);
        }

        public static Number operator &(Number left, Number right)
        {
            return new Number(left._value & right._value, false);
        }

        public static Number operator |(Number left, Number right)
        {
            return new Number(left._value | right._value, false);
        }

        public static Number operator ^(Number left, Number right)
        {
            return new Number(left._value ^ right._value, false);
        }

        public static Number operator >> (Number left, int right)
        {
            return new Number(left._value >> right, false);
        }

        public static Number operator <<(Number left, int right)
        {
            return new Number(left._value << right, false);
        }

        public static bool operator ==(Number left, Number right)
        {
            return left._value == right._value;
        }

        public static bool operator !=(Number left, Number right)
        {
            return left._value != right._value;
        }

        public static bool operator >(Number left, Number right)
        {
            return left._value > right._value;
        }

        public static bool operator >=(Number left, Number right)
        {
            return left._value >= right._value;
        }

        public static bool operator <(Number left, Number right)
        {
            return left._value < right._value;
        }

        public static bool operator <=(Number left, Number right)
        {
            return left._value <= right._value;
        }

        #endregion // Basic operators

        public Number Sqrt()
        {
            if (this._value <= 0L)
            {
                return Number.Zero;
            }

            long square = this._value * DecimalPart;
            long root = square / 30L; //eanote little bit more than square root of 1L * Decimal part.
            long previous = root;

            for (int i = 0; i < 20; i++)
            {
                root = (root + square / root) >> 1;

                if (previous <= root)
                {
                    return new Number((previous == root) ? root : previous, false);
                }

                previous = root;
            }

            return new Number(root, false);
        }

        public Number Abs()
        {
            return new Number(Abs(this._value), false);
        }

        private static long Abs(long value)
        {
            return (value + (value >> 63)) ^ (value >> 63);
        }

        private static long CorrectValue(long value)
        {
            if (value > MaxNumber)
            {
                return MaxNumber;
            }
            else if (value < MinNumber)
            {
                return MinNumber;
            }

            return value;
        }

        private static DateTime Convert(Number number)
        {
            long ticks = number._value * TimeSpan.TicksPerMillisecond;

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
