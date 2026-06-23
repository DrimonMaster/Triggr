namespace TriggerProject;

public interface IEventBus
{
    void Subscribe<TEvent>(Action<TEvent> handler);
    void Publish<TEvent>(TEvent e);
}
