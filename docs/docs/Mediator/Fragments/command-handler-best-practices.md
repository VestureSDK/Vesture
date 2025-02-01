**[!INCLUDE [do](../../Fragments/do-inline-header.md)] suffix your handler with `...Handler`** \
To enhance the developers understand your handler is indeed a handler, ensure to suffix them with `...Handler`.

```csharp
// do not
class ChangeToDoItemStatusService : CommandHandler<ChangeToDoItemStatusCommand> { /* omitted */ }

// instead, suffix the name with ...Handler
class ChangeToDoItemStatusHandler : CommandHandler<ChangeToDoItemStatusCommand> { /* omitted */ }
```

***

**[!INCLUDE [avoid](../../Fragments/avoid-inline-header.md)] using `IMediator` within a handler** \
To avoid loops and hard to debug scnerios, do not use a `IMediator` within a handler resulting in
nested calls to other handlers. For that kind of scenario, kindly refer to the `Saga` pattern instead.

> [!Warning]
> While the Saga pattern is still a work in progress, the principle should still stand
> and be followed.

```csharp
// do not
class ChangeToDoItemStatusHandler : CommandHandler<ChangeToDoItemStatusCommand> 
{ 
	private readonly IMediator _mediator;

	public ChangeToDoItemStatusHandler(IMediator mediator)
	{
		_mediator = mediator;
	}
	
	public override async Task HandleAsync(ChangeToDoItemStatusCommand command, CancellationToken cancellationToken)
	{
		using var scope = new TransactionScope();

		// sub command call
		var updateItemStatusCommand = new UpdateItemStatusCommand() { /* omitted */ };
		await _mediator.InvokeAsync(updateItemStatusCommand);
		
		// sub command call
		var sendEmailCommand = new SendEmailCommand() { /* omitted */ };
		await _mediator.InvokeAsync(sendEmailCommand);
		
		scope.Complete();
	}
}

// instead, use a saga
class ChangeToDoItemStatusSaga : Saga<ChangeToDoItemStatusCommand> 
{
	public override async Task HandleAsync(ChangeToDoItemStatusCommand command, CancellationToken cancellationToken)
	{
		// sub command call
		var updateItemStatusCommand = new UpdateItemStatusCommand() { /* omitted */ };
		await InvokeAsync(updateItemStatusCommand);
		
		// sub command call
		var sendEmailCommand = new SendEmailCommand() { /* omitted */ };
		await InvokeAsync(sendEmailCommand);
	}
}
```