using Shtoockie.Matematika;

namespace Shtoockie.Fizika
{
    public class RoundBody : Body2D
    {
        public override int Code => World2D.RoundCode;

		private Numerus _radius;
		public Numerus Radius
		{
			get { return _radius; }
			protected set { _radius = value; }
		}

        public RoundBody(Numerus mass, Vector2N position, Numerus elasticity, Numerus radius)
			: base(mass, position, elasticity)
		{
			_radius = radius;
        }
    }
}
