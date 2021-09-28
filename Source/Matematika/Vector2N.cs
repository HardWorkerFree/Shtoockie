using Shtookie.Matematika;

namespace Shtoockie.Matematika
{
    public struct Vector2N
    {
        public static Vector2N Zero => new Vector2N(0L, 0L);

        public Number X;
        public Number Y;

        #region Constructors

        public Vector2N(Number x, Number y)
        {
            X = x;
            Y = y;
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

        public override int GetHashCode()
        {
            return ((X << 5) + X) ^ Y;
        }

        public static bool operator ==(Vector2N left, Vector2N right)
        {
            return (left.X == right.X) && (left.Y == right.Y);
        }

        public static bool operator !=(Vector2N left, Vector2N right)
        {
            return (left.X != right.X) || (left.Y != right.Y);
        }

        #endregion // Basic operators
    }
}
