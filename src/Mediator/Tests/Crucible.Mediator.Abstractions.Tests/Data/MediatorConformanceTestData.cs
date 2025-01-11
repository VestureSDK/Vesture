using Crucible.Mediator.Abstractions.Tests.Commands.Mocks;
using Crucible.Mediator.Abstractions.Tests.Events.Mocks;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Abstractions.Tests.Requests.Mocks;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;

#pragma warning disable CA2211 // Non-constant fields should not be visible
namespace Crucible.Mediator.Abstractions.Tests.Data
{
    public class MediatorConformanceTestData
    {
        public static object[] GetCommandContracts_ReqGenerics_TestData() => GetCommandContracts_ReqGenerics_TestData_WithParams([]);
        public static object[] GetCommandContracts_ReqGenerics_TestData_WithParams(object[] @params) =>
        [
            new object[] { new MockCommand() },
            new object[] { new MockUnmarked() },
        ];

        public static object[] CommandContracts_ReqResGenerics_TestData;

        public static object[] EventContracts_ReqGenerics_TestData =
        {
            new object[] { new MockEvent() },
            new object[] { new MockUnmarked() },
        };

        public static object[] EventContracts_ReqResGenerics_TestData =
        {
            new object[] { new MockEvent(), new EventResponse() },
            new object[] { new MockUnmarked(), new EventResponse() },
        };

        public static object[] RequestContracts_ReqResGenerics_TestData =
        {
            new object[] { new MockRequest(), new MockResponse() },
            new object[] { new MockUnmarked(), new MockUnmarked() },
        };

        public static object[] NoResponseContracts_ReqResGenerics_TestData =
        {
            new object[] { new MockUnmarked(), new NoResponse() },
        };

        public static object[] All_NoResponseContracts_ReqResGenerics_TestData;

        public static object[] All_Contracts_ReqResGenerics_TestData;

        static MediatorConformanceTestData()
        {
            CommandContracts_ReqResGenerics_TestData = GetCommandContracts_ReqGenerics_TestData().Cast<object[]>()
                .Select(a => new object[] { a[0], new CommandResponse() })
                .ToArray();

            var allNoResponseContracts = new List<object[]>();
            allNoResponseContracts.AddRange(CommandContracts_ReqResGenerics_TestData);
            allNoResponseContracts.AddRange(EventContracts_ReqResGenerics_TestData);
            allNoResponseContracts.AddRange(NoResponseContracts_ReqResGenerics_TestData);
            All_NoResponseContracts_ReqResGenerics_TestData = allNoResponseContracts.ToArray();

            var allContracts = new List<object[]>();
            allContracts.AddRange(RequestContracts_ReqResGenerics_TestData);
            allContracts.AddRange(CommandContracts_ReqResGenerics_TestData);
            allContracts.AddRange(EventContracts_ReqResGenerics_TestData);
            allContracts.AddRange(NoResponseContracts_ReqResGenerics_TestData);
            All_Contracts_ReqResGenerics_TestData = allContracts.ToArray();
        }
    }
}
#pragma warning restore CA2211 // Non-constant fields should not be visible
