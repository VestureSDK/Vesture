---
uid: Crucible.Mediator.Commands
summary: *content
---

The namespace <xref:Crucible.Mediator.Commands> includes classes and interfaces
to handle the mediator event use case with <xref:Crucible.Mediator.Commands.ICommand> 
as contract and <xref:Crucible.Mediator.Commands.CommandHandler`1> as handler.

Commands represent actions or operations that trigger 
state changes in the system that do not require a response 
or result, such as creating, updating, or deleting data.

---
uid: Crucible.Mediator.Commands
---

***

```mermaid
sequenceDiagram

    participant App as Your App

    participant Mediator as Mediator
    participant Middlewares as Middlewares
    participant Handler as CommandHandler

    Note over App: Create<br/>MyCommand: ICommand
    App-)Mediator: MyCommand
    activate App
    activate Mediator
    Note over App: (await) Task/Void
    Mediator-)Handler: InvocationContext<MyCommand, CommandResponse>
    activate Handler
    activate Middlewares
    Note over Mediator,Handler: Invocation Pipeline
    Handler-->Mediator: 
    deactivate Middlewares
    deactivate Handler
    Mediator--xApp: 
    deactivate Mediator
    deactivate App
```

### Example

[!INCLUDE [example](../Fragments/command-example.md)]
