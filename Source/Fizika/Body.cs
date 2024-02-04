using System;

using Shtoockie.Matematika;

namespace Shtoockie.Fizika
{
	public abstract class Body
    {
		private BaseWorld _world;

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

		private Numerus _mass;
		public Numerus Mass
		{
			get { return _mass; }
			protected set { _mass = value; }
		}

		public bool IsEthereal => _world == null;

		public bool IsStatic => _movement == Vector2N.Zero;

		public Body(Numerus mass, Vector2N position)
		{
            _mass = mass;
			_position = position;
			_movement = Vector2N.Zero;
		}

		public void Materialize(BaseWorld targetWorld)
		{
			if (_world != null)
			{
				_world.RemoveBody(this);
			}

			_world = targetWorld ?? throw new ArgumentNullException(nameof(targetWorld));
			_world.AddBody(this);
		}

		public void Annihilate()
		{
			if (_world != null)
			{
				_world.RemoveBody(this);
			}

			_world = null;
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
