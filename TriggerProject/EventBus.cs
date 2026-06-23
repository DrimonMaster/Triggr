namespace TriggerProject;

public sealed class EventBus : IEventBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public IDisposable Subscribe<TEvent>(Action<TEvent> handler, Func<TEvent, bool>? condition = null)
    {
        Action<TEvent> wrapped = condition == null
            ? handler
            : e => { if (condition(e)) handler(e); };

        var type = typeof(TEvent);
        if (!_handlers.TryGetValue(type, out var list))
        {
            list = new List<Delegate>();
            _handlers[type] = list;
        }
        list.Add(wrapped);

        return new Token(() => list.Remove(wrapped));
    }

    public IDisposable SubscribeOnce<TEvent>(Action<TEvent> handler, Func<TEvent, bool>? condition = null)
    {
        IDisposable? token = null;
        token = Subscribe<TEvent>(e =>
        {
            token!.Dispose();
            handler(e);
        }, condition);
        return token;
    }

    public void Publish<TEvent>(TEvent e)
    {
        var type = typeof(TEvent);
        if (!_handlers.TryGetValue(type, out var list))
            return;

        foreach (var handler in list.ToArray())
            ((Action<TEvent>)handler)(e);
    }

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
