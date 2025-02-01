> [!TIP]
> For an improved developer experience and to promote consistent design 
> patterns, it is highly recommended to implement this marker interface 
> wherever applicable. Doing so enforces clearer semantics when interacting 
> with `IMediator`, ensuring that the intended patterns of request, command, 
> and event handling are adhered to.
>
> Without implementing the marker interface, the conceptual separation 
> between handlers and orchestrators (or similar patterns) may become blurred, 
> potentially leading to unintended usage patterns, reduced maintainability, 
> or deviations from best practices.