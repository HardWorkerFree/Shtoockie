using Shtoockie.Matematika;

namespace Shtoockie.Fizika
{
    public class RoundBody : Body
    {
        public override int Code => World2D.RoundCode;

		private Numerus _radius;
		public Numerus Radius
		{
			get { return _radius; }
			protected set { _radius = value; }
		}

        public RoundBody(Numerus mass, Vector2N position, Numerus radius)
			: base(mass, position)
		{
			_radius = radius;
        }
    }
}
