using System.Globalization;

namespace Shtookie.Matematika
{
    public struct Number
    {
        private const long IntegerPart = 10_000_000;
        private const long DecimalPart = 1_000_000;
        private static string DecimalSeparator = ".";
        private static char DecimalSeparatorSymbol = '.';
        private static string DefaultView = GetView(0);

        public static Number Zero = "0.0";
        public static Number Max = "999999.999999";
        public static Number Min = "-999999.999999";

        private readonly long _number;
        private readonly string _view;

        /// <summary>
        /// Create new number.
        /// </summary>
        /// <param name="number">Number in format "-1234567.123456"</param>
        public Number(string number)
        {
            _number = 0;
            _view = null;

            if (string.IsNullOrEmpty(number))
            {
                return;
            }

            if (!double.TryParse(number, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out _))
            {
                return;
            }

            string[] parts = number.Split(DecimalSeparatorSymbol);

            _number = (long.Parse(parts[0]) % IntegerPart) * DecimalPart;

            if ((parts.Length > 1) && long.TryParse(parts[1].PadRight(6, '0').Substring(0, 6), out long decimals))
            {
                if (_number >= 0)
                {
                    _number += decimals;
                }
                else
                {
                    _number -= decimals;
                }
            }

            _view = GetView(_number);
        }

        private Number(long number)
        {
            _number = number;
            _view = GetView(_number);
        }

        private static string GetView(long number)
        {
            return (number / DecimalPart).ToString(CultureInfo.InvariantCulture) + DecimalSeparator + (number % DecimalPart).ToString(CultureInfo.InvariantCulture).PadLeft(6, '0');
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
            
            if (number > Max._number)
            {
                return Max;
            }
            else if (number < Min._number)
            {
                return Min;
            }

            return new Number(number);
        }

        public static Number operator /(Number left, Number right)
        {
            long number = (left._number * DecimalPart) / right._number;

            if (number > Max._number)
            {
                return Max;
            }
            else if (number < Min._number)
            {
                return Min;
            }

            return new Number(number);
        }

        public static Number Sqrt(Number number)
        {
            long sqrt = (long)System.Math.Sqrt(number._number * DecimalPart);

            if (sqrt > Max._number)
            {
                return Max;
            }

            return new Number(sqrt);
        }

        public override string ToString()
        {
            return _view ?? DefaultView;
        }

        public static implicit operator Number(string number) => new Number(number);
        public static implicit operator string(Number number) => number.ToString();
        public static implicit operator Number(int number) => new Number((long)number * DecimalPart);
    }
}
