namespace TriggerProject;

public interface IEventBus
{
    IDisposable Subscribe<TEvent>(Action<TEvent> handler, Func<TEvent, bool>? condition = null);
    void Publish<TEvent>(TEvent e);
}
