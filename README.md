[![CI](https://github.com/DrimonMaster/Triggr/actions/workflows/ci.yml/badge.svg)](https://github.com/DrimonMaster/Triggr/actions/workflows/ci.yml)

# Triggr

Typed event bus for .NET. Zero dependencies. Subscribe, publish, unsubscribe — that's it.

## Installation

> NuGet coming soon.

## Usage

### Subscribe and publish

```csharp
var bus = new EventBus();

bus.Subscribe<OrderPlaced>(e => Console.WriteLine($"Order #{e.Id} placed"));

bus.Publish(new OrderPlaced(42));
```

### Condition filter and unsubscribe token

```csharp
var token = bus.Subscribe<OrderPlaced>(
    handler: e => Console.WriteLine($"Big order: {e.Id}"),
    condition: e => e.Total > 1000
);

// unsubscribe
token.Dispose();
```

### Subscribe once

```csharp
bus.SubscribeOnce<UserCreated>(e => Console.WriteLine($"Welcome, {e.Name}!"));
// handler fires on first matching publish, then unsubscribes automatically
```

### Priority

Higher priority handlers fire first. Equal priority preserves subscription order.

```csharp
bus.Subscribe<OrderPlaced>(_ => Console.WriteLine("low"),  priority: 0);
bus.Subscribe<OrderPlaced>(_ => Console.WriteLine("high"), priority: 10);

bus.Publish(new OrderPlaced(1));
// high
// low
```

### Middleware

```csharp
bus.Use(new LoggingMiddleware()); // built-in

// or custom:
bus.Use(new LambdaMiddleware((e, next) =>
{
    Console.WriteLine($"Before: {e.GetType().Name}");
    next(e);
    Console.WriteLine("After");
}));
```

Middleware runs before handlers on every `Publish`. Call `next` to continue the pipeline, skip it to block the event.

## Links

- Repository: [github.com/DrimonMaster/Triggr](https://github.com/DrimonMaster/Triggr)
