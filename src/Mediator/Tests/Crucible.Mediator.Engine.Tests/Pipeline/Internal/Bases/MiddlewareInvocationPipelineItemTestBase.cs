using Crucible.Mediator.Abstractions.Tests.Commands.Mocks;
using Crucible.Mediator.Abstractions.Tests.Events.Mocks;
using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Abstractions.Tests.Requests.Mocks;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Tests.Pipeline.Resolvers.Mocks;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Moq;
using Any = object;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Internal.Bases
{
    public abstract class MiddlewareInvocationPipelineItemConformanceTestBase<TRequest, TResponse, TMiddlewareItem>
        where TMiddlewareItem : IMiddlewareInvocationPipelineItem<TRequest, TResponse>
    {
        protected int Order { get; set; }

        protected Lazy<TMiddlewareItem> MiddlewareItemInitializer { get; }

        protected TMiddlewareItem MiddlewareItem => MiddlewareItemInitializer.Value;

        protected MockInvocationContext<TRequest, TResponse> Context { get; }

        protected MockInvocationMiddleware<TRequest, TResponse> Middleware { get; } = new();

        protected MockNext Next { get; }

        protected CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        public MiddlewareInvocationPipelineItemConformanceTestBase(TRequest defaultRequest)
        {
            Next = new();

            Context = new() { Request = defaultRequest! };

#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
            MiddlewareItemInitializer = new Lazy<TMiddlewareItem>(() => CreateMiddlewareItem(Order));
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        }

        protected abstract TMiddlewareItem CreateMiddlewareItem(int order);

        [Theory]
        [ConformanceTest]
        [TestCase(Int32.MinValue, Description = "Order set to a min")]
        [TestCase(-123, Description = "Order set to a negative number")]
        [TestCase(0, Description = "Order set to 0")]
        [TestCase(123, Description = "Order set to a positive number")]
        [TestCase(Int32.MaxValue, Description = "Order set to a max")]
        public void Order_IsCtorOrder(int expectedOrder)
        {
            // Arrange
            Order = expectedOrder;

            // Act
            var actualOrder = MiddlewareItem.Order;

            // Assert
            Assert.That(actualOrder, Is.EqualTo(expectedOrder), message: "Actual order should be equal to provided ctor order");
        }

        [Theory]
        [ConformanceTest]
        // IRequest<TResponse>
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Any, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ MockRequest, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Any, MockResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ MockRequest, MockResponse>()]
        // ICommand
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ Any, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ MockCommand, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ Any, CommandResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ Any, NoResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ MockCommand, CommandResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ MockCommand, NoResponse>()]
        // IEvent
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ Any, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ MockEvent, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ Any, EventResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ Any, NoResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ MockEvent, EventResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ MockEvent, NoResponse>()]
        // Unmarked (IRequest<TResponse>)
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ Any, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ MockUnmarked, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ Any, MockUnmarked>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ MockUnmarked, MockUnmarked>()]
        // Unmarked (ICommand)
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ Any, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ MockUnmarked, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ Any, CommandResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ Any, NoResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ MockUnmarked, CommandResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ MockUnmarked, NoResponse>()]
        // Unmarked (IEvent)
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ Any, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ MockUnmarked, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ Any, EventResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ Any, NoResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ MockUnmarked, EventResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ MockUnmarked, NoResponse>()]
        // Unmarked (Special NoResponse)
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ Any, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ MockUnmarked, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ Any, NoResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ MockUnmarked, NoResponse>()]
        public void IsApplicable_IsTrue_WhenMiddlewareSignatureMacthesOrLowerLevelThanContracts
            <TContractRequest, TContractResponse, TMiddlewareRequest, TMiddlewareResponse>()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<TMiddlewareRequest, TMiddlewareResponse>();
            var contextType = typeof(IInvocationContext<TContractRequest, TContractResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: $"Middleware ({typeof(TMiddlewareRequest).Name} -> {typeof(TMiddlewareResponse).Name}) should be applicable for contract ({typeof(TContractRequest).Name} -> {typeof(TContractResponse).Name})");
        }

        [Theory]
        [ConformanceTest]
        // IRequest<TResponse>
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Any, Other>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Other, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ MockRequest, Other>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Other, MockResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Other, CommandResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Other, EventResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Other, NoResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Other, Other>()]
        // ICommand
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ Any, Other>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ Other, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ Other, Other>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ MockCommand, Other>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ Other, CommandResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ Other, EventResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ Other, NoResponse>()]
        // IEvent
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ Any, Other>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ Other, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ Other, Other>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ MockEvent, Other>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ Other, CommandResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ Other, EventResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ Other, NoResponse>()]
        // Unmarked (IRequest<TResponse>)
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ Any, Other>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ Other, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ MockUnmarked, Other>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ Other, MockUnmarked>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ Other, CommandResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ Other, EventResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ Other, NoResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ Other, Other>()]
        // Unmarked (ICommand)
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ Any, Other>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ Other, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ Other, Other>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ MockUnmarked, Other>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ Other, CommandResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ Other, EventResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ Other, NoResponse>()]
        // Unmarked (IEvent)
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ Any, Other>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ Other, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ Other, Other>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ MockUnmarked, Other>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ Other, CommandResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ Other, EventResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ Other, NoResponse>()]
        // Unmarked (Special NoResponse)
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ Any, Other>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ Other, Any>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ Other, Other>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ MockUnmarked, Other>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ Other, CommandResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ Other, EventResponse>()]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ Other, NoResponse>()]
        public void IsApplicable_IsFalse_WhenMiddlewareSignatureDoesNotMatchOrGreaterLevelThanContracts
            <TContractRequest, TContractResponse, TMiddlewareRequest, TMiddlewareResponse>()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<TMiddlewareRequest, TMiddlewareResponse>();
            var contextType = typeof(IInvocationContext<TContractRequest, TContractResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.False, message: $"Middleware ({typeof(TMiddlewareRequest).Name} -> {typeof(TMiddlewareResponse).Name}) should NOT be applicable for contract ({typeof(TContractRequest).Name} -> {typeof(TContractResponse).Name})");
        }

        [Test]
        [ConformanceTest]
        public async Task HandleAsync_MiddlewareIsInvoked()
        {
            // Arrange
            // No arrange required

            // Act
            await MiddlewareItem.HandleAsync(Context, Next, CancellationToken);

            // Assert
            Middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once, failMessage: "HandleAsync should called resolved middleware");
        }

        [Test]
        [ConformanceTest]
        public async Task HandleAsync_NextItemInChainIsInvoked()
        {
            // Arrange
            // No arrange required

            // Act
            await MiddlewareItem.HandleAsync(Context, Next, CancellationToken);

            // Assert
            Next.Mock.Verify(m => m(It.IsAny<CancellationToken>()), Times.Once);
        }

        protected abstract IMiddlewareInvocationPipelineItem CreateItemForMiddlewareSignature<TMiddlewareRequest, TMiddlewareResponse>();
    }

    public class Other { }
}
