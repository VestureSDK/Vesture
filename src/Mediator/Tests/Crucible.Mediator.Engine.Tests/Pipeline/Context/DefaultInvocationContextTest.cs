using Crucible.Mediator.Commands;
using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Engine.Tests.Pipeline.Context.Bases;
using Crucible.Mediator.Events;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Context
{
    public class DefaultInvocationContextTest_Event : DefaultInvocationContextTestBase<object, EventResponse>
    {
        public DefaultInvocationContextTest_Event()
            : base(new object()) { }

        [Test]
        public void IsEvent_IsTrue()
        {
            // Arrange
            // No arrange required

            // Act
            var isEvent = InvocationContext.IsEvent;

            // Assert
            Assert.That(isEvent, Is.True, message: $"{nameof(DefaultInvocationContext.IsEvent)} should be {true}");
        }

        [Test]
        public void IsCommand_IsFalse()
        {
            // Arrange
            // No arrange required

            // Act
            var isCommand = InvocationContext.IsCommand;

            // Assert
            Assert.That(isCommand, Is.False, message: $"{nameof(DefaultInvocationContext.IsCommand)} should be {false}");
        }

        [Test]
        public void IsRequest_IsFalse()
        {
            // Arrange
            // No arrange required

            // Act
            var isRequest = InvocationContext.IsRequest;

            // Assert
            Assert.That(isRequest, Is.False, message: $"{nameof(DefaultInvocationContext.IsRequest)} should be {false}");
        }

        [Test]
        public void HasResponseType_IsFalse()
        {
            // Arrange
            // No arrange required

            // Act
            var hasResponseType = InvocationContext.HasResponseType;

            // Assert
            Assert.That(hasResponseType, Is.False, message: $"{nameof(DefaultInvocationContext.HasResponseType)} should be {false}");
        }
    }

    public class DefaultInvocationContextTest_Command : DefaultInvocationContextTestBase<object, CommandResponse>
    {
        public DefaultInvocationContextTest_Command()
            : base(new object()) { }

        [Test]
        public void IsEvent_IsFalse()
        {
            // Arrange
            // No arrange required

            // Act
            var isEvent = InvocationContext.IsEvent;

            // Assert
            Assert.That(isEvent, Is.False, message: $"{nameof(DefaultInvocationContext.IsEvent)} should be {false}");
        }

        [Test]
        public void IsCommand_IsTrue()
        {
            // Arrange
            // No arrange required

            // Act
            var isCommand = InvocationContext.IsCommand;

            // Assert
            Assert.That(isCommand, Is.True, message: $"{nameof(DefaultInvocationContext.IsCommand)} should be {true}");
        }

        [Test]
        public void IsRequest_IsFalse()
        {
            // Arrange
            // No arrange required

            // Act
            var isRequest = InvocationContext.IsRequest;

            // Assert
            Assert.That(isRequest, Is.False, message: $"{nameof(DefaultInvocationContext.IsRequest)} should be {false}");
        }

        [Test]
        public void HasResponseType_IsFalse()
        {
            // Arrange
            // No arrange required

            // Act
            var hasResponseType = InvocationContext.HasResponseType;

            // Assert
            Assert.That(hasResponseType, Is.False, message: $"{nameof(DefaultInvocationContext.HasResponseType)} should be {false}");
        }
    }

    public class DefaultInvocationContextTest_Request : DefaultInvocationContextTestBase<object, object>
    {
        public DefaultInvocationContextTest_Request()
            : base(new object()) { }

        [Test]
        public void IsEvent_IsFalse()
        {
            // Arrange
            // No arrange required

            // Act
            var isEvent = InvocationContext.IsEvent;

            // Assert
            Assert.That(isEvent, Is.False, message: $"{nameof(DefaultInvocationContext.IsEvent)} should be {false}");
        }

        [Test]
        public void IsCommand_IsFalse()
        {
            // Arrange
            // No arrange required

            // Act
            var isCommand = InvocationContext.IsCommand;

            // Assert
            Assert.That(isCommand, Is.False, message: $"{nameof(DefaultInvocationContext.IsCommand)} should be {false}");
        }

        [Test]
        public void IsRequest_IsTrue()
        {
            // Arrange
            // No arrange required

            // Act
            var isRequest = InvocationContext.IsRequest;

            // Assert
            Assert.That(isRequest, Is.True, message: $"{nameof(DefaultInvocationContext.IsRequest)} should be {true}");
        }

        [Test]
        public void HasResponseType_IsTrue()
        {
            // Arrange
            // No arrange required

            // Act
            var hasResponseType = InvocationContext.HasResponseType;

            // Assert
            Assert.That(hasResponseType, Is.True, message: $"{nameof(DefaultInvocationContext.HasResponseType)} should be {true}");
        }

        [Test]
        public void HasResponse_IsFalse_WhenNoResponseSet()
        {
            // Arrange
            // No arrange required

            // Act
            var hasResponse = InvocationContext.HasResponse;

            // Assert
            Assert.That(hasResponse, Is.False, message: $"{nameof(DefaultInvocationContext.HasResponse)} should be {false} when no response is set");
        }

        [Test]
        public void HasResponse_IsTrue_WhenResponseSet()
        {
            // Arrange
            InvocationContext.SetResponse(new object());

            // Act
            var hasResponse = InvocationContext.HasResponse;

            // Assert
            Assert.That(hasResponse, Is.True, message: $"{nameof(DefaultInvocationContext.HasResponse)} should be {true} when a response is set");
        }

        [Test]
        public void SetResponse_Response_IsTheOneSet_WhenNoResponseSetBefore()
        {
            // Arrange
            var sampleResponse = new object();

            // Act
            InvocationContext.SetResponse(sampleResponse);

            // Assert
            Assert.That(InvocationContext.Response, Is.SameAs(sampleResponse), message: $"{nameof(DefaultInvocationContext.Response)} should be same as response set with {nameof(DefaultInvocationContext.SetResponse)} when no response set.");
        }

        [Test]
        public void SetResponse_Response_IsTheLastOneSet_WhenSettingMultipleResponses()
        {
            // Arrange
            var firstSampleResponse = new object();
            InvocationContext.SetResponse(firstSampleResponse);

            var lastSampleResponse = new object();

            // Act
            InvocationContext.SetResponse(lastSampleResponse);

            // Assert
            Assert.That(InvocationContext.Response, Is.SameAs(lastSampleResponse), message: $"{nameof(DefaultInvocationContext.Response)} should be same as the last response set with {nameof(DefaultInvocationContext.SetResponse)} when multiple response set.");
        }

        [Test]
        public void SetResponse_Response_IsUnset_WhenSettingNullResponse()
        {
            // Arrange
            InvocationContext.SetResponse(new object());

            // Act
            InvocationContext.SetResponse(null);

            // Assert
            Assert.That(InvocationContext.Response, Is.Null, message: $"{nameof(DefaultInvocationContext.Response)} should be null when calling {nameof(DefaultInvocationContext.SetResponse)} with null.");
        }
    }
}
