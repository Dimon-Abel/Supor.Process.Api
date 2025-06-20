﻿using System.Collections.Concurrent;

namespace Supor.Process.Common.Common
{
    public class Queue<T>
    {
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
        }

        public bool TryDequeue(out T item)
        {
            return _queue.TryDequeue(out item);
        }
    }
}
