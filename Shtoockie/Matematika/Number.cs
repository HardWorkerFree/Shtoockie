using System;
using System.Globalization;

namespace Shtookie.Matematika
{
    public struct Number
    {
        private const long IntegerPart = 100_000; ////10_000_000
        private const long DecimalPart = 10_000;////1_000_000;
        private const int Decimals = 4;
        private const long MaxNumber = 999999999L;
        private const long MinNumber = -999999999L;

        private static string DecimalSeparator = ".";
        private static char DecimalSeparatorSymbol = '.';

        public static Number Zero = new Number(0L);
        public static Number Max = new Number(MaxNumber);
        public static Number Min = new Number(MinNumber);

        private readonly long _number;
        private string _view;

        /// <summary>
        /// Create new number.
        /// </summary>
        /// <param name="number">Number in format "-12345.1234"</param>
        public Number(string number)
        {
            _number = 0;
            _view = null;

            if (string.IsNullOrEmpty(number))
            {
                return;
            }

            string[] parts = number.Replace(',', '.').Split(DecimalSeparatorSymbol);

            _number = (long.Parse(parts[0]) % IntegerPart) * DecimalPart;

            if (parts.Length > 1)
            {
                long decimals = long.Parse(parts[1].PadRight(Decimals, '0').Substring(0, Decimals));

                if (decimals < 0)
                {
                    throw new FormatException();
                }

                if (_number >= 0)
                {
                    _number += decimals;
                }
                else
                {
                    _number -= decimals;
                }
            }

            if (_number > MaxNumber)
            {
                _number = MaxNumber;
            }
            else if (_number < MinNumber)
            {
                _number = MinNumber;
            }
        }

        private Number(long number)
        {
            _number = number;
            _view = null;

            if (_number > MaxNumber)
            {
                _number = MaxNumber;
            }
            else if (_number < MinNumber)
            {
                _number = MinNumber;
            }
        }

        public override bool Equals(object obj)
        {
            return _number.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _number.GetHashCode();
        }

        public static bool operator ==(Number left, Number right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Number left, Number right)
        {
            return !left.Equals(right);
        }

        public static Number operator +(Number left, Number right)
        {
            return new Number(left._number + right._number);
        }

        public static Number operator -(Number left, Number right)
        {
            return new Number(left._number - right._number);
        }

        public static Number operator *(Number left, Number right)
        {
            long number = (left._number * right._number) / DecimalPart;
            
            return new Number(number);
        }

        public static Number operator /(Number left, Number right)
        {
            long number = (left._number * DecimalPart) / right._number;

            return new Number(number);
        }

        public static Number Sqrt(Number number)
        {
            return new Number((long)Math.Sqrt(number._number * DecimalPart));
        }

        public static Number Abs(Number number)
        {
            return new Number(number._number >= 0 ? number._number : (-number._number));
        }

        public override string ToString()
        {
            if (_view == null)
            {
                long absNumber = _number >= 0 ? _number : (-_number);
                _view = (_number / DecimalPart).ToString(CultureInfo.InvariantCulture) + DecimalSeparator + (absNumber % DecimalPart).ToString(CultureInfo.InvariantCulture).PadLeft(Decimals, '0');
            }

            return _view;
        }

        public static implicit operator Number(string number) => new Number(number);
        public static implicit operator string(Number number) => number.ToString();
        public static implicit operator Number(int number) => new Number((long)number * DecimalPart);
    }
}
