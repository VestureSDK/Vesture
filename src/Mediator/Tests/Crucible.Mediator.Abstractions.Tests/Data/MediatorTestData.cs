using System.Collections.Generic;
using Crucible.Mediator.Abstractions.Tests.Commands.Mocks;
using Crucible.Mediator.Abstractions.Tests.Data.Internal;
using Crucible.Mediator.Abstractions.Tests.Events.Mocks;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Abstractions.Tests.Requests.Mocks;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;

#pragma warning disable CA2211 // Non-constant fields should not be visible
namespace Crucible.Mediator.Abstractions.Tests.Data
{
    internal static class T
    {
        internal static object[] AppendExtraParams(this object[] data, object[] extraParams)
        {
            var dataExpanded = new List<object[]>();
            foreach (var item in data)
            {
                var lData = (object[])item;
                var uData = new object[lData.Length + extraParams.Length];
                for (var i = 0; i < lData.Length; i++)
                {
                    uData[i] = lData[i];
                }
                for (var i = lData.Length - 1; i < (extraParams.Length + (lData.Length - 1)); i++)
                {
                    uData[i] = lData[i];
                }
                dataExpanded.Add(uData);
            }
            return [.. dataExpanded];
        }
    }

    public partial class MediatorTestData
    {
        public static IEnumerable<ContractRequestTestData> Get_Request_Command() => [
            new() { Request = new MockCommand() },
            new() { Request = new MockUnmarked() }
        ];

        public static IEnumerable<ContractRequestResponseTestData> Get_RequestResponse_Command() =>
            Get_Request_Command()
            .Select(data => new ContractRequestResponseTestData { 
                Request = data.Request, 
                Response = new CommandResponse() 
            });

        public static IEnumerable<ContractRequestTestData> Get_Request_Event() => [
            new() { Request = new MockEvent() },
            new() { Request = new MockUnmarked() }
        ];

        public static IEnumerable<ContractRequestResponseTestData> Get_RequestResponse_Event() =>
            Get_Request_Event()
            .Select(data => new ContractRequestResponseTestData
            {
                Request = data.Request,
                Response = new EventResponse()
            });

        public static IEnumerable<ContractRequestResponseTestData> Get_RequestResponse_Request() => [
            new() { Request = new MockRequest(), Response = new MockResponse() }
        ];

        public static IEnumerable<ContractRequestResponseTestData> Get_RequestResponse_NoResponse() => [
            new() { Request = new MockUnmarked(), Response = new NoResponse() }
        ];

        public static IEnumerable<ContractRequestResponseTestData> Get_RequestResponse_Unmarked() => [
            new() { Request = new MockUnmarked(), Response = new MockUnmarked() }
        ];

        public static IEnumerable<ContractRequestResponseTestData> Get_RequestResponse_All()
        {
            var data = new List<ContractRequestResponseTestData>();

            data.AddRange(Get_RequestResponse_Command());
            data.AddRange(Get_RequestResponse_Event());
            data.AddRange(Get_RequestResponse_Request());
            data.AddRange(Get_RequestResponse_NoResponse());
            data.AddRange(Get_RequestResponse_Unmarked());

            return data;
        }

        static MediatorTestData()
        {
            //CommandContracts_ReqResGenerics_TestData = GetCommand_Request().Cast<object[]>()
            //    .Select(a => new object[] { a[0], new CommandResponse() })
            //    .ToArray();

            //var allNoResponseContracts = new List<object[]>();
            //allNoResponseContracts.AddRange(CommandContracts_ReqResGenerics_TestData);
            //allNoResponseContracts.AddRange(EventContracts_ReqResGenerics_TestData);
            //allNoResponseContracts.AddRange(NoResponseContracts_ReqResGenerics_TestData);
            //All_NoResponseContracts_ReqResGenerics_TestData = allNoResponseContracts.ToArray();

            //var allContracts = new List<object[]>();
            //allContracts.AddRange(RequestContracts_ReqResGenerics_TestData);
            //allContracts.AddRange(CommandContracts_ReqResGenerics_TestData);
            //allContracts.AddRange(EventContracts_ReqResGenerics_TestData);
            //allContracts.AddRange(NoResponseContracts_ReqResGenerics_TestData);
            //All_Contracts_ReqResGenerics_TestData = allContracts.ToArray();
        }
    }
}
#pragma warning restore CA2211 // Non-constant fields should not be visible
