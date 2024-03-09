namespace Shtoockie.Kod
{
    public class AscPair<T> where T : ICode
    {
        private readonly T _one;
        public T One => _one;

        private readonly T _other;
        public T Other => _other;

        private readonly int _orCode;
        public int OrCode => _orCode;

        public AscPair(T one, T other)
        {
            if (one.Code > other.Code)
            {
                _one = other;
                _other = one;
            }
            else if (one.Code == other.Code)
            {
                if (one.GetHashCode() > other.GetHashCode())
                {
                    _one = other;
                    _other = one;
                }
                else
                {
                    _one = one;
                    _other = other;
                }
            }
            else
            {
                _one = one;
                _other = other;
            }

            _orCode = _one.Code | _other.Code;
        }

        public override bool Equals(object obj)
        {
            if (obj is AscPair<T> other)
            {
                return (this._one.Equals(other._one)) && (this._other.Equals(other._other));
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _one.GetHashCode() * -1521134295 + _other.GetHashCode();
        }
    }
}
