using System;

namespace Shtoockie.Kod
{
    public interface IReadonlyBinding<TValue>
    {
        TValue Value { get; }
        void Subscribe(Action<TValue, TValue> action);
        void Unsubscribe(Action<TValue, TValue> action);
    }
}
