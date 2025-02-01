**[!INCLUDE [do](../../Fragments/do-inline-header.md)] suffix your handler with `...Handler`** \
To enhance the developers understand your handler is indeed a handler, ensure to suffix them with `...Handler`.

```csharp
// do not
class ChangeToDoItemService : CommandHandler<ChangeToDoItemStatusCommand> { /* omitted */ }

// instead, suffix the name with ...Handler
class ChangeToDoItemHandler : CommandHandler<ChangeToDoItemStatusCommand> { /* omitted */ }
```