namespace TriggerProject;

public sealed class EventBus : IEventBus
{
    private readonly Dictionary<Type, List<Entry>> _handlers = new();
    private int _order;

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
        var type = typeof(TEvent);
        if (!_handlers.TryGetValue(type, out var list))
            return;

        var snapshot = list.ToArray();
        Array.Sort(snapshot, (a, b) =>
        {
            int cmp = b.Priority.CompareTo(a.Priority);
            return cmp != 0 ? cmp : a.Order.CompareTo(b.Order);
        });

        foreach (var entry in snapshot)
            ((Action<TEvent>)entry.Handler)(e);
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
