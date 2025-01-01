using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator
{
    /// <summary>
    /// Defines a mediator to handle <see cref="ICommand"/>, <see cref="IEvent"/> and <see cref="IRequest{TResponse}"/>.
    /// </summary>
    /// <inheritdoc cref="ICommand" path="/example"/>
    /// <inheritdoc cref="IEvent" path="/example"/>
    /// <inheritdoc cref="IRequest{TResponse}" path="/example"/>
    /// <example><![CDATA[
    /// ### Unmarked handling
    /// You might avoid using markers interfaces and endup still use the mediator. 
    /// ```csharp
    /// public class MyResponse
    /// {
    ///     // Add the necessary properties that will 
    ///     // be filled by your handler
    ///     ...
    /// }
    /// 
    /// // A sample request class without the IRequest<MyResponse> marker
    /// public class MyRequest
    /// {
    ///     // Add the necessary properties that will 
    ///     // be used by your handler
    ///     ...
    /// }
    /// ```
    /// Define a handler class inheriting from `RequestHandler<MyRequest, MyResponse>` and overriding the `HandleAsync` method.
    /// ```csharp
    /// public class MyRequestHandler : RequestHandler<MyRequest, MyResponse>
    /// {
    ///     public override async Task<MyResponse> HandleAsync(MyRequest request, CancellationToken cancellationToken)
    ///     {
    ///         // Do something with the request and return the response
    ///         await ...
    ///         
    ///         return new MyResponse
    ///         {
    ///             ...
    ///         }
    ///     }
    /// }
    /// ```
    /// Register your handler in the DI.
    /// ```csharp
    /// // Create the service collection and add the mediator to it
    /// var services = new ServiceCollection();
    /// 
    /// services.AddMediator()
    ///     // For your request, registered your handler
    ///     .Request<MyRequest, MyResponse>()
    ///         .HandleWith<MyRequestHandler>();
    /// 
    /// // Build the service provider after registering your handler
    /// using var serviceProvider = services.BuildServiceProvider();
    /// ```
    /// Use the `IMediator` to execute your request.
    /// ```csharp
    /// // Initialize the request
    /// var request = new MyRequest
    /// {
    ///     ...
    /// };
    /// 
    /// // Retrieve the mediator from the service provider and execute your request
    /// // you have to specify <MyResponse> since the request is not makred with IRequest<MyResponse>
    /// var mediator = serviceProvider.GetRequiredService<IMediator>();
    /// var response = await mediator.HandleAsync<MyResponse>(request);
    /// 
    /// // Handle the response provided
    /// ...
    /// ```
    /// You can alternatively capture the invocation context
    /// ```csharp
    /// // you have to specify <MyResponse> since the request is not makred with IRequest<MyResponse>
    /// var context = await mediator.HandleAndCaptureAsync<MyResponse>(request);
    /// if (context.HasError)
    /// {
    ///     // do something with the exception
    ///     var exception = context.Error;
    ///     ...
    /// }
    /// else
    /// {
    ///     // Do something on success
    ///     ...
    /// }
    /// ```
    /// ]]></example>
    public interface IMediator
    {
        /// <summary>
        /// Handles the specified <paramref name="request"/> (as a <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>)
        /// and returns the expected <typeparamref name="TResponse"/>.
        /// </summary>
        /// <typeparam name="TResponse">The expected response from <paramref name="request"/>.</typeparam>
        /// <param name="request">The <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/> to handle.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>The expected <typeparamref name="TResponse"/> value.</returns>
        Task<TResponse> HandleAsync<TResponse>(object request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the specified <paramref name="request"/> (as a <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>)
        /// and returns the <see cref="IInvocationContext{TResponse}"/> containing the expected 
        /// <typeparamref name="TResponse"/> or any <see cref="Exception"/> that might have occured.
        /// </summary>
        /// <typeparam name="TResponse">The expected response from <paramref name="request"/>.</typeparam>
        /// <param name="request">The <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/> to handle.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>
        /// The <see cref="IInvocationContext{TResponse}"/> containing the expected 
        /// <typeparamref name="TResponse"/> or any <see cref="Exception"/> that might have occured.
        /// </returns>
        Task<IInvocationContext<TResponse>> HandleAndCaptureAsync<TResponse>(object request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the specified <paramref name="request"/> and returns the expected <typeparamref name="TResponse"/>.
        /// </summary>
        /// <typeparam name="TResponse">The expected response from <paramref name="request"/>.</typeparam>
        /// <param name="request">The <see cref="IRequest{TResponse}"/> to execute.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>The expected <typeparamref name="TResponse"/> value.</returns>
        Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the specified <paramref name="request"/> and returns the <see cref="IInvocationContext{TResponse}"/> containing
        /// the expected <typeparamref name="TResponse"/> or any <see cref="Exception"/> that might have occured.
        /// </summary>
        /// <typeparam name="TResponse">The expected response from <paramref name="request"/>.</typeparam>
        /// <param name="request">The <see cref="IRequest{TResponse}"/> to execute.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>
        /// The <see cref="IInvocationContext{TResponse}"/> containing the expected 
        /// <typeparamref name="TResponse"/> or any <see cref="Exception"/> that might have occured.
        /// </returns>
        Task<IInvocationContext<TResponse>> ExecuteAndCaptureAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the specified <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>The executing process.</returns>
        Task InvokeAsync(ICommand command, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the specified <paramref name="command"/> and returns the 
        /// <see cref="IInvocationContext"/> containing any <see cref="Exception"/> that might have occured.
        /// </summary>
        /// <param name="command">The <see cref="ICommand"/> to execute.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>
        /// The <see cref="IInvocationContext"/> containing any <see cref="Exception"/> that might have occured.
        /// </returns>
        Task<IInvocationContext> InvokeAndCaptureAsync(ICommand command, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes the specified <paramref name="event"/>.
        /// </summary>
        /// <param name="event">The <see cref="IEvent"/> to publish.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the publication and related handlings.</param>
        /// <returns>The publication and handling process.</returns>
        Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes the specified <paramref name="event"/> and returns the <see cref="IInvocationContext"/> 
        /// containing any <see cref="Exception"/> that might have occured.
        /// </summary>
        /// <param name="event">The <see cref="IEvent"/> to publish.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the publication and related handlings.</param>
        /// <returns>
        /// The <see cref="IInvocationContext"/> containing any <see cref="Exception"/> that might have occured.
        /// </returns>
        Task<IInvocationContext> PublishAndCaptureAsync(IEvent @event, CancellationToken cancellationToken = default);
    }
}
