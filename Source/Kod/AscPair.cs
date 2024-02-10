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
            else
            {
                _one = one;
                _other = other;
            }

            _orCode = _one.Code | _other.Code;
        }
    }
}
