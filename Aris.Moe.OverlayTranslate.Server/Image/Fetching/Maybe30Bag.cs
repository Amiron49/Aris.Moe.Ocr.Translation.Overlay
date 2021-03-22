using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching
{
    public class Maybe30Bag<T>
    {
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        public List<T> Contents => _queue.ToList();

        public void Add(T thing)
        {
            var tooMany = _queue.Count - 30;

            if (tooMany > 0)
            {
                for (var i = 0; i < tooMany; i++)
                    _queue.TryDequeue(out _);
            }

            _queue.Enqueue(thing);
        }
    }
}