namespace Crucible.Mediator.Tests.Requests
{
    public class RequestMediatorTest : MediatorTest
    {
        protected MockRequestHandler Handler { get; } = new MockRequestHandler();

        protected TestMediatorRequest Request { get; } = new TestMediatorRequest();

        public RequestMediatorTest()
            : base()
        {
            MediatorDiBuilder
                .Request<TestMediatorRequest, TestMediatorResponse>()
                    .HandleWith(Handler);
        }

        [Fact]
        public async Task ExecuteAsync_Throws_WhenRequestIsNull()
        {
            // Arrange
            // no arrange required

            // Act
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => Mediator.ExecuteAsync<TestMediatorResponse>(request: null, CancellationToken));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Assert
            Assert.Equal("request", exception.ParamName);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("b")]
        [InlineData("c")]
        public async Task ExecuteAsync_Response_HasExpectedValue(string value)
        {
            // Arrange
            Handler.MockExecute = (request) =>
            {
                return new TestMediatorResponse
                {
                    TestProperty = value,
                };
            };

            // Act
            TestMediatorResponse response = await Mediator.ExecuteAsync(Request, CancellationToken);

            // Assert
            Assert.Equal(value, response.TestProperty);
        }
    }
}
