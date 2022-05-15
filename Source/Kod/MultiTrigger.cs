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

        private Action[] _observableChangeHandlers;

        public MultiTrigger(params Action[] observableChangeHandlers)
        {
            _observers = new List<IPropertyObserver>();
            _observableChangeHandlers = observableChangeHandlers ?? new Action[0];
        }

        public MultiTrigger Observe<TObservable>(object source, string sourcePropertyName)
        {
            IPropertyObserver observer = new PropertyObserver<TObservable>(source, sourcePropertyName);
            _observers.Add(observer);

            return this;
        }

        public void SetupChangeHandler(params Action[] observableChangeHandlers)
        {
            _observableChangeHandlers = observableChangeHandlers ?? new Action[0];
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
            for (int i = 0; i < _observableChangeHandlers.Length; i++)
            {
                _observableChangeHandlers[i]?.Invoke();
            }
        }
    }
}
