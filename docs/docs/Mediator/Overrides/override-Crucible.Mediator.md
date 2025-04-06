---
uid: Vesture.Mediator
summary: *content
---

The namespace <xref:Vesture.Mediator> includes classes and interfaces
to use the mediator pattern via <xref:Vesture.Mediator.IMediator>.

A mediator acts as a central hub that coordinates the communication between 
different components, such as requests, commands, and events, and their 
respective handlers. It decouples the sender from the receiver, 
enabling a more modular and maintainable design. 

The <xref:Vesture.Mediator.IMediator> manages the entire request lifecycle, ensuring that each request is 
dispatched to the appropriate handler, optionally passing through middleware 
for additional processing. This promotes loose coupling, single 
responsibility, and clear separation of concerns within the application 
architecture.