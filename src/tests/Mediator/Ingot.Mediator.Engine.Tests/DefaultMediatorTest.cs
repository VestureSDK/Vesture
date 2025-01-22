using Ingot.Mediator.Abstractions.Tests.Data;
using Ingot.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Ingot.Mediator.Abstractions.Tests.Data.Annotations.Events;
using Ingot.Mediator.Abstractions.Tests.Data.Annotations.Requests;
using Ingot.Mediator.Commands;
using Ingot.Mediator.Engine.Mocks.Pipeline;
using Ingot.Mediator.Engine.Mocks.Pipeline.Internal;
using Ingot.Mediator.Engine.Pipeline.Internal;
using Ingot.Mediator.Engine.Tests.Pipeline.Internal.NoOp;
using Ingot.Mediator.Events;
using Ingot.Mediator.Invocation;
using Ingot.Mediator.Mocks.Invocation;
using Ingot.Mediator.Requests;
using Ingot.Testing;
using Ingot.Testing.Annotations;

namespace Ingot.Mediator.Engine.Tests
{
    [ImplementationTest]
    public class DefaultMediatorTest : EngineMediatorTestBase<DefaultMediator>
    {
        protected ICollection<IMiddlewareInvocationPipelineItem> MiddlewareItems { get; } = [];

        protected Dictionary<(Type RequestType, Type ResponseType), MockInvocationPipeline> Pipelines { get; } = [];

        protected NUnitTestContextMsLogger<DefaultMediator> Logger { get; } = new();

        protected MockNoOpInvocationPipelineResolver NoOpInvocationPipelineResolver { get; } = new();

        protected override DefaultMediator CreateMediator()
        {
            foreach (var pipeline in Pipelines.Values)
            {
                pipeline.Middlewares = MiddlewareItems;
            }

            return new(
                Logger,
                Pipelines.Values,
                NoOpInvocationPipelineResolver);
        }

        protected MockInvocationPipeline<TRequest, TResponse> GetOrCreatePipeline<TRequest, TResponse>()
        {
            var pipelineKey = (typeof(TRequest), typeof(TResponse));
            if (!(Pipelines.TryGetValue(pipelineKey, out var p) && p is MockInvocationPipeline<TRequest, TResponse> pipeline))
            {
                pipeline = new MockInvocationPipeline<TRequest, TResponse>();
                Pipelines[pipelineKey] = pipeline;
            }

            return pipeline;
        }

        protected override void RegisterHandler<TRequest, TResponse>(IInvocationHandler<TRequest, TResponse> handler)
        {
            var pipeline = GetOrCreatePipeline<TRequest, TResponse>();
            pipeline.Handlers.Add(handler);
        }

        protected override void RegisterMiddleware<TRequest, TResponse>(IInvocationMiddleware<TRequest, TResponse> middleware)
        {
            var item = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>()
            {
                Middleware = middleware
            };

            MiddlewareItems.Add(item);
        }

        protected override void RegisterMiddleware<TRequest, TResponse>(int order, IInvocationMiddleware<TRequest, TResponse> middleware)
        {
            var item = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>()
            {
                Order = order,
                Middleware = middleware
            };

            MiddlewareItems.Add(item);
        }

        [Test]
        public void Ctor_ArgumentNullException_IfLoggerIsNull()
        {
            // Arrange
            // No arrange required

            // Act / Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(() => new DefaultMediator(null, [], NoOpInvocationPipelineResolver));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Test]
        public void Ctor_ArgumentNullException_IfPipelinesIsNull()
        {
            // Arrange
            // No arrange required

            // Act / Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(() => new DefaultMediator(Logger, null, NoOpInvocationPipelineResolver));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Test]
        public void Ctor_ArgumentNullException_IfResolverIsNull()
        {
            // Arrange
            // No arrange required

            // Act / Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(() => new DefaultMediator(Logger, [], null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.

        [Test]
        [TestCaseSource_RequestResponse_All]
        public void HandleAndCaptureAsync_Throws_IfContractIsNull<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : class
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            request = null;

            // Act / Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => Mediator.HandleAndCaptureAsync<TResponse>(request, CancellationToken));
        }

        [Test]
        [TestCaseSource_RequestResponse_All]
        public void HandleAsync_Throws_IfContractIsNull<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : class
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            request = null;

            // Act / Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => Mediator.HandleAsync<TResponse>(request, CancellationToken));
        }

        [Test]
        [TestCaseSource_RequestResponse_Request]
        public void ExecuteAndCaptureAsync_Throws_IfContractIsNull<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : class, IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            request = null;

            // Act / Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => Mediator.ExecuteAndCaptureAsync(request, CancellationToken));
        }

        [Test]
        [TestCaseSource_RequestResponse_Request]
        public void ExecuteAsync_Throws_IfContractIsNull<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : class, IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            request = null;

            // Act / Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => Mediator.ExecuteAsync(request, CancellationToken));
        }

        [Test]
        [TestCaseSource_RequestResponse_Request]
        public void ExecuteAsync_Throws_IfNoPipelineRegistered<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : class, IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<MediatorTestData.Unrelated, MediatorTestData.Unrelated>();
            AddHandler(handler);

            // Act / Assert
            Assert.ThrowsAsync<KeyNotFoundException>(() => Mediator.ExecuteAsync(request, CancellationToken));
        }

        [Test]
        [TestCaseSource_RequestResponse_Command]
        public void InvokeAndCaptureAsync_Throws_IfContractIsNull<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : class, ICommand
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            request = null;

            // Act / Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => Mediator.InvokeAndCaptureAsync(request, CancellationToken));
        }

        [Test]
        [TestCaseSource_RequestResponse_Command]
        public void InvokeAsync_Throws_IfContractIsNull<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : class, ICommand
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            request = null;

            // Act / Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => Mediator.InvokeAsync(request, CancellationToken));
        }

        [Test]
        [TestCaseSource_RequestResponse_Command]
        public void InvokeAsync_Throws_IfNoPipelineRegistered<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : class, ICommand
        {
            // Arrange
            var handler = new MockInvocationHandler<MediatorTestData.Unrelated, MediatorTestData.Unrelated>();
            AddHandler(handler);

            // Act / Assert
            Assert.ThrowsAsync<KeyNotFoundException>(() => Mediator.InvokeAsync(request, CancellationToken));
        }

        [Test]
        [TestCaseSource_RequestResponse_Event]
        public void PublishAndCaptureAsync_Throws_IfContractIsNull<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : class, IEvent
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            request = null;

            // Act / Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => Mediator.PublishAndCaptureAsync(request, CancellationToken));
        }

        [Test]
        [TestCaseSource_RequestResponse_Event]
        public void PublishAsync_Throws_IfContractIsNull<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : class, IEvent
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            request = null;

            // Act / Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => Mediator.PublishAsync(request, CancellationToken));
        }
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
    }
}
