**[!INCLUDE [do](../../Fragments/do-inline-header.md)] mark your commands with `ICommand`** \
To enhance the developer experience, ensure your commands implement the `ICommand` marker interface.

```csharp
// do not
class ChangeToDoItemStatusCommand { /* omitted */ }
await mediator.HandleAsync<CommandResponse>(new ChangeToDoItemStatusCommand());

// instead, implement ICommand
class ChangeToDoItemStatusCommand : ICommand { /* omitted */ }
await mediator.InvokeAsync(new ChangeToDoItemStatusCommand());
```

***

**[!INCLUDE [do](../../Fragments/do-inline-header.md)] suffix your commands with `...Command`** \
To enhance the developers understand your command is indeed a command, ensure to suffix them with `...Command`.

```csharp
// do not
class ChangeToDoItemStatus : ICommand { /* omitted */ }
await mediator.InvokeAsync(new ChangeToDoItemStatus());

// instead, suffix the name with ...Command
class ChangeToDoItemStatusCommand : ICommand { /* omitted */ }
await mediator.InvokeAsync(new ChangeToDoItemStatusCommand());
```

***

**[!INCLUDE [do](../../Fragments/do-inline-header.md)] make your commands serializable** \
Ensure your command class is serializable so you can easily re-use it via with a distributed `IMediator`.

```csharp
// do not
class ChangeToDoItemStatusCommand : ICommand
{
	public Func<string> IdProvider { get; set; }
}

// instead, ensure command is serializable
class ChangeToDoItemStatusCommand : ICommand 
{
	public string Id { get; set; }
}
```

***

**[!INCLUDE [do](../../Fragments/dont-inline-header.md)]  use `ICommand` for events** \
Ensure to be familiar with the semantics and use cases of `ICommand` and `IEvent` to avoid implementing the wrong marker.

```csharp
// do not
class OnToDoItemStatusChangeCommand : ICommand { /* omitted */ }
await mediator.InvokeAsync(new OnToDoItemStatusChangeCommand());

// instead, use events
class OnToDoItemStatusChangeEvent : IEvent { /* omitted */ }
await mediator.PublishAsync(new OnToDoItemStatusChangeEvent());
```