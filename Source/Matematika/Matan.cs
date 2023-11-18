using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shtoockie.Matematika
{
    public static class Matan
    {
        private readonly static Numerus _pi = (Numerus)3_141_592L;
        public static Numerus Pi => _pi;

        private readonly static Numerus _piHalf = (Numerus)1_570_796L;
        public static Numerus PiHalf => _piHalf;

        private readonly static Numerus _piTripleByTwo = (Numerus)4_712_388L;
        public static Numerus PiTripleByTwo => _piTripleByTwo;

        private readonly static Numerus _piDouble = (Numerus)6_283_185L;
        public static Numerus PiDouble => _piDouble;

        private readonly static Numerus _degreesInOneRadian = (Numerus)180 / _pi;
        public static Numerus DegreesInOneRadian => _degreesInOneRadian;

        private static readonly Numerus[] _cosCache = new Numerus[(long)_piHalf + 1];
        private static readonly Numerus[] _arccosCache = new Numerus[(long)Numerus.One + 1];

        static Matan()
        {
            FillTrigonometryCache();
        }

        private static void FillTrigonometryCache()
        {
            Numerus clarificator = (Numerus)100_000_000;
            Numerus firstInvertedClarificator = Numerus.One / (Numerus)10_000;
            Numerus secondInvertedClarificator = Numerus.One / (Numerus)10_000;
            Numerus[] factorials = new Numerus[7];

            Numerus factorial = Numerus.One;

            for (int n = 1; n < factorials.Length; n++)
            {
                factorial *= (Numerus)(2 * n * (2 * n - 1));
                factorials[n] = factorial;
            }

            for (long i = 1; i < _cosCache.Length; i++)
            {
                if (i == 1_000)
                {
                    clarificator = (Numerus)10_000_000;
                    firstInvertedClarificator = Numerus.One / (Numerus)10_000;
                    secondInvertedClarificator = Numerus.One / (Numerus)1_000;
                }
                else if (i == 10_000)
                {
                    clarificator = (Numerus)1_000_000;
                    firstInvertedClarificator = Numerus.One / (Numerus)1_000;
                    secondInvertedClarificator = Numerus.One / (Numerus)1_000;
                }
                else if (i == 100_000)
                {
                    clarificator = (Numerus)100_000;
                    firstInvertedClarificator = Numerus.One / (Numerus)1_000;
                    secondInvertedClarificator = Numerus.One / (Numerus)1_00;
                }

                Numerus alpha = (Numerus)i;

                //eanote accuracy fix
                Numerus cosA = clarificator;

                Numerus multiplier = clarificator;
                Numerus sign;

                for (int n = 1; n < factorials.Length; n++)
                {
                    multiplier *= alpha;
                    multiplier *= alpha;
                    multiplier /= (Numerus)(2 * n * (2 * n - 1));
                    
                    if ((n % 2 == 0))
                    {
                        cosA += multiplier;
                    }
                    else
                    {
                        cosA -= multiplier;
                    }
                }

                //eanote accuracy rollback
                cosA = cosA * firstInvertedClarificator * secondInvertedClarificator;
                _cosCache[i] = cosA;
                _arccosCache[(long)cosA] = (Numerus)i;
            }

            _cosCache[0] = Numerus.One;
            _arccosCache[(long)Numerus.One] = Numerus.Zero;
            _cosCache[(long)_piHalf] = Numerus.Zero;
            _arccosCache[0] = _piHalf;
        }

        public static Numerus Cos(Numerus alpha)
        {
            alpha %= _piDouble;

            alpha = alpha.Abs();

            if (alpha <= _piHalf)
            {
                return _cosCache[(long)alpha]; //eanote I-четверть
            }

            if (alpha <= _pi)
            {
                return -_cosCache[(long)(_pi - alpha)];  //eanote II-четверть
            }
            
            if (alpha <= _piTripleByTwo)
            {
                return -_cosCache[(long)(alpha - _pi)]; //eanote III-четверть
            }

            return _cosCache[(long)(_piDouble - alpha)]; //eanote IV-четверть
        }

        public static Numerus Acos(Numerus x)
        {
            if (x >= -Numerus.One)
            {
                if (x >= Numerus.Zero)
                {
                    return _arccosCache[(long)x];
                }

                return _pi - _arccosCache[(long)x.Abs()];
            }

            throw new InvalidOperationException();
        }
    }
}
