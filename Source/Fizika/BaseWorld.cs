using Shtoockie.Matematika;

namespace Shtoockie.Fizika
{
    public abstract class BaseWorld
    {
        private static readonly Numerus _gravitation = (Numerus)9_806_650L;
        public static Numerus Gravitation => _gravitation;

        private static readonly Numerus _frictionMultiplier = (Numerus)(900_000L);
        public static Numerus FrictionMultiplier => _frictionMultiplier;

        public BaseWorld()
        {
            //eanote F=ma; Fтр=-uN; N=mg; F+Fтр=0; a=-ug; u=0,5;
        }

        public abstract void Materialize(Body body);

        public abstract void Annihilate(Body body);

        public abstract void Observe(Numerus deltaTime);
    }
}
