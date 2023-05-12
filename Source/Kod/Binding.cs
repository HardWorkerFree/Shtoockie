using System;
using System.Collections.Generic;

namespace Shtoockie.Kod
{
    public class Binding<TValue> : IBinding<TValue> where TValue : struct
    {
		private TValue _value;
		public TValue Value
		{
			get { return _value; }
			set { Update(value); }
		}

		private readonly HashSet<Action<TValue, TValue>> _subscribers;

		public Binding(TValue initialValue)
		{
			_value = initialValue;
			_subscribers = new HashSet<Action<TValue, TValue>>();
		}

		public Binding() : this(default(TValue))
		{
		}

		public void Subscribe(Action<TValue, TValue> action)
		{
			_subscribers.Add(action);
		}

		public void Unsubscribe(Action<TValue, TValue> action)
		{
			_subscribers.Remove(action);
		}

		private void Update(TValue value)
		{
			if (!_value.Equals(value))
			{
				TValue oldValue = _value;
				_value = value;

				foreach (var subscriber in _subscribers)
				{
					subscriber(oldValue, _value);
				}
			}
		}
	}
}
