using System;

namespace Shtoockie.Kod
{
    public class SimpleTrigger : ITrigger
    {
        private readonly Func<bool> _predicate;
        private readonly Action _handler;
        private int _fireCount;
        public int FireCount => _fireCount;

        public SimpleTrigger(Func<bool> predicate, Action handler)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _fireCount = 0;
        }

        public void Check()
        {
            if (_predicate())
            {
                Fire();
            }
        }

        public void Fire()
        {
            _fireCount++;

            _handler();
        }
    }
}
