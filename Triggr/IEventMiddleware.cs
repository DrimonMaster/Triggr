namespace Triggr;

public interface IEventMiddleware
{
    void Handle(object e, Action<object> next);
}
