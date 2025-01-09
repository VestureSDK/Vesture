using Crucible.Mediator.Abstractions.Tests.Commands;
using Crucible.Mediator.Abstractions.Tests.Events;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Abstractions.Tests.Requests;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Any = object;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Internal.Bases
{
    public abstract class MiddlewareInvocationPipelineItemTestBase<TMiddlewareItem>
        where TMiddlewareItem : IMiddlewareInvocationPipelineItem
    {
        protected int Order { get; set; }

        protected Lazy<TMiddlewareItem> MiddlewareItemInitializer { get; }

        protected TMiddlewareItem MiddlewareItem => MiddlewareItemInitializer.Value;

        public MiddlewareInvocationPipelineItemTestBase()
        {
#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
            MiddlewareItemInitializer = new Lazy<TMiddlewareItem>(() => CreateMiddlewareItem(Order));
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        }

        protected abstract TMiddlewareItem CreateMiddlewareItem(int order);

        [Theory]
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
        // IRequest<TResponse>
        [CustomTestCase</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Any, Any>()]
        [CustomTestCase</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ MockRequest, Any>()]
        [CustomTestCase</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Any, MockResponse>()]
        [CustomTestCase</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ MockRequest, MockResponse>()]
        // ICommand
        [CustomTestCase</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ Any, Any>()]
        [CustomTestCase</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ MockCommand, Any>()]
        [CustomTestCase</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ Any, CommandResponse>()]
        [CustomTestCase</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ Any, NoResponse>()]
        [CustomTestCase</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ MockCommand, CommandResponse>()]
        [CustomTestCase</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ MockCommand, NoResponse>()]
        // IEvent
        [CustomTestCase</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ Any, Any>()]
        [CustomTestCase</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ MockEvent, Any>()]
        [CustomTestCase</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ Any, EventResponse>()]
        [CustomTestCase</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ Any, NoResponse>()]
        [CustomTestCase</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ MockEvent, EventResponse>()]
        [CustomTestCase</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ MockEvent, NoResponse>()]
        // Unmarked (IRequest<TResponse>)
        [CustomTestCase</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ Any, Any>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ MockUnmarked, Any>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ Any, MockUnmarked>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ MockUnmarked, MockUnmarked>()]
        // Unmarked (ICommand)
        [CustomTestCase</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ Any, Any>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ MockUnmarked, Any>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ Any, CommandResponse>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ Any, NoResponse>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ MockUnmarked, CommandResponse>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ MockUnmarked, NoResponse>()]
        // Unmarked (IEvent)
        [CustomTestCase</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ Any, Any>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ MockUnmarked, Any>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ Any, EventResponse>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ Any, NoResponse>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ MockUnmarked, EventResponse>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ MockUnmarked, NoResponse>()]
        // Unmarked (Special NoResponse)
        [CustomTestCase</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ Any, Any>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ MockUnmarked, Any>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ Any, NoResponse>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ MockUnmarked, NoResponse>()]
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
        // IRequest<TResponse>
        [CustomTestCase</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Any, Other>()]
        [CustomTestCase</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Other, Any>()]
        [CustomTestCase</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ MockRequest, Other>()]
        [CustomTestCase</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Other, MockResponse>()]
        [CustomTestCase</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Other, CommandResponse>()]
        [CustomTestCase</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Other, EventResponse>()]
        [CustomTestCase</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Other, NoResponse>()]
        [CustomTestCase</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Other, Other>()]
        // ICommand
        [CustomTestCase</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ Any, Other>()]
        [CustomTestCase</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ Other, Any>()]
        [CustomTestCase</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ Other, Other>()]
        [CustomTestCase</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ MockCommand, Other>()]
        [CustomTestCase</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ Other, CommandResponse>()]
        [CustomTestCase</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ Other, EventResponse>()]
        [CustomTestCase</*Contract:*/ MockCommand, CommandResponse, /*Middleware:*/ Other, NoResponse>()]
        // IEvent
        [CustomTestCase</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ Any, Other>()]
        [CustomTestCase</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ Other, Any>()]
        [CustomTestCase</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ Other, Other>()]
        [CustomTestCase</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ MockEvent, Other>()]
        [CustomTestCase</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ Other, CommandResponse>()]
        [CustomTestCase</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ Other, EventResponse>()]
        [CustomTestCase</*Contract:*/ MockEvent, EventResponse, /*Middleware:*/ Other, NoResponse>()]
        // Unmarked (IRequest<TResponse>)
        [CustomTestCase</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ Any, Other>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ Other, Any>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ MockUnmarked, Other>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ Other, MockUnmarked>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ Other, CommandResponse>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ Other, EventResponse>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ Other, NoResponse>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, MockUnmarked, /*Middleware:*/ Other, Other>()]
        // Unmarked (ICommand)
        [CustomTestCase</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ Any, Other>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ Other, Any>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ Other, Other>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ MockUnmarked, Other>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ Other, CommandResponse>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ Other, EventResponse>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, CommandResponse, /*Middleware:*/ Other, NoResponse>()]
        // Unmarked (IEvent)
        [CustomTestCase</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ Any, Other>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ Other, Any>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ Other, Other>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ MockUnmarked, Other>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ Other, CommandResponse>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ Other, EventResponse>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, EventResponse, /*Middleware:*/ Other, NoResponse>()]
        // Unmarked (Special NoResponse)
        [CustomTestCase</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ Any, Other>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ Other, Any>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ Other, Other>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ MockUnmarked, Other>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ Other, CommandResponse>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ Other, EventResponse>()]
        [CustomTestCase</*Contract:*/ MockUnmarked, NoResponse, /*Middleware:*/ Other, NoResponse>()]
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

        protected abstract IMiddlewareInvocationPipelineItem CreateItemForMiddlewareSignature<TMiddlewareRequest, TMiddlewareResponse>();
    }

    public class Other { }
}
