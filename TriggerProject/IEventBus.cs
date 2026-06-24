namespace TriggerProject;

public interface IEventBus
{
    IDisposable Subscribe<TEvent>(Action<TEvent> handler, Func<TEvent, bool>? condition = null, int priority = 0);
    IDisposable SubscribeOnce<TEvent>(Action<TEvent> handler, Func<TEvent, bool>? condition = null, int priority = 0);
    void Publish<TEvent>(TEvent e);
}