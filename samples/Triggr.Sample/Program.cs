using Triggr;

var bus = new EventBus();
bus.Use(new LoggingMiddleware());

bus.SubscribeOnce<PlayerLevelUp>(
    _ => Console.WriteLine("  Tutorial: Welcome! You reached level 1.\n"),
    condition: e => e.Level == 1);

bus.SubscribeOnce<PlayerLevelUp>(
    _ => Console.WriteLine("  Tutorial: Great! Now you can place buildings.\n"),
    condition: e => e.Level == 2);

bus.SubscribeOnce<PlayerLevelUp>(
    _ =>
    {
        Console.WriteLine("  Tutorial: Building placement unlocked!");
        bus.Publish(new BuildingPlaced("Barracks"));
        Console.WriteLine();
    },
    condition: e => e.Level == 3);

bus.Subscribe<BuildingPlaced>(
    e => Console.WriteLine($"  Building placed: {e.Type}\n"),
    condition: e => e.Type == "Barracks");

Console.WriteLine("=== Simulation start ===\n");
bus.Publish(new PlayerLevelUp(1));
bus.Publish(new PlayerLevelUp(2));
bus.Publish(new PlayerLevelUp(3));
bus.Publish(new PlayerLevelUp(4));
Console.WriteLine("=== Simulation end ===");

record PlayerLevelUp(int Level);
record BuildingPlaced(string Type);
