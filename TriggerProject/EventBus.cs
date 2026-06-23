namespace TriggerProject;

public sealed class EventBus : IEventBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public void Subscribe<TEvent>(Action<TEvent> handler)
    {
        var type = typeof(TEvent);
        if (!_handlers.TryGetValue(type, out var list))
        {
            list = new List<Delegate>();
            _handlers[type] = list;
        }
        list.Add(handler);
    }

    public void Publish<TEvent>(TEvent e)
    {
        var type = typeof(TEvent);
        if (!_handlers.TryGetValue(type, out var list))
            return;

        foreach (var handler in list)
            ((Action<TEvent>)handler)(e);
    }
}
