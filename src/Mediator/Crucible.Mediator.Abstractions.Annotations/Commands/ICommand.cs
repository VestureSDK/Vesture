namespace Crucible.Mediator.Commands
{
    /// <summary>
    /// Defines a command not returning a reponse when invoked via a mediator.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>While not necessary to implement it, this marker interface will enhance the developer experience.</item>
    /// <item>It is strongly suggested the command is also serializable for distributed application scenario.</item>
    /// </list>
    /// </remarks>
    /// <example><![CDATA[
    /// ### `ICommand` invocation
    /// Define a class with the `ICommand` interface marker. 
    /// ```csharp
    /// public class MyCommand : ICommand
    /// {
    ///     // Add the necessary properties that will 
    ///     // be used by your handler
    ///     ...
    /// }
    /// ```
    /// Define a handler class inheriting from `CommandHandler<MyCommand>` and overriding the `HandleAsync` method.
    /// ```csharp
    /// public class MyCommandHandler : CommandHandler<MyCommand>
    /// {
    ///     public override async Task HandleAsync(MyCommand command, CancellationToken cancellationToken)
    ///     {
    ///         // Do something with the command
    ///         await ...
    ///     }
    /// }
    /// ```
    /// Register your handler in the DI.
    /// ```csharp
    /// // Create the service collection and add the mediator to it
    /// var services = new ServiceCollection();
    /// 
    /// services.AddMediator()
    ///     // For your command, registered your handler
    ///     .Command<MyCommand>()
    ///         .HandleWith<MyCommandHandler>();
    /// 
    /// // Build the service provider after registering your handler
    /// using var serviceProvider = services.BuildServiceProvider();
    /// ```
    /// Use the `IMediator` to invoke your command.
    /// ```csharp
    /// // Initialize your command
    /// var command = new MyCommand
    /// {
    ///     ...
    /// };
    /// 
    /// // Retrieve the mediator from the service provider and invoke your command
    /// var mediator = serviceProvider.GetRequiredService<IMediator>();
    /// await mediator.InvokeAsync(command);
    /// ```
    /// You can alternatively capture the invocation context
    /// ```csharp
    /// var context = await mediator.InvokeAndCaptureAsync(command);
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
    public interface ICommand
    {
        // Marker interface
    }
}
