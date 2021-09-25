using System;
using System.Globalization;

namespace Shtookie.Matematika
{
    public struct Number
    {
        private const long IntegerPart = 1000_000L;
        private const long DecimalPart = 1_000L;
        private const double DecimalPartD = 1_000D;
        private const decimal DecimalPartM = 1_000M;
        private const int Decimals = 3;
        private const long MaxNumber = 1000_000_000L;
        private const long MinNumber = -1000_000_000L;

        private static string DecimalSeparator = ".";
        private static char DecimalSeparatorSymbol = '.';

        public static Number Zero = new Number(0L);
        public static Number Max = new Number(MaxNumber);
        public static Number Min = new Number(MinNumber);

        private readonly long _value;

        #region Constructors

        private Number(long number)
        {
            _value = CorrectNumber(number);
        }

        public Number(int number)
        {
            _value = CorrectNumber(number * DecimalPart);
        }

        public Number(double number)
        {
            _value = CorrectNumber((long)(number * DecimalPartD));
        }

        public Number(decimal number)
        {
            _value = CorrectNumber((long)(number * DecimalPartM));
        }

        #endregion // Constructors

        #region Basic operators

        public static implicit operator Number(long number) => new Number(number);
        public static implicit operator long(Number number) => number._value;

        public static implicit operator Number(int number) => new Number(number);
        public static implicit operator Number(double number) => new Number(number);
        public static implicit operator Number(decimal number) => new Number(number);

        public static implicit operator int(Number number) => (int)(number._value / DecimalPart);
        public static implicit operator double(Number number) => (double)number._value / DecimalPartD;
        public static implicit operator decimal(Number number) => (decimal)number._value / DecimalPartM;

        public override bool Equals(object obj)
        {
            return _value.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return (_value / DecimalPart).ToString(CultureInfo.InvariantCulture) + DecimalSeparator + Math.Abs(_value % DecimalPart).ToString(CultureInfo.InvariantCulture).PadLeft(Decimals, '0'); ;
        }

        public static Number operator +(Number left, Number right)
        {
            return new Number(left._value + right._value);
        }

        public static Number operator -(Number left, Number right)
        {
            return new Number(left._value - right._value);
        }

        public static Number operator *(Number left, Number right)
        {
            return new Number((left._value * right._value) / DecimalPart);
        }

        public static Number operator /(Number left, Number right)
        {
            return new Number((left._value * DecimalPart) / right._value);
        }

        public static Number operator ++(Number number)
        {
            return new Number(number._value + DecimalPart);
        }

        public static Number operator --(Number left)
        {
            return new Number(left._value - DecimalPart);
        }

        public static Number operator -(Number left)
        {
            return new Number(-left._value);
        }

        public static Number operator ~(Number number)
        {
            return new Number(~number._value);
        }

        public static Number operator &(Number left, Number right)
        {
            return new Number(left._value & right._value);
        }

        public static Number operator |(Number left, Number right)
        {
            return new Number(left._value | right._value);
        }

        public static Number operator ^(Number left, Number right)
        {
            return new Number(left._value ^ right._value);
        }

        public static Number operator >> (Number left, int right)
        {
            return new Number(left._value >> right);
        }

        public static Number operator <<(Number left, int right)
        {
            return new Number(left._value << right);
        }

        public static bool operator ==(Number left, Number right)
        {
            return left._value == right._value;
        }

        public static bool operator !=(Number left, Number right)
        {
            return !(left._value == right._value);
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

        public static Number Sqrt(Number number)
        {
            if (number._value <= 0L)
            {
                return new Number(0L);
            }

            long square = number._value * DecimalPart;
            long root = square / 30L; //eanote little bit more than square root of 1L * Decimal part.
            long previous = root;

            for (int i = 0; i < 20; i++)
            {
                root = (root + square / root) >> 1;

                if (previous <= root)
                {
                    return new Number((previous == root) ? root : previous);
                }

                previous = root;
            }

            return new Number(root);
        }

        public static Number Abs(Number number)
        {
            return new Number(Math.Abs(number._value));
        }

        private static long CorrectNumber(long number)
        {
            if (number > MaxNumber)
            {
                return MaxNumber;
            }
            else if (number < MinNumber)
            {
                return MinNumber;
            }

            return number;
        }
    }
}
