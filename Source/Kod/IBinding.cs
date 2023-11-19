using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shtoockie.Kod
{
    public interface IBinding<TValue>
    {
        TValue Value { get; set; }
        void Subscribe(Action<TValue, TValue> action);
        void Unsubscribe(Action<TValue, TValue> action);
    }
}
