namespace Crucible.Mediator.Requests
{
    /// <summary>
    /// <para>
    /// A <see cref="IRequest{TResponse}"/> defines a contract for requests in your application. 
    /// </para>
    /// <para>
    /// Requests typically represent queries or operations that require a response or result, such as retrieving data 
    /// or performing calculations. They are used when you need to request information or perform an operation and 
    /// expect a response in return. 
    /// </para>
    /// <para>
    /// When executed via the <c>IMediator</c>, the appropriate <c>IRequestHandler</c> is determined 
    /// and the corresponding logic is executed to produce the expected <typeparamref name="TResponse"/>.
    /// This process promotes clean, decoupled code by separating the request's definition from the handling logic,
    /// leading to a more maintainable and flexible design.
    /// </para>
    /// <para>
    /// Using the mediator pattern with requests helps ensure that the request-handling logic is centralized, 
    /// and you can easily swap or extend handlers without affecting the core logic that uses them. It also facilitates 
    /// handling cross-cutting concerns like logging, validation, or transaction management in a clean, centralized manner.
    /// </para>
    /// </summary>
    /// <typeparam name="TResponse">The response type expected from the <c>IMediator</c>.</typeparam>
    public interface IRequest<TResponse>
    {
        // Marker interface
    }
}
