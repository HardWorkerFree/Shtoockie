using System.Globalization;
using System;

namespace Shtoockie.Kod
{
    public readonly struct Decimus
    {
        private const int MaximumOffsetPower = 8;
        private const int MaximumOffsetPowerNegative = -MaximumOffsetPower;

        private static readonly long[] Powers;

        public static readonly Decimus Zero;
        public static readonly Decimus One;

        private readonly int _number;
        private readonly int _exponent;

        static Decimus()
        {
            Powers = new long[]
            {
                1L,
                10L,
                100L,
                1_000L,
                10_000L,
                100_000L,
                1_000_000L,
                10_000_000L,
                100_000_000L,
                1_000_000_000L,
                10_000_000_000L,
                100_000_000_000L,
                1_000_000_000_000L,
                10_000_000_000_000L,
                100_000_000_000_000L,
                1_000_000_000_000_000L,
                10_000_000_000_000_000L,
                100_000_000_000_000_000L,
                1_000_000_000_000_000_000L,
                long.MaxValue
            };

            Zero = new Decimus(0, 0);
            One = new Decimus(1, 0);
        }

        /// <summary>
        /// 9 significant digits from ±1 x 10^-many до ±9,99999999 x 10^many
        /// </summary>
        /// <param name="number">number without delimiter</param>
        /// <param name="exponent">-many..many</param>
        public Decimus(long number, int exponent)
        {
            if (number == 0)
            {
                _number = 0;
                _exponent = 0;

                return;
            }

            _exponent = exponent + Normalize(ref number);
            _number = (int)number;
        }

        private static int Normalize(ref long number)
        {
            bool negative = number < 0;

            if (negative)
            {
                number = -number;
            }

            int exponentOffset = MaximumOffsetPower;

            while (number > Powers[exponentOffset])
            {
                exponentOffset++;
            }

            while (number < Powers[exponentOffset])
            {
                exponentOffset--;
            }

            exponentOffset -= MaximumOffsetPower;

            if (exponentOffset < 0)
            {
                number *= Powers[-exponentOffset];
            }
            else
            {
                number /= Powers[exponentOffset];
            }

            if (negative)
            {
                number = -number;
            }

            return exponentOffset;
        }

        public override string ToString()
        {
            return this.ToString(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
        }

        public string ToString(string numberDecimalSeparator)
        {
            int absoluteExponent = Math.Abs(_exponent);
            if (_exponent < 0 && _exponent > MaximumOffsetPowerNegative)
            {
                return $"{_number / Powers[absoluteExponent]}{numberDecimalSeparator}{(_number % Powers[-_exponent]).ToString().PadLeft(-_exponent, '0')}";
            }
            else
            {
                return $"{_number}E{_exponent}"; //надо 5.321123E-20
            }
        }
    }
}
