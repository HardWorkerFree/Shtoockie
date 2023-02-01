using Shtoockie.Matematika;

namespace Shtoockie.Fizika
{
	public abstract class Body
    {
		private Vector2N _position;
		public Vector2N Position
		{
			get { return _position; }
			protected set { _position = value; }
		}

		private Vector2N _movement;
		public Vector2N Movement
		{
			get { return _movement; }
			protected set { _movement = value; }
		}

		private Numerus _mass;
		public Numerus Mass
		{
			get { return _mass; }
			protected set { _mass = value; }
		}

		public Body(Numerus mass, Vector2N position, Vector2N movement)
		{
			_mass = mass;
			_position = position;
			_movement = movement;
		}

		public abstract bool CheckIntersection(RoundBody other);

		public virtual void Move(Numerus deltaTime)
		{
			_position *= deltaTime;
		}
	}
}
