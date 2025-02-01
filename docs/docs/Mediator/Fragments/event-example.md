Define a class with the `IEvent` interface marker. 
```csharp
public class MyEvent : IEvent
{
    // Add the necessary properties that will 
    // be used by your handler
    ...
}
```
Define a handler class inheriting from `EventHandler<MyEvent>` and overriding the `HandleAsync` method.
```csharp
public class MyEventHandler : EventHandler<MyEvent>
{
    public override async Task HandleAsync(MyEvent @event, CancellationToken cancellationToken)
    {
        // Do something with the event
        await ...
    }
}
```
Register your handler in the DI.
```csharp
// Create the service collection and add the mediator to it
var services = new ServiceCollection();

services.AddMediator()
    // For your event, registered your handler
    .Event<MyEvent>()
        .HandleWith<MyEventHandler>();

// Build the service provider after registering your handler
using var serviceProvider = services.BuildServiceProvider();
```
Use the `IMediator` to publish your event.
```csharp
// Initialize the event
var @event = new MyEvent
{
    ...
};

// Retrieve the mediator from the service provider and publish your event
var mediator = serviceProvider.GetRequiredService<IMediator>();
await mediator.PublishAsync(@event);
```
You can alternatively capture the invocation context
```csharp
var context = await mediator.PublishAndCaptureAsync(@event);
if (context.HasError)
{
    // do something with the exception
    var exception = context.Error;
    ...
}
else
{
    // Do something on success
    ...
}
```