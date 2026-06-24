namespace Triggr;

public sealed class EventBus : IEventBus
{
    private readonly Dictionary<Type, List<Entry>> _handlers = new();
    private readonly List<IEventMiddleware> _middleware = new();
    private int _order;

    public void Use(IEventMiddleware middleware) => _middleware.Add(middleware);

    public IDisposable Subscribe<TEvent>(Action<TEvent> handler, Func<TEvent, bool>? condition = null, int priority = 0)
    {
        Action<TEvent> wrapped = condition == null
            ? handler
            : e => { if (condition(e)) handler(e); };

        var type = typeof(TEvent);
        if (!_handlers.TryGetValue(type, out var list))
        {
            list = new List<Entry>();
            _handlers[type] = list;
        }

        var entry = new Entry(wrapped, priority, _order++);
        list.Add(entry);

        return new Token(() => list.Remove(entry));
    }

    public IDisposable SubscribeOnce<TEvent>(Action<TEvent> handler, Func<TEvent, bool>? condition = null, int priority = 0)
    {
        IDisposable? token = null;
        token = Subscribe<TEvent>(e =>
        {
            token!.Dispose();
            handler(e);
        }, condition, priority);
        return token;
    }

    public void Publish<TEvent>(TEvent e)
    {
        _handlers.TryGetValue(typeof(TEvent), out var list);

        var snapshot = list != null ? list.ToArray() : Array.Empty<Entry>();
        if (snapshot.Length > 1)
            Array.Sort(snapshot, (a, b) =>
            {
                int cmp = b.Priority.CompareTo(a.Priority);
                return cmp != 0 ? cmp : a.Order.CompareTo(b.Order);
            });

        Action<object> invokeHandlers = ev =>
        {
            foreach (var entry in snapshot)
                ((Action<TEvent>)entry.Handler)((TEvent)ev);
        };

        Action<object> pipeline = invokeHandlers;
        for (int i = _middleware.Count - 1; i >= 0; i--)
        {
            var mw = _middleware[i];
            var next = pipeline;
            pipeline = ev => mw.Handle(ev, next);
        }
        
        pipeline(e!);
    }

    private sealed record Entry(Delegate Handler, int Priority, int Order);

    private sealed class Token : IDisposable
    {
        private readonly Action _remove;
        private bool _disposed;

        public Token(Action remove) => _remove = remove;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _remove();
        }
    }
}
