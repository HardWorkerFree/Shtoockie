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
		public Vector2N Movement => _movement;

		private Vector2N _direction;
		public Vector2N Direction => _direction;

		private Numerus _speed;
		public Numerus Speed => _speed;

		public bool IsFixed => _movement == Vector2N.Zero;

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

		public virtual void Relocate(Vector2N position)
		{
			_position = position;
		}

		public virtual void Redirect(Vector2N movement)
		{
			_movement = movement;
			_speed = movement.Length();
			_direction = movement / _speed;
		}

		public virtual void Slowdown(Numerus deltaTime, Numerus counterforce)
		{
			_speed -= counterforce * deltaTime;

			if (_speed <= Numerus.Zero)
			{
				_speed = Numerus.Zero;
				_direction = Vector2N.Zero;
				_movement = Vector2N.Zero;

				return;
			}

			_movement = _direction * _speed;
		}

		public virtual void Move(Numerus deltaTime)
		{
			_position = _position + _movement * deltaTime;
		}
    }
}
