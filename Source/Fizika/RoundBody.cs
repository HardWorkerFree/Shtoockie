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

		public RoundBody(Numerus mass, Vector2N position, Numerus radius)
			: base(mass, position)
		{
			_radius = radius;
        }

        public override bool CheckIntersection(RoundBody other)
        {
            Numerus distanceSquared = (this.Position + other.Position).LengthSquared();
			Numerus minimalDistance = this._radius + other._radius;

            return (distanceSquared - (minimalDistance * minimalDistance)) < Numerus.Zero;
        }
    }
}
