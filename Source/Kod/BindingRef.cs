using System;
using System.Collections.Generic;

namespace Shtoockie.Kod
{
    public class BindingRef<TValue> : IBinding<TValue>, IReadonlyBinding<TValue> where TValue : class
    {
		private TValue _value;
		public TValue Value
		{
			get { return _value; }
			set { Update(value); }
		}

		private readonly HashSet<Action<TValue, TValue>> _subscribers;

		public BindingRef(TValue initialValue = null)
		{
			_value = initialValue;
			_subscribers = new HashSet<Action<TValue, TValue>>();
		}

		public void Subscribe(Action<TValue, TValue> action)
		{
			_subscribers.Add(action);
		}

		public void Unsubscribe(Action<TValue, TValue> action)
		{
			_subscribers.Remove(action);
		}

		public void Update(TValue value)
		{
			if (_value != value)
			{
				TValue oldValue = _value;
				_value = value;

				foreach (var subscriber in _subscribers)
				{
					subscriber(oldValue, _value);
				}
			}
		}

		public void ForcedUpdate(TValue value)
		{
            TValue oldValue = _value;
            _value = value;

            foreach (var subscriber in _subscribers)
            {
                subscriber(oldValue, _value);
            }
        }

		public void SilentUpdate(TValue value)
		{
            _value = value;
        }
	}
}
