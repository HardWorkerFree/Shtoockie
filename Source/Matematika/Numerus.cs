using System;
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

        public static readonly Numerus _one = (Numerus)1;
        public static Numerus One => _one;

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

        public Numerus Sqrt()
        {
            if (this._value <= 0L)
            {
                return Numerus.Zero;
            }

            long square = this._value * DecimalPart;
            int powerOfTwo = 19;
            long root = square >> powerOfTwo;

            while (root > 0)
            {
                if (root < 32L)
                {
                    root >>= 1;
                    powerOfTwo += 1;
                }
                else if (root < 1_024L)
                {
                    root >>= 5;
                    powerOfTwo += 5;
                }
                else if (root < 1_048_576L)
                {
                    root >>= 10;
                    powerOfTwo += 10;
                }
                else if (root < int.MaxValue)
                {
                    root >>= 15;
                    powerOfTwo += 15;
                }
                else
                {
                    root >>= 20;
                    powerOfTwo += 20;
                }
            }

            powerOfTwo >>= 1;

            //eanote use "root = (long)Math.Sqrt(square * DecimalPart);" if you want increase perfomance by ten times, but inaccurate on large numbers

            root = square >> powerOfTwo;

            long previous = square;
            long rootDivision = 1L;

            while (true)
            {
                rootDivision = square / root;
                root = rootDivision + root;
                root = root >> 1;

                if (previous == root)
                {
                    return new Numerus(root);
                }
                else if (previous < root)
                {
                    return new Numerus(previous);
                }

                previous = root;
            }
        }

        public Numerus Abs()
        {
            return (this._value >= 0) ? this : -this;
        }

        private static long Abs(long value)
        {
            //eanote return (value + (value >> 63)) ^ (value >> 63);
            return (value >= 0) ? value : -value;
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
