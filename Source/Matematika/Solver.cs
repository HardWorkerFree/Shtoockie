namespace Shtoockie.Matematika
{
    public static class Solver
    {
        /// <summary>
        /// ax^2+bx+c=0. В ответе первый корень всегда больше второго (так удобнее).
        /// </summary>
        public static Numerus[] SolveQuadraticEquation(Numerus a, Numerus b, Numerus c)
        {
            if (a == Numerus.Zero)
            {
                if (b == Numerus.Zero)
                {
                    return new Numerus[0];
                }

                //eanote bx+c=0 => x=-c/b;
                return new Numerus[1] { -(c / b) };
            }

            //eanote D = b^2 - 4ac
            Numerus d = (b * b) - (a * c * (Numerus)4);

            //eanote D > 0 => x1,x2 = (-b +/- Sqrt(D)) / 2a
            if (d > Numerus.Zero)
            {
                Numerus sqrtD = d.Sqrt();
                Numerus doubleA = a.Redouble();
                Numerus firstRoot = ((-b) + sqrtD) / doubleA;
                Numerus secondRoot = ((-b) - sqrtD) / doubleA;

                return (firstRoot >= secondRoot) ? new Numerus[2] { firstRoot, secondRoot } : new Numerus[2] { secondRoot, firstRoot };
            }

            //eanote D = 0 => x=-(b/2a)
            if (d == Numerus.Zero)
            {
                return new Numerus[1] { -(b / ((Numerus)2 * a)) };
            }

            //eanote D < 0 => 0
            return new Numerus[0];
        }
    }
}
