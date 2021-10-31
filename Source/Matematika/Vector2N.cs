using Shtoockie.Matematika;

namespace Shtoockie.Matematika
{
    public readonly struct Vector2N
    {
        private static readonly Vector2N _zero = new Vector2N(Number.Zero, Number.Zero);
        public static Vector2N Zero => _zero;

        private readonly Number _x;
        public Number X => _x;

        private readonly Number _y;
        public Number Y => _y;

        #region Constructors

        public Vector2N(Number x, Number y)
        {
            _x = x;
            _y = y;
        }

        public Vector2N(Number value) : this(value, value)
        {
        }

        #endregion // Constructors

        #region Basic operators

        public override bool Equals(object obj)
        {
            if (obj is Vector2N vector)
            {
                return this == vector;
            }

            return false;
        }

        public bool Equals(Vector2N vector)
        {
            return this == vector;
        }

        public override int GetHashCode()
        {
            return (int)Number.ToExact(((_x << 5) + _x) ^ _y);
        }

        public override string ToString()
        {
            return _x.ToString() + "; " + _y.ToString();
        }

        public static bool operator ==(Vector2N left, Vector2N right)
        {
            return (left._x == right._x) && (left._y == right._y);
        }

        public static bool operator !=(Vector2N left, Vector2N right)
        {
            return (left._x != right._x) || (left._y != right._y);
        }

        public static Vector2N operator +(Vector2N left, Vector2N right)
        {
            return new Vector2N(left._x + right._x, left._y + right._y);
        }

        public static Vector2N operator -(Vector2N left, Vector2N right)
        {
            return new Vector2N(left._x - right._x, left._y - right._y);
        }

        public static Vector2N operator *(Vector2N left, Vector2N right)
        {
            return new Vector2N(left._x * right._x, left._y * right._y);
        }

        public static Vector2N operator *(Vector2N left, Number right)
        {
            return new Vector2N(left._x * right, left._y * right);
        }

        public static Vector2N operator *(Number left, Vector2N right)
        {
            return new Vector2N(left * right._x, left * right._y);
        }

        public static Vector2N operator /(Vector2N left, Vector2N right)
        {
            return new Vector2N(left._x / right._x, left._y / right._y);
        }

        public static Vector2N operator /(Vector2N left, Number right)
        {
            return new Vector2N(left._x / right, left._y / right);
        }

        #endregion // Basic operators

        public static Number Dot(Vector2N left, Vector2N right)
        {
            return (left._x * right._x) + (left._y * right._y);
        }

        public Vector2N Abs()
        {
            return new Vector2N(_x.Abs(), _y.Abs());
        }

        public Vector2N Normalize()
        {
            Number length = ((_x * _x) + (_y * _y)).Sqrt();
            return new Vector2N(_x / length, _y / length);
        }

        public Number LengthSquared()
        {
            return (_x * _x) + (_y * _y);
        }

        public Number Length()
        {
            return ((_x * _x) + (_y * _y)).Sqrt();
        }
    }
}
