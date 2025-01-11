using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Invocation
{
    /// <summary>
    /// <para>
    /// An <see cref="IInvocationWorkflow"/> serves as a higher-level orchestrator within the mediator pattern, designed to manage 
    /// and coordinate the flow of multiple operations across different handlers.
    /// </para>
    /// <para>
    /// Unlike a handler, which is typically responsible for handling a single request or command, 
    /// a workflow orchestrates a sequence of operations, potentially involving multiple handlers or contracts, 
    /// ensuring that they are executed in the correct order.
    /// </para>
    /// <para>
    /// Workflows encapsulate the logic for managing interactions and dependencies between operations, including:
    /// </para>
    /// <list type="bullet">
    ///     <item><description>Coordinating the execution of multiple handlers in a sequence.</description></item>
    ///     <item><description>Managing complex workflows with conditional logic, retries, or side-effects.</description></item>
    ///     <item><description>Ensuring consistency by managing the outcomes of each handler in the sequence.</description></item>
    /// </list>
    /// <para>
    /// A workflow leverages the mediator pattern to delegate execution to other handlers or operations, 
    /// ensuring that each operation is completed before proceeding to the next. This enables the development 
    /// of complex workflows while keeping individual handlers simple and focused on their specific tasks.
    /// </para>
    /// <para>
    /// The workflow is responsible for orchestrating the flow of operations, using the mediator to invoke commands, 
    /// handle requests, and publish events in the right sequence, while also managing failure cases or retries 
    /// when needed.
    /// </para>
    /// </summary>
    /// <seealso cref="IRequest{TResponse}"/>
    /// <seealso cref="ICommand"/>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="IInvocationHandler{TRequest, TResponse}"/>
    /// <seealso cref="IMediator"/>
    public interface IInvocationWorkflow
    {
#pragma warning disable RS0016 // Add public types and members to the declared API
        /// <exclude />
        public IWorkflowMediator Mediator { set; }
#pragma warning restore RS0016 // Add public types and members to the declared API
    }
}
