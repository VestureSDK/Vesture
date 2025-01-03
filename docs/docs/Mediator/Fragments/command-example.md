This example will guide you through on how to use `ICommand` and
the `IMediator` to handle commands within your application.

> [!NOTE]
> If you have not passed through the Setup and get started section,
> it is strongly recommended to read these before continuing.

First, define a class with the `ICommand` interface marker. 

```csharp
public class MyCommand : ICommand
{
    // Add the necessary properties that will 
    // be used by your handler
    ...
}
```

Then, define a handler class inheriting from `CommandHandler<MyCommand>` and overriding the `HandleAsync` method.

```csharp
public class MyCommandHandler : CommandHandler<MyCommand>
{
    public override async Task HandleAsync(MyCommand command, CancellationToken cancellationToken)
    {
        // Do something with the command
        await ...
    }
}
```

Register your handler in the DI for the `MyCommand` aforedmentioned.

```csharp
// Create the service collection and add the mediator to it
var services = new ServiceCollection();

services.AddMediator()
    // For your command, registered your handler
    .Command<MyCommand>()
        .HandleWith<MyCommandHandler>();

// Build the service provider after registering your handler
using var serviceProvider = services.BuildServiceProvider();
```

Finally, use the `IMediator` to invoke your command by using `InvokeAsync`.

```csharp
// Initialize your command
var command = new MyCommand
{
    ...
};

// Retrieve the mediator from the service provider and invoke your command
var mediator = serviceProvider.GetRequiredService<IMediator>();
await mediator.InvokeAsync(command);
```

Or alternatively, to deal with the `IInvocationContext` and 
act on it, use `InvokeAndCaptureAsync` instead. 

```csharp
var context = await mediator.InvokeAndCaptureAsync(command);
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
