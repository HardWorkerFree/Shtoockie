namespace Shtoockie.Literatura
{
    public struct Text
    {   private readonly bool _hasValue;
        private readonly string _value;

        public static Text Empty = new Text(); 

        #region Constructors

        public Text(string text)
        {
            _hasValue = !string.IsNullOrEmpty(text);
            _value = _hasValue ? text : string.Empty;
        }

        #endregion // Constructors

        #region Basic operators

        public static implicit operator Text(string text) => new Text(text);
        public static implicit operator string(Text text) => text._hasValue ? text._value : string.Empty;

        public override bool Equals(object obj)
        {
            if (obj is Text text)
            {
                return this._value == text._value;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _hasValue ? _value.GetHashCode() : string.Empty.GetHashCode();
        }

        public static bool operator ==(Text left, Text right)
        {
            return left._value == right._value;
        }

        public static bool operator !=(Text left, Text right)
        {
            return !(left._value == right._value);
        }

        public static Text operator +(Text left, Text right)
        {
            return new Text(left._value + right._value);
        }

        #endregion // Basic operators
    }
}
