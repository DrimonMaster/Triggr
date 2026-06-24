namespace Triggr;

public sealed class LoggingMiddleware : IEventMiddleware
{
    public void Handle(object e, Action<object> next)
    {
        Console.WriteLine($"[Event] {e.GetType().Name}: {e}");
        next(e);
    }
}
