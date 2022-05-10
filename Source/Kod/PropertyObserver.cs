using System;
using System.Collections.Generic;
#if DEBUG
using System.Diagnostics;
#endif
using System.Reflection;

namespace Shtoockie.Kod
{
    public class PropertyObserver<TObservable> : IPropertyObserver
    {
        private readonly Func<TObservable> _observableValueGetter;

        private TObservable _previousObservableValue;
        public TObservable PreviousObservableValue => _previousObservableValue;

        private TObservable _currentObservableValue;
        public TObservable CurrentObservableValue => _currentObservableValue;

        private bool _isChanged;
        public bool IsChanged => _isChanged;

        private PropertyObserver(Func<TObservable> observableValueGetter)
        {
            _observableValueGetter = observableValueGetter ?? DefaultGet;

            _previousObservableValue = _observableValueGetter();
            _currentObservableValue = _previousObservableValue;
        }

        public PropertyObserver(object source, string sourcePropertyName)
            : this(CreateObservableValueGetter(source, sourcePropertyName))
        {
        }

        private static Func<TObservable> CreateObservableValueGetter(object source, string observablePropertyName)
        {
            ArgumentNullException.ThrowIfNull(source, nameof(source));
            ArgumentNullException.ThrowIfNull(observablePropertyName, nameof(observablePropertyName));

            Type sourceType = source.GetType();
            PropertyInfo sourcePropertyInfo = sourceType.GetProperty(observablePropertyName);

            if (sourcePropertyInfo == null)
            {
#if DEBUG
                Debug.WriteLine($"'{source}'.'{observablePropertyName}' is missed.");
#endif
                return DefaultGet;
            }

            MethodInfo sourcePropertyGetter = sourcePropertyInfo.GetGetMethod();

            if (sourcePropertyGetter == null)
            {
#if DEBUG
                Debug.WriteLine($"'{source}'.'{observablePropertyName}' is not public.");
#endif
                return DefaultGet;
            }

            return (Func<TObservable>)Delegate.CreateDelegate(typeof(Func<TObservable>), source, sourcePropertyGetter.Name);
        }

        private static TObservable DefaultGet()
        {
            return default(TObservable);
        }

        public void Check()
        {
            _currentObservableValue = _observableValueGetter.Invoke();
            _isChanged = !EqualityComparer<TObservable>.Default.Equals(_previousObservableValue, _currentObservableValue);
            _previousObservableValue = _currentObservableValue;
        }
    }
}
