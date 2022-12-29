using Shtoockie.Matematika;

namespace Shtoockie.Matematika
{
    public readonly struct Vector2N
    {
        private static readonly Vector2N _zero = new Vector2N(Numerus.Zero, Numerus.Zero);
        public static Vector2N Zero => _zero;

        private readonly Numerus _x;
        public Numerus X => _x;

        private readonly Numerus _y;
        public Numerus Y => _y;

        #region Constructors

        public Vector2N(Numerus x, Numerus y)
        {
            _x = x;
            _y = y;
        }

        public Vector2N(Numerus value) : this(value, value)
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
            return (((_x << 5) + _x) ^ _y).GetHashCode();
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

        public static Vector2N operator *(Vector2N left, Numerus right)
        {
            return new Vector2N(left._x * right, left._y * right);
        }

        public static Vector2N operator *(Numerus left, Vector2N right)
        {
            return new Vector2N(left * right._x, left * right._y);
        }

        public static Vector2N operator /(Vector2N left, Vector2N right)
        {
            return new Vector2N(left._x / right._x, left._y / right._y);
        }

        public static Vector2N operator /(Vector2N left, Numerus right)
        {
            return new Vector2N(left._x / right, left._y / right);
        }

        #endregion // Basic operators

        public static Numerus Dot(Vector2N left, Vector2N right)
        {
            return (left._x * right._x) + (left._y * right._y);
        }

        public Vector2N Abs()
        {
            return new Vector2N(_x.Abs(), _y.Abs());
        }

        public Vector2N Normalize()
        {
            Numerus length = ((_x * _x) + (_y * _y)).Sqrt();
            return new Vector2N(_x / length, _y / length);
        }

        public Numerus LengthSquared()
        {
            return (_x * _x) + (_y * _y);
        }

        public Numerus Length()
        {
            return ((_x * _x) + (_y * _y)).Sqrt();
        }
    }
}
