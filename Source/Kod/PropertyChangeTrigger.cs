using System;
using System.Collections.Generic;
using System.Reflection;

namespace Shtoockie.Kod
{
    public class PropertyChangeTrigger<TValue> : ITrigger
    {
        private TValue _previousValue;
        public TValue PreviousValue => _previousValue;

        private TValue _currentValue;
        public TValue CurrentValue => _currentValue;

        private Func<TValue> _sourceValueGetter;
        private Action _changeHandler;

        private PropertyChangeTrigger(Func<TValue> sourceValueGetter, Action changeHandler)
        {
            _sourceValueGetter = sourceValueGetter ?? DefaultGet;
            _changeHandler = changeHandler ?? DefaultHandle;

            _previousValue = _sourceValueGetter();
            _currentValue = _previousValue;
        }

        public PropertyChangeTrigger(object source, string sourcePropertyName)
            : this(GetSourceValueGetter(source, sourcePropertyName), null)
        {
        }

        public PropertyChangeTrigger(Action changeHandler)
            : this(null, changeHandler)
        {
        }

        public PropertyChangeTrigger(object source, string sourcePropertyName, Action changeHandler)
            : this(GetSourceValueGetter(source, sourcePropertyName), changeHandler)
        {
        }

        public void SetupSourceValueGetter(object source, string sourcePropertyName)
        {
            _sourceValueGetter = GetSourceValueGetter(source, sourcePropertyName) ?? DefaultGet;
        }

        public void SetupChangeHandler(Action changeHandler)
        {
            _changeHandler = changeHandler ?? DefaultHandle;
        }

        private static Func<TValue> GetSourceValueGetter(object source, string sourcePropertyName)
        {
            Type sourceType = source.GetType();
            PropertyInfo sourcePropertyInfo = sourceType.GetProperty(sourcePropertyName);
            MethodInfo sourcePropertyGetter = sourcePropertyInfo.GetGetMethod();
            return (Func<TValue>)Delegate.CreateDelegate(typeof(Func<TValue>), source, sourcePropertyGetter.Name);
        }

        private static TValue DefaultGet()
        {
            return default(TValue);
        }

        private static void DefaultHandle()
        {
        }

        public static implicit operator TValue(PropertyChangeTrigger<TValue> trigger) => trigger._currentValue;

        public void Check()
        {
            _currentValue = _sourceValueGetter.Invoke();

            if (!EqualityComparer<TValue>.Default.Equals(_previousValue, _currentValue))
            {
                _changeHandler();

                _previousValue = _currentValue;
            }
        }

        public void Fire()
        {
            _changeHandler();
        }
    }
}
