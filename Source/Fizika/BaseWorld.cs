using Shtoockie.Matematika;

namespace Shtoockie.Fizika
{
    public abstract class BaseWorld
    {
        private readonly Numerus _gravitation;
        public Numerus Gravitation => _gravitation;

        private readonly Numerus _defaultFriction;
        public Numerus DefaultFriction => _defaultFriction;

        public BaseWorld()
        {
            _gravitation = (Numerus)9_806_650L;
            //eanote F=ma; Fтр=-uN; N=mg; F+Fтр=0; a=-ug; u=0,5;
            _defaultFriction = (Numerus)(-010_000L) * _gravitation;
        }

        public abstract void Materialize(Body body);

        public abstract void Annihilate(Body body);

        public abstract void Observe(Numerus deltaTime);
    }
}
