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
        private PropertyObserver<TObservable> _observer;

        private Action _observableChangeHandler;

        private Trigger(PropertyObserver<TObservable> observer, Action observableChangeHandler)
        {
            _observer = observer ?? throw new ArgumentNullException(nameof(observer));
            _observableChangeHandler = observableChangeHandler ?? DefaultHandle;
        }

        public Trigger(object source, string sourcePropertyName)
            : this(new PropertyObserver<TObservable>(source, sourcePropertyName), null)
        {
        }

        public Trigger(object source, string sourcePropertyName, Action observableChangeHandler)
            : this(new PropertyObserver<TObservable>(source, sourcePropertyName), observableChangeHandler)
        {
        }

        public void SetupChangeHandler(Action observableChangeHandler)
        {
            _observableChangeHandler = observableChangeHandler ?? DefaultHandle;
        }

        private void DefaultHandle()
        {
        }

        public void Check()
        {
            _observer.Check();

            if (_observer.IsChanged)
            {
                Fire();
            }
        }

        public void Fire()
        {
            _observableChangeHandler.Invoke();
        }
    }
}
