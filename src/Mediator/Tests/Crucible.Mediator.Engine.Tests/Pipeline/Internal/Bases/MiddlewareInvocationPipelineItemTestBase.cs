using Crucible.Mediator.Abstractions.Tests.Data.Annotations.Invocation;
using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Invocation;
using Moq;

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

        [Test]
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

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: true)]
        public void IsApplicable_IsTrue_WhenMiddlewareSignatureMacthesOrLowerLevelThanContracts
            <TContractRequest, TContractResponse, TMiddlewareRequest, TMiddlewareResponse>
            (TContractRequest request, TContractResponse response, TMiddlewareRequest middlewareRequest, TMiddlewareResponse middlewareResponse)
        {
            // Arrange
            var middlewareItem = CreateItemForMiddlewareSignature<TMiddlewareRequest, TMiddlewareResponse>();
            var contextType = typeof(IInvocationContext<TContractRequest, TContractResponse>);

            // Act
            var isApplicable = middlewareItem.IsApplicable(contextType);

            // Asser
            Assert.That(isApplicable, Is.True, message: $"Middleware ({typeof(TMiddlewareRequest).Name} -> {typeof(TMiddlewareResponse).Name}) should be applicable for contract ({typeof(TContractRequest).Name} -> {typeof(TContractResponse).Name})");
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: false)]
        public void IsApplicable_IsFalse_WhenMiddlewareSignatureDoesNotMatchOrGreaterLevelThanContracts
            <TContractRequest, TContractResponse, TMiddlewareRequest, TMiddlewareResponse>
            (TContractRequest request, TContractResponse response, TMiddlewareRequest middlewareRequest, TMiddlewareResponse middlewareResponse)
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
