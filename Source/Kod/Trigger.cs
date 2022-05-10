using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shtoockie.Kod
{
    public class Trigger<TObservable> : ITrigger
    {
        private TObservable _previousObservableValue;
        public TObservable PreviousObservableValue => _previousObservableValue;

        private TObservable _currentObservableValue;
        public TObservable CurrentObservableValue => _currentObservableValue;

        private Func<TObservable> _observableValueGetter;
        private Action _observableChangeHandler;

        private Trigger(Func<TObservable> observableValueGetter, Action observableChangeHandler)
        {
            _observableValueGetter = observableValueGetter ?? DefaultGet;
            _observableChangeHandler = observableChangeHandler ?? DefaultHandle;

            _previousObservableValue = _observableValueGetter();
            _currentObservableValue = _previousObservableValue;
        }

        public Trigger(object source, string sourcePropertyName)
            : this(CreateObservableValueGetter(source, sourcePropertyName), null)
        {
        }

        public Trigger(Action observableChangeHandler)
            : this(null, observableChangeHandler)
        {
        }

        public Trigger(object source, string sourcePropertyName, Action observableChangeHandler)
            : this(CreateObservableValueGetter(source, sourcePropertyName), observableChangeHandler)
        {
        }

        public void SetupSourceValueGetter(object source, string sourcePropertyName)
        {
            _observableValueGetter = CreateObservableValueGetter(source, sourcePropertyName);
        }

        public void SetupChangeHandler(Action observableChangeHandler)
        {
            _observableChangeHandler = observableChangeHandler ?? DefaultHandle;
        }

        private static Func<TObservable> CreateObservableValueGetter(object source, string observablePropertyName)
        {
            Type sourceType = source.GetType();
            PropertyInfo sourcePropertyInfo = sourceType.GetProperty(observablePropertyName);

            if (sourcePropertyInfo == null)
            {
                return DefaultGet;
            }

            MethodInfo sourcePropertyGetter = sourcePropertyInfo.GetGetMethod();

            if (sourcePropertyGetter == null)
            {
                return DefaultGet;
            }

            return (Func<TObservable>)Delegate.CreateDelegate(typeof(Func<TObservable>), source, sourcePropertyGetter.Name);
        }

        private static TObservable DefaultGet()
        {
            return default(TObservable);
        }

        private void DefaultHandle()
        {
        }

        public void Check()
        {
            _currentObservableValue = _observableValueGetter.Invoke();

            if (!EqualityComparer<TObservable>.Default.Equals(_previousObservableValue, _currentObservableValue))
            {
                Fire();

                _previousObservableValue = _currentObservableValue;
            }
        }

        public void Fire()
        {
            _observableChangeHandler?.Invoke();
        }
    }
}
