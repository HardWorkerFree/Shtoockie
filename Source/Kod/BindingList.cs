using System;
using System.Collections.Generic;

namespace Shtoockie.Kod
{
    public class BindingList<TItem>
    {
        private readonly List<TItem> _items;

        private readonly Binding<int> _count;
        public Binding<int> Count => _count;

        private readonly HashSet<Action<TItem>> _addHandlers;
        private readonly HashSet<Action<TItem>> _removeHandlers;

        public BindingList()
        {
            _items = new List<TItem>();
            _count = new Binding<int>(0);
            _addHandlers = new HashSet<Action<TItem>>();
            _removeHandlers = new HashSet<Action<TItem>>();
        }

        public void SubscribeAddHandler(Action<TItem> addHandler)
        {
            _addHandlers.Add(addHandler);
        }

        public void SubscribeRemoveHandler(Action<TItem> removeHandler)
        {
            _removeHandlers.Add(removeHandler);
        }

        public void UnsubscribeAddHandler(Action<TItem> addHandler)
        {
            _addHandlers.Remove(addHandler);
        }

        public void UnsubscribeRemoveHandler(Action<TItem> removeHandler)
        {
            _removeHandlers.Remove(removeHandler);
        }

        public void Add(TItem item)
        {
            _items.Add(item);

            foreach (var addHandler in _addHandlers)
            {
                addHandler(item);
            }

            _count.Value = _items.Count;
        }

        public void Remove(TItem item)
        {
            _items.Remove(item);

            foreach (var removeHandler in _removeHandlers)
            {
                removeHandler(item);
            }

            _count.Value = _items.Count;
        }

        public void Clear()
        {
            _items.Clear();
            _count.Value = _items.Count;
        }
    }
}
