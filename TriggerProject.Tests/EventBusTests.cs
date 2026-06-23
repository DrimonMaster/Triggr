using Xunit;
using TriggerProject;

namespace TriggerProject.Tests;

public class EventBusTests
{
    private record UserCreated(string Name);
    private record OrderPlaced(int Id);

    [Fact]
    public void Subscriber_receives_published_event()
    {
        var bus = new EventBus();
        UserCreated? received = null;

        bus.Subscribe<UserCreated>(e => received = e);
        bus.Publish(new UserCreated("Alice"));

        Assert.Equal("Alice", received?.Name);
    }

    [Fact]
    public void Two_subscribers_both_receive_event()
    {
        var bus = new EventBus();
        var calls = 0;

        bus.Subscribe<UserCreated>(_ => calls++);
        bus.Subscribe<UserCreated>(_ => calls++);
        bus.Publish(new UserCreated("Bob"));

        Assert.Equal(2, calls);
    }

    [Fact]
    public void Publish_with_no_subscribers_does_not_throw()
    {
        var bus = new EventBus();
        bus.Publish(new UserCreated("Eve"));
    }

    [Fact]
    public void Subscriber_does_not_receive_different_event_type()
    {
        var bus = new EventBus();
        var called = false;

        bus.Subscribe<OrderPlaced>(_ => called = true);
        bus.Publish(new UserCreated("Alice"));

        Assert.False(called);
    }

    [Fact]
    public void Disposed_token_removes_subscription()
    {
        var bus = new EventBus();
        var calls = 0;

        var token = bus.Subscribe<UserCreated>(_ => calls++);
        token.Dispose();
        bus.Publish(new UserCreated("Alice"));

        Assert.Equal(0, calls);
    }

    [Fact]
    public void Condition_true_handler_is_called()
    {
        var bus = new EventBus();
        var called = false;

        bus.Subscribe<UserCreated>(_ => called = true, condition: e => e.Name == "Alice");
        bus.Publish(new UserCreated("Alice"));

        Assert.True(called);
    }

    [Fact]
    public void Condition_false_handler_is_not_called()
    {
        var bus = new EventBus();
        var called = false;

        bus.Subscribe<UserCreated>(_ => called = true, condition: e => e.Name == "Alice");
        bus.Publish(new UserCreated("Bob"));

        Assert.False(called);
    }
}
