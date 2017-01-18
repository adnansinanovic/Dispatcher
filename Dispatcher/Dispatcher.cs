using System;
using System.Collections.Generic;
using System.Threading;

namespace Dispatcher
{
    public class Dispatcher : Dispatcher<object>
    {
        public Dispatcher(Action<object> itemProcessed, Action<object> itemAdded) : base(itemProcessed, itemAdded)
        {
        }
    }

    public class Dispatcher<T> : IDisposable where T : class
    {
        private readonly Queue<DispatcherItem<T>> _items;
        private readonly object _lock;
        private readonly ManualResetEvent _resetEvent;
        private readonly Thread _thread;
        private bool _running;

        public Action<T> ItemProcessed;
        public Action<T> ItemAdded;

        public Dispatcher(Action<T> itemProcessed, Action<T> itemAdded = null)
        {
            ItemProcessed = itemProcessed;

            _lock = new object();
            _items = new Queue<DispatcherItem<T>>();

            _resetEvent = new ManualResetEvent(false);
            _thread = new Thread(Worker);            
            _thread.IsBackground = true;
            _thread.Start();
        }

        public void Dispose()
        {
            _running = true;

            _resetEvent.Set();

            _thread.Join();

            _items.Clear();
        }

        private void OnItemAdded(T item)
        {
            ItemAdded?.Invoke(item);
        }

        private void OnItemProcessed(T item)
        {
            ItemProcessed?.Invoke(item);
        }

        private void Worker(object obj)
        {
            while (!_running)
            {
                _resetEvent.WaitOne();

                DispatcherItem<T> dispatcherItem = null;

                lock (_lock)
                {
                    if (_items.Count > 0)
                        dispatcherItem = _items.Dequeue();

                    if (_items.Count == 0)
                        _resetEvent.Reset();
                }

                if (dispatcherItem != null)
                {
                    dispatcherItem.Set();
                    OnItemProcessed(dispatcherItem.Item);
                }
            }
        }

        public void Add(T item, bool wait = false)
        {
            DispatcherItem<T> consumerProducerItem = new DispatcherItem<T>(item);
            lock (_lock)
            {
                _items.Enqueue(consumerProducerItem);

                OnItemAdded(item);

                _resetEvent.Set();
            }

            if (wait)
                consumerProducerItem.WaitOne();
        }
    }
}
