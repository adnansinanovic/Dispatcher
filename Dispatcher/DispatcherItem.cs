using System.Threading;

namespace Dispatcher
{
    public class DispatcherItem<T>
    {
        private readonly AutoResetEvent _waitEvent = new AutoResetEvent(false);

        public DispatcherItem(T item)
        {
            Item = item;
        }

        public T Item { get; private set; }

        public void WaitOne()
        {
            _waitEvent.WaitOne();
        }

        public void Set()
        {
            _waitEvent.Set();
        }
    }
}
