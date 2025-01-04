---
uid: Crucible.Mediator.Commands.ICommand
---

[!INCLUDE [example](../Fragments/marker-devx-tip.md)]

---
uid: Crucible.Mediator.Commands.ICommand
example: [*content]
---

```csharp
// Implement the interface ICommand to enhance the developer experience
public class SampleCommand : ICommand
{
	// Add the properties that will be used by your handler
	...
}
```

---
uid: Crucible.Mediator.Commands.ICommand
example: []
remarks: *content
---

> [!NOTE]
> The `ICommand` is part of the commands use case of the `IMediator`.
To be familiar with its overall usage, kindly refer to 
the `Commands` documentation.

***

[!INCLUDE [best-practices](../Fragments/command-best-practices.md)]