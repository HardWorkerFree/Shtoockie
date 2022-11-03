using System;
using System.Globalization;

namespace Shtoockie.Matematika
{
    public readonly struct Numerus
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

        private static readonly Numerus _zero = new Numerus(0L, true);
        public static Numerus Zero => _zero;

        private static readonly Numerus _max = new Numerus(MaxNumber, true);
        public static Numerus Max => _max;

        public static readonly Numerus _min = new Numerus(MinNumber, true);
        public static Numerus Min => _min;

        private readonly long _value;

        #region Constructors

        private Numerus(long number, bool isCorrectionRequired)
        {
            _value = isCorrectionRequired ? CorrectValue(number) : number;
        }

        public Numerus(int number)
        {
            _value = CorrectValue(number * DecimalPart);
        }

        public Numerus(float number)
        {
            _value = CorrectValue((long)(number * DecimalPartF));
        }

        public Numerus(double number)
        {
            _value = CorrectValue((long)(number * DecimalPartD));
        }

        public Numerus(decimal number)
        {
            _value = CorrectValue((long)(number * DecimalPartM));
        }

        public static Numerus FromExact(long number) => new Numerus(number, true);
        public static long ToExact(Numerus number) => number._value;

        public static Numerus FromInteger(int number) => new Numerus(number);
        public static int ToInteger(Numerus number) => (int)(number._value / DecimalPart);

        #endregion // Constructors

        #region Implicit operators

        public static implicit operator Numerus(float number) => new Numerus(number);
        public static implicit operator Numerus(double number) => new Numerus(number);
        public static implicit operator Numerus(decimal number) => new Numerus(number);

        public static implicit operator float(Numerus number) => (float)number._value / DecimalPartF;
        public static implicit operator double(Numerus number) => (double)number._value / DecimalPartD;
        public static implicit operator decimal(Numerus number) => (decimal)number._value / DecimalPartM;

        public static implicit operator string(Numerus number) => number.ToString();

        public static implicit operator Numerus(DateTime dateTime) => new Numerus(dateTime.Ticks / TimeSpan.TicksPerMillisecond, false);
        public static implicit operator DateTime(Numerus number) => Convert(number);

        public static implicit operator Numerus(TimeSpan timeSpan) => new Numerus(timeSpan.Ticks / TimeSpan.TicksPerMillisecond, false);
        public static implicit operator TimeSpan(Numerus number) => TimeSpan.FromTicks(number._value * TimeSpan.TicksPerMillisecond);

        #endregion // Implicit operators

        #region Basic operators

        public override bool Equals(object obj)
        {
            if (obj is Numerus number)
            {
                return this._value == number._value;
            }

            return false;
        }

        public bool Equals(Numerus number)
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

        public static Numerus operator +(Numerus left, Numerus right)
        {
            return new Numerus(left._value + right._value, false);
        }

        public static Numerus operator -(Numerus left, Numerus right)
        {
            return new Numerus(left._value - right._value, false);
        }

        public static Numerus operator *(Numerus left, Numerus right)
        {
            return new Numerus((left._value * right._value) / DecimalPart, false);
        }

        public static Numerus operator /(Numerus left, Numerus right)
        {
            return new Numerus((left._value * DecimalPart) / right._value, false);
        }

        public static Numerus operator %(Numerus left, Numerus right)
        {
            return new Numerus(left._value % right._value, false);
        }

        public static Numerus operator ++(Numerus number)
        {
            return new Numerus(number._value + DecimalPart, false);
        }

        public static Numerus operator --(Numerus left)
        {
            return new Numerus(left._value - DecimalPart, false);
        }

        public static Numerus operator -(Numerus left)
        {
            return new Numerus(-left._value, false);
        }

        public static Numerus operator ~(Numerus number)
        {
            return new Numerus(~number._value, false);
        }

        public static Numerus operator &(Numerus left, Numerus right)
        {
            return new Numerus(left._value & right._value, false);
        }

        public static Numerus operator |(Numerus left, Numerus right)
        {
            return new Numerus(left._value | right._value, false);
        }

        public static Numerus operator ^(Numerus left, Numerus right)
        {
            return new Numerus(left._value ^ right._value, false);
        }

        public static Numerus operator >> (Numerus left, int right)
        {
            return new Numerus(left._value >> right, false);
        }

        public static Numerus operator <<(Numerus left, int right)
        {
            return new Numerus(left._value << right, false);
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

        public Numerus Sqrt()
        {
            if (this._value <= 0L)
            {
                return Numerus.Zero;
            }

            long square = this._value * DecimalPart;
            long root = square / 30L; //eanote little bit more than square root of 1L * Decimal part.
            long previous = root;

            for (int i = 0; i < 20; i++)
            {
                root = (root + square / root) >> 1;

                if (previous <= root)
                {
                    return new Numerus((previous == root) ? root : previous, false);
                }

                previous = root;
            }

            return new Numerus(root, false);
        }

        public Numerus Abs()
        {
            return new Numerus(Abs(this._value), false);
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

        private static DateTime Convert(Numerus number)
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
