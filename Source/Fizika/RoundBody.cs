using Shtoockie.Matematika;

namespace Shtoockie.Fizika
{
    public class RoundBody : Body
    {
		private Numerus _radius;

		public Numerus Radius
		{
			get { return _radius; }
			protected set { _radius = value; }
		}

		public RoundBody(Numerus mass, Vector2N position, Vector2N movement, Numerus radius)
			: base(mass, position, movement)
		{
			_radius = radius;
		}

        public override bool CheckIntersection(RoundBody other)
        {
            Numerus distance = (this.Position + other.Position).Length();

			return (distance - this._radius - other._radius) < Numerus.Zero;
        }
    }
}
