using Shtoockie.Kod;
using Shtoockie.Matematika;

namespace Shtoockie.Fizika
{
    public abstract class Body : ICode
    {
		public abstract int Code { get; }

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
		public Numerus Mass => _mass;

		private Numerus _invertedMass;

		private Numerus _normalReaction;
		public Numerus NormalReaction => _normalReaction;

		private Numerus _elasticity;
		public Numerus Elasticity => _elasticity;

        public bool IsStatic => _movement == Vector2N.Zero;

		public Body(Numerus mass, Vector2N position, Numerus elasticity)
		{
            _mass = mass;
            _invertedMass = Numerus.One / mass;
            _position = position;
			_movement = Vector2N.Zero;
			_normalReaction = _mass * BaseWorld.Gravitation;
			_elasticity = elasticity;
		}

		public void Relocate(Vector2N position)
		{
			_position = position;
		}

		public void Redirect(Vector2N movement)
		{
            if (movement == Vector2N.Zero)
            {
                _speed = Numerus.Zero;
                _direction = Vector2N.Zero;
                _movement = Vector2N.Zero;

                return;
            }

            _movement = movement;
			_speed = movement.Length();

            if (_speed <= (Numerus)1_000L) //eanote точность скорости до остановки, чтобы не было бесконечного движения.
            {
                _speed = Numerus.Zero;
                _direction = Vector2N.Zero;
                _movement = Vector2N.Zero;

                return;
            }

            _direction = movement / _speed;
		}

		public void AddForce(Vector2N force, Numerus deltaTime)
		{
			//eanote F=ma => F=mdV/dt => dV=Fdt/m
			Vector2N deltaSpeed = force * deltaTime * _invertedMass;
			Redirect(_movement + deltaSpeed);
		}

		public void AddFrictionForce(Numerus frictionMultiplier, Numerus deltaTime)
		{
            //eanote Fтр=-uN;
            Numerus frictionForce = -frictionMultiplier * _normalReaction;
			Vector2N deltaSpeed = _direction * frictionForce * deltaTime * _invertedMass;

			if (_movement.LengthSquared() < deltaSpeed.LengthSquared())
			{
				Stop();

				return;
			}

			Redirect(_movement + deltaSpeed);
        }

        public void Move(Numerus deltaTime)
		{
			_position = _position + _movement * deltaTime;
		}

		public void Stop()
		{
			Redirect(Vector2N.Zero);
        }
    }
}
