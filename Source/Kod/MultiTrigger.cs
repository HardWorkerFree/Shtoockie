using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shtoockie.Kod
{
    public class MultiTrigger : ITrigger
    {
        readonly List<IPropertyObserver> _observers;

        private Action _observableChangeHandler;

        public MultiTrigger(Action observableChangeHandler)
        {
            _observers = new List<IPropertyObserver>();
            _observableChangeHandler = observableChangeHandler ?? DefaultHandle;
        }

        public MultiTrigger Observe<TObservable>(object source, string sourcePropertyName)
        {
            IPropertyObserver observer = new PropertyObserver<TObservable>(source, sourcePropertyName);
            _observers.Add(observer);

            return this;
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
            bool isAnyChanged = false;
            for (int i = 0; i < _observers.Count; i++)
            {
                _observers[i].Check();
                isAnyChanged |= _observers[i].IsChanged;
            }

            if (isAnyChanged)
            {
                Fire();
            }
        }

        public void Fire()
        {
            _observableChangeHandler?.Invoke();
        }
    }
}
