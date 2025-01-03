Define a response class and a request class with the `IRequest<MyResponse>` interface marker. 
```csharp
public class MyResponse
{
    // Add the necessary properties that will 
    // be filled by your handler
    ...
}

public class MyRequest : IRequest<MyResponse>
{
    // Add the necessary properties that will 
    // be used by your handler
    ...
}
```
Define a handler class inheriting from `RequestHandler<MyRequest, MyResponse>` and overriding the `HandleAsync` method.
```csharp
public class MyRequestHandler : RequestHandler<MyRequest, MyResponse>
{
    public override async Task<MyResponse> HandleAsync(MyRequest request, CancellationToken cancellationToken)
    {
        // Do something with the request and return the response
        await ...
        
        return new MyResponse
        {
            ...
        }
    }
}
```
Register your handler in the DI.
```csharp
// Create the service collection and add the mediator to it
var services = new ServiceCollection();

services.AddMediator()
    // For your request, registered your handler
    .Request<MyRequest, MyResponse>()
        .HandleWith<MyRequestHandler>();

// Build the service provider after registering your handler
using var serviceProvider = services.BuildServiceProvider();
```
Use the `IMediator` to execute your request.
```csharp
// Initialize the request
var request = new MyRequest
{
    ...
};

// Retrieve the mediator from the service provider and execute your request
var mediator = serviceProvider.GetRequiredService<IMediator>();
var response = await mediator.ExecuteAsync(request);

// Handle the response provided
...
```
You can alternatively capture the invocation context
```csharp
var context = await mediator.ExecuteAndCaptureAsync(request);
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