using System.Collections.Generic;

using Shtoockie.Matematika;

namespace Shtoockie.Fizika
{
    public abstract class WorldBase
    {
        private readonly HashSet<Body> _bodies;
        public IReadOnlyCollection<Body> Bodies => _bodies;

        private readonly Numerus _gravitation;
        public Numerus Gravitation => _gravitation;

        private readonly Numerus _defaultFriction;
        public Numerus DefaultFriction => _defaultFriction;

        public WorldBase()
        {
            _bodies = new HashSet<Body>();
            _gravitation = (Numerus)9_806_650L;
            //eanote F=ma; Fтр=-uN; N=mg; F+Fтр=0; a=-ug; u=0,5;
            _defaultFriction = -Numerus.One.Halve() * _gravitation;
        }

        public void AddBody(Body body)
        {
            _bodies.Add(body);
        }

        public void RemoveBody(Body body)
        {
            _bodies.Remove(body);
        }

        public void Observe(Numerus deltaTime)
        {
            for (int i = 0; i < _bodies.Count; i++)
            {

            }

            foreach (Body body in _bodies)
            {
            }
        }
    }
}
