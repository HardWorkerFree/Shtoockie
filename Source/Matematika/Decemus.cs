using System.Globalization;

namespace Shtoockie.Matematika
{
    public readonly struct Decemus
    {
        //[1][___31___][___32___]
        private const int BitLength = 64;
        private const int SignOffset = 63;
        private const int ExponentSignDuplicatorOffset = 1;
        private const int ExponentSetOffset = 32;
        private const int ExponentGetOffset = 33;
        private const long SignBits = 0x1L;
        private const long ExponentBits = 0x7F_FF_FF_FFL;
        private const long NumberBits = 0xFF_FF_FF_FFL;
        private const long NegativeSignValue = long.MinValue;

        private const long SignificantDigits = 9L;

        private const long Power0 = 1L;
        private const long Power1 = 10L;
        private const long Power2 = 100L;
        private const long Power3 = 1_000L;
        private const long Power4 = 10_000L;
        private const long Power5 = 100_000L;
        private const long Power6 = 1_000_000L;
        private const long Power7 = 10_000_000L;
        private const long Power8 = 100_000_000L;
        private const long Power9 = 1_000_000_000L;
        private const long Power10 = 10_000_000_000L;
        private const long Power11 = 100_000_000_000L;
        private const long Power12 = 1_000_000_000_000L;
        private const long Power13 = 10_000_000_000_000L;
        private const long Power14 = 100_000_000_000_000L;
        private const long Power15 = 1_000_000_000_000_000L;
        private const long Power16 = 10_000_000_000_000_000L;
        private const long Power17 = 100_000_000_000_000_000L;
        private const long Power18 = 1_000_000_000_000_000_000L;
        private const long Limit = long.MaxValue;

        public static readonly Decemus Zero = new Decemus(0L, 0L);
        public static readonly Decemus One = new Decemus(1L, 0L);

        private readonly long _number;

        /// <summary>
        /// 9 significant digits from ±1 x 10^-many до ±9,99999999 x 10^many
        /// </summary>
        /// <param name="number">number without delimiter</param>
        /// <param name="exponent">-many..many</param>
        public Decemus(long number, long exponent)
        {
            _number = 0L;

            if (number == 0L)
            {
                return;
            }

            if (number < 0L)
            {
                number = -number;
                _number = NegativeSignValue;
            }

            #region adjustment

            if (number < Power9) //1-9
            {
                if (number >= Power8) //9
                {
                    number *= Power0;
                    number -= 0L;
                }
                else if (number < Power4) //1-4
                {
                    if (number < Power2) //1-2
                    {
                        if (number < Power1) //1
                        {
                            number *= Power8;
                            exponent -= 8L;
                        }
                        else //2
                        {
                            number *= Power7;
                            exponent -= 7L;
                        }
                    }
                    else //3-4
                    {
                        if (number < Power3) //3
                        {
                            number *= Power6;
                            exponent -= 6L;
                        }
                        else //4
                        {
                            number *= Power5;
                            exponent -= 5L;
                        }
                    }
                }
                else //5-8
                {
                    if (number < Power6) //5-6
                    {
                        if (number < Power5) //5
                        {
                            number *= Power4;
                            exponent -= 4L;
                        }
                        else //6
                        {
                            number *= Power3;
                            exponent -= 3L;
                        }
                    }
                    else //7-8
                    {
                        if (number < Power7) //7
                        {
                            number *= Power2;
                            exponent -= 2L;
                        }
                        else //8
                        {
                            number *= Power1;
                            exponent -= 1L;
                        }
                    }
                }
            }
            else //10-18 (+Limit)
            {
                if (number < Power10) //10
                {
                    number /= Power1;
                    exponent += 1L;
                }
                else if (number < Power14) //11-14
                {
                    if (number < Power12) //11-12
                    {
                        if (number < Power11) //11
                        {
                            number /= Power2;
                            exponent += 2L;
                        }
                        else //12
                        {
                            number /= Power3;
                            exponent += 3L;
                        }
                    }
                    else //13-14
                    {
                        if (number < Power13) //13
                        {
                            number /= Power4;
                            exponent += 4L;
                        }
                        else //14
                        {
                            number /= Power5;
                            exponent += 5L;
                        }
                    }
                }
                else if (number < Power18) //15-18 
                {
                    if (number < Power16) //15-16
                    {
                        if (number < Power15) //15
                        {
                            number /= Power6;
                            exponent += 6L;
                        }
                        else //16
                        {
                            number /= Power7;
                            exponent += 7L;
                        }
                    }
                    else //17-18
                    {
                        if (number < Power17) //17
                        {
                            number /= Power8;
                            exponent += 8L;
                        }
                        else //18
                        {
                            number /= Power9;
                            exponent += 9L;
                        }
                    }
                }
                else //limit 19
                {
                    number /= Power10;
                    exponent += 10L;
                }
            }

            #endregion // adjustment

            _number = ((exponent & ExponentBits) << ExponentSetOffset) | number | _number;
        }

        public override string ToString()
        {
            return this.ToString(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
        }

        public string ToString(string numberDecimalSeparator)
        {
            long exponent = (this._number << ExponentSignDuplicatorOffset) >> ExponentGetOffset;
            long thisNumber = (this._number < 0) ? -(this._number & NumberBits) : (this._number & NumberBits);
            return $"{thisNumber}E{exponent}";
            //if (_exponent < 0 && _exponent > MaximumOffsetPowerNegative)
            //{
            //    return $"{_number}E{_exponent}"; //$"{_number / Powers[absoluteExponent]}{numberDecimalSeparator}{(_number % Powers[-_exponent]).ToString().PadLeft(-_exponent, '0')}";
            //}
            //else
            //{
            //    return $"{_number}E{_exponent}"; //надо 5.321123E-20
            //}
        }

        public Decemus Add(Decemus other)
        {
            long thisNumber = this._number;
            long otherNumber = other._number;

            if (thisNumber == 0L)
            {
                return other;
            }
            else if (otherNumber == 0L)
            {
                return this;
            }

            long thisExponent = (thisNumber << ExponentSignDuplicatorOffset) >> ExponentGetOffset;
            long otherExponent = (otherNumber << ExponentSignDuplicatorOffset) >> ExponentGetOffset;
            long exponentDelta = thisExponent - otherExponent;

            thisNumber = (thisNumber < 0) ? -(thisNumber & NumberBits) : (thisNumber & NumberBits);
            otherNumber = (otherNumber < 0) ? -(otherNumber & NumberBits) : (otherNumber & NumberBits);

            bool isNegativeExponentDelta = exponentDelta < 0;

            if (isNegativeExponentDelta)
            {
                exponentDelta = -exponentDelta;
            }

            switch (exponentDelta)
            {
                case 0:
                    return new Decemus(thisNumber + otherNumber, thisExponent);
                case 1:
                    return isNegativeExponentDelta ? new Decemus(thisNumber + otherNumber * Power1, thisExponent) : new Decemus(thisNumber * Power1 + otherNumber, otherExponent);
                case 2:
                    return isNegativeExponentDelta ? new Decemus(thisNumber + otherNumber * Power2, thisExponent) : new Decemus(thisNumber * Power2 + otherNumber, otherExponent);
                case 3:
                    return isNegativeExponentDelta ? new Decemus(thisNumber + otherNumber * Power3, thisExponent) : new Decemus(thisNumber * Power3 + otherNumber, otherExponent);
                case 4:
                    return isNegativeExponentDelta ? new Decemus(thisNumber + otherNumber * Power4, thisExponent) : new Decemus(thisNumber * Power4 + otherNumber, otherExponent);
                case 5:
                    return isNegativeExponentDelta ? new Decemus(thisNumber + otherNumber * Power5, thisExponent) : new Decemus(thisNumber * Power5 + otherNumber, otherExponent);
                case 6:
                    return isNegativeExponentDelta ? new Decemus(thisNumber + otherNumber * Power6, thisExponent) : new Decemus(thisNumber * Power6 + otherNumber, otherExponent);
                case 7:
                    return isNegativeExponentDelta ? new Decemus(thisNumber + otherNumber * Power7, thisExponent) : new Decemus(thisNumber * Power7 + otherNumber, otherExponent);
                case 8:
                    return isNegativeExponentDelta ? new Decemus(thisNumber + otherNumber * Power8, thisExponent) : new Decemus(thisNumber * Power8 + otherNumber, otherExponent);
                default:
                    if (isNegativeExponentDelta)
                    {
                        return other;
                    }
                    else
                    {
                        return this;
                    }
            }
        }

        public Decemus Multiply(Decemus other)
        {
            long thisNumber = this._number;
            long otherNumber = other._number;

            if (thisNumber == 0L || otherNumber == 0L)
            {
                return Decemus.Zero;
            }

            long thisExponent = (thisNumber << ExponentSignDuplicatorOffset) >> ExponentGetOffset;
            long otherExponent = (otherNumber << ExponentSignDuplicatorOffset) >> ExponentGetOffset;

            thisNumber = (thisNumber < 0) ? -(thisNumber & NumberBits) : (thisNumber & NumberBits);
            otherNumber = (otherNumber < 0) ? -(otherNumber & NumberBits) : (otherNumber & NumberBits);

            return new Decemus(thisNumber * otherNumber, thisExponent + otherExponent);
        }
    }
}
