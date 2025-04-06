---
uid: Vesture.Mediator.Invocation
summary: *content
---

[!INCLUDE [example](../../Fragments/advanced-usage-label.md)]

---

The namespace <xref:Vesture.Mediator.Invocation> includes classes and interfaces
to extend the mediator pipeline via <xref:Vesture.Mediator.Invocation.InvocationMiddleware`2>.

Middlewares process requests as they pass through 
the mediator pipeline. They enable cross-cutting concerns, such as logging, 
validation, caching, or performance monitoring, to be handled transparently. 
Middleware can modify, transform, or short-circuit requests and responses, 
providing a flexible mechanism to intercept and extend the behavior of the 
mediation process without altering individual handlers or requests directly.
