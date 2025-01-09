using Crucible.Mediator.Commands;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;
using Any = object;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Internal.Bases
{
    public abstract class MiddlewareInvocationPipelineItemTestBase<TRequest, TResponse, TMiddlewareItem>
        where TMiddlewareItem : IMiddlewareInvocationPipelineItem<TRequest, TResponse>
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
            Assert.That(actualOrder, Is.EqualTo(expectedOrder), message: "Actual order should be equalt to provided ctor order");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleRequest_WhenMiddlewareIsAnyAny()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, Any>();
            var contextType = typeof(IInvocationContext<SampleRequest, SampleResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleRequest -> SampleResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleRequest_WhenMiddlewareIsSampleRequestAny()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleRequest, Any>();
            var contextType = typeof(IInvocationContext<SampleRequest, SampleResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleRequest -> SampleResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleRequest_WhenMiddlewareIsAnySampleResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, SampleResponse>();
            var contextType = typeof(IInvocationContext<SampleRequest, SampleResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleRequest -> SampleResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleRequest_WhenMiddlewareIsSampleRequestSampleResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleRequest, SampleResponse>();
            var contextType = typeof(IInvocationContext<SampleRequest, SampleResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleRequest -> SampleResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleCommand_WhenMiddlewareIsAnyAny()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, Any>();
            var contextType = typeof(IInvocationContext<SampleCommand, CommandResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleCommand -> CommandResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleCommand_WhenMiddlewareIsSampleCommandAny()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleCommand, Any>();
            var contextType = typeof(IInvocationContext<SampleCommand, CommandResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleCommand -> CommandResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleCommand_WhenMiddlewareIsAnyCommandResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, CommandResponse>();
            var contextType = typeof(IInvocationContext<SampleCommand, CommandResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleCommand -> CommandResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleCommand_WhenMiddlewareIsAnyNoResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, NoResponse>();
            var contextType = typeof(IInvocationContext<SampleCommand, CommandResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleCommand -> CommandResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleCommand_WhenMiddlewareIsSampleCommandCommandResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleCommand, CommandResponse>();
            var contextType = typeof(IInvocationContext<SampleCommand, CommandResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleCommand -> CommandResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleCommand_WhenMiddlewareIsSampleCommandNoResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleCommand, NoResponse>();
            var contextType = typeof(IInvocationContext<SampleCommand, CommandResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleCommand -> CommandResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleEvent_WhenMiddlewareIsAnyAny()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, Any>();
            var contextType = typeof(IInvocationContext<SampleEvent, EventResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleEvent -> EventResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleEvent_WhenMiddlewareIsSampleEventAny()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleEvent, Any>();
            var contextType = typeof(IInvocationContext<SampleEvent, EventResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleEvent -> EventResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleEvent_WhenMiddlewareIsAnyEventResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, EventResponse>();
            var contextType = typeof(IInvocationContext<SampleEvent, EventResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleEvent -> EventResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleEvent_WhenMiddlewareIsAnyNoResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, NoResponse>();
            var contextType = typeof(IInvocationContext<SampleEvent, EventResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleEvent -> EventResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleEvent_WhenMiddlewareIsSampleEventEventResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleEvent, EventResponse>();
            var contextType = typeof(IInvocationContext<SampleEvent, EventResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleEvent -> EventResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleEvent_WhenMiddlewareIsSampleEventNoResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleEvent, NoResponse>();
            var contextType = typeof(IInvocationContext<SampleEvent, EventResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleEvent -> EventResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndSampleUnmarked_WhenMiddlewareIsAnyAny()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, Any>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, SampleUnmarked>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> SampleUnmarked)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndSampleUnmarked_WhenMiddlewareIsSampleUnmarkedAny()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleUnmarked, Any>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, SampleUnmarked>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> SampleUnmarked)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndSampleUnmarked_WhenMiddlewareIsAnySampleUnmarked()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, SampleUnmarked>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, SampleUnmarked>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> SampleUnmarked)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndSampleUnmarked_WhenMiddlewareIsSampleUnmarkedSampleUnmarked()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleUnmarked, SampleUnmarked>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, SampleUnmarked>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> SampleUnmarked)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndCommandResponse_WhenMiddlewareIsAnyAny()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, Any>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, CommandResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> CommandResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndCommandResponse_WhenMiddlewareIsSampleUnmarkedAny()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleUnmarked, Any>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, CommandResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> CommandResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndCommandResponse_WhenMiddlewareIsAnyCommandResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, CommandResponse>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, CommandResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> CommandResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndCommandResponse_WhenMiddlewareIsAnyNoResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, NoResponse>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, CommandResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> CommandResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndCommandResponse_WhenMiddlewareIsSampleUnmarkedCommandResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleUnmarked, CommandResponse>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, CommandResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> CommandResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndCommandResponse_WhenMiddlewareIsSampleUnmarkedNoResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleUnmarked, NoResponse>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, CommandResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> CommandResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndEventResponse_WhenMiddlewareIsAnyAny()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, Any>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, EventResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> EventResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndEventResponse_WhenMiddlewareIsSampleUnmarkedAny()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleUnmarked, Any>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, EventResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> EventResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndEventResponse_WhenMiddlewareIsAnyEventResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, EventResponse>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, EventResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> EventResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndEventResponse_WhenMiddlewareIsAnyNoResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, NoResponse>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, EventResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> EventResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndEventResponse_WhenMiddlewareIsSampleUnmarkedEventResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleUnmarked, EventResponse>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, EventResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> EventResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndEventResponse_WhenMiddlewareIsSampleUnmarkedNoResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleUnmarked, NoResponse>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, EventResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> EventResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndNoResponse_WhenMiddlewareIsAnyAny()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, Any>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, NoResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> NoResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndNoResponse_WhenMiddlewareIsSampleUnmarkedAny()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleUnmarked, Any>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, NoResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> NoResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndNoResponse_WhenMiddlewareIsAnyNoResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, NoResponse>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, NoResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> NoResponse)");
        }

        [Test]
        public void IsApplicable_IsTrue_ForSampleUnmarkedAndNoResponse_WhenMiddlewareIsSampleUnmarkedNoResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleUnmarked, NoResponse>();
            var contextType = typeof(IInvocationContext<SampleUnmarked, NoResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: "Should be applicable for contract (SampleUnmarked -> NoResponse)");
        }

        [Test]
        public void IsApplicable_IsFalse_WhenMiddlewareIsSampleUnmarkedAny()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleUnmarked, Any>();
            var contextType = typeof(IInvocationContext<Any, Any>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.False, message: "Should not be applicable for contract (any -> any)");
        }

        [Test]
        public void IsApplicable_IsFalse_WhenMiddlewareIsAnySampleUnmarked()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, SampleUnmarked>();
            var contextType = typeof(IInvocationContext<Any, Any>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.False, message: "Should not be applicable for contract (any -> any)");
        }

        [Test]
        public void IsApplicable_IsFalse_WhenMiddlewareIsSampleUnmarkedSampleUnmarked()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<SampleUnmarked, SampleUnmarked>();
            var contextType = typeof(IInvocationContext<Any, Any>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.False, message: "Should not be applicable for contract (any -> any)");
        }

        [Test]
        public void IsApplicable_IsFalse_WhenMiddlewareIsAnyCommandResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, CommandResponse>();
            var contextType = typeof(IInvocationContext<Any, Any>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.False, message: "Should not be applicable for contract (any -> any)");
        }

        [Test]
        public void IsApplicable_IsFalse_WhenMiddlewareIsAnyEventResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, EventResponse>();
            var contextType = typeof(IInvocationContext<Any, Any>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.False, message: "Should not be applicable for contract (any -> any)");
        }

        [Test]
        public void IsApplicable_IsFalse_WhenMiddlewareIsAnyNoResponse()
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<Any, NoResponse>();
            var contextType = typeof(IInvocationContext<Any, Any>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.False, message: "Should not be applicable for contract (any -> any)");
        }

        protected abstract IMiddlewareInvocationPipelineItem CreateItemForMiddlewareSignature<TMiddlewareRequest, TMiddlewareResponse>();

        public class SampleUnmarked { }

        public class SampleResponse { }

        public class SampleRequest : IRequest<SampleResponse> { }

        public class SampleEvent : IEvent { }

        public class SampleCommand : IEvent { }
    }
}
