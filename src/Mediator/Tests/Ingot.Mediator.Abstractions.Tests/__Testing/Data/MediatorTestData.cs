using Ingot.Mediator.Abstractions.Tests.Data.Internal;
using Ingot.Mediator.Commands;
using Ingot.Mediator.Events;
using Ingot.Mediator.Invocation;
using Ingot.Mediator.Mocks.Commands;
using Ingot.Mediator.Mocks.Events;
using Ingot.Mediator.Mocks.Invocation;
using Ingot.Mediator.Mocks.Requests;
using Any = object;

namespace Ingot.Mediator.Abstractions.Tests.Data
{
    public static class MediatorTestData
    {
        public static IEnumerable<ContractRequestTestData> Get_Request_Command() => [
            new() { Request = new MockCommand() }
        ];

        public static IEnumerable<ContractRequestResponseTestData> Get_RequestResponse_Command() =>
            Get_Request_Command()
            .Select(data => new ContractRequestResponseTestData
            {
                Request = data.Request,
                Response = new CommandResponse()
            });

        public static IEnumerable<ContractRequestTestData> Get_Request_Event() => [
            new() { Request = new MockEvent() }
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
            new() { Request = new MockUnmarked(), Response = new MockUnmarked() },
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

        public static IEnumerable<ContractRequestResponseMediatorRequestResponseTestData> Get_RequestResponseMediatorRequestResponse_IsApplicable_All(Func<ContractRequestResponseMediatorRequestResponseTestData, bool>? predicate = null)
        {
            var data = new List<ContractRequestResponseMediatorRequestResponseTestData>();

            var allRequestResponses = Get_RequestResponse_All();
            foreach (var requestResponse in allRequestResponses)
            {
                data.Add(new()
                {
                    Request = requestResponse.Request,
                    Response = requestResponse.Response,
                    MediatorRequest = new Any(),
                    MediatorResponse = new Any(),
                });

                data.Add(new()
                {
                    Request = requestResponse.Request,
                    Response = requestResponse.Response,
                    MediatorRequest = requestResponse.Request,
                    MediatorResponse = new Any(),
                });

                data.Add(new()
                {
                    Request = requestResponse.Request,
                    Response = requestResponse.Response,
                    MediatorRequest = new Any(),
                    MediatorResponse = requestResponse.Response,
                });

                data.Add(new()
                {
                    Request = requestResponse.Request,
                    Response = requestResponse.Response,
                    MediatorRequest = requestResponse.Request,
                    MediatorResponse = requestResponse.Response,
                });

                if (requestResponse.Request is MockContract && requestResponse.Request.GetType() != typeof(MockContract))
                {
                    data.Add(new()
                    {
                        Request = requestResponse.Request,
                        Response = requestResponse.Response,
                        MediatorRequest = new MockContract(),
                        MediatorResponse = requestResponse.Response,
                    });

                    data.Add(new()
                    {
                        Request = requestResponse.Request,
                        Response = requestResponse.Response,
                        MediatorRequest = new MockContract(),
                        MediatorResponse = new Any(),
                    });
                }

                if (requestResponse.Response is MockContract && requestResponse.Response.GetType() != typeof(MockContract))
                {
                    data.Add(new()
                    {
                        Request = requestResponse.Request,
                        Response = requestResponse.Response,
                        MediatorRequest = requestResponse.Request,
                        MediatorResponse = new MockContract(),
                    });

                    data.Add(new()
                    {
                        Request = requestResponse.Request,
                        Response = requestResponse.Response,
                        MediatorRequest = new Any(),
                        MediatorResponse = new MockContract(),
                    });
                }

                if (requestResponse.Response is NoResponse && requestResponse.Response.GetType() != NoResponse.Type)
                {
                    data.Add(new()
                    {
                        Request = requestResponse.Request,
                        Response = requestResponse.Response,
                        MediatorRequest = requestResponse.Request,
                        MediatorResponse = new NoResponse(),
                    });
                }
            }

            return data.Where(d => predicate?.Invoke(d) ?? true).ToList();
        }

        public static IEnumerable<ContractRequestResponseMediatorRequestResponseTestData> Get_RequestResponseMediatorRequestResponse_IsNotApplicable_All(Func<ContractRequestResponseMediatorRequestResponseTestData, bool>? predicate = null)
        {
            var data = new List<ContractRequestResponseMediatorRequestResponseTestData>();

            var allRequestResponses = Get_RequestResponse_All();
            foreach (var requestResponse in allRequestResponses)
            {
                data.Add(new()
                {
                    Request = requestResponse.Request,
                    Response = requestResponse.Response,
                    MediatorRequest = requestResponse.Request,
                    MediatorResponse = new Unrelated(),
                });

                data.Add(new()
                {
                    Request = requestResponse.Request,
                    Response = requestResponse.Response,
                    MediatorRequest = new Unrelated(),
                    MediatorResponse = requestResponse.Response,
                });

                data.Add(new()
                {
                    Request = requestResponse.Request,
                    Response = requestResponse.Response,
                    MediatorRequest = new Unrelated(),
                    MediatorResponse = new Unrelated(),
                });

                data.Add(new()
                {
                    Request = requestResponse.Request,
                    Response = requestResponse.Response,
                    MediatorRequest = new Any(),
                    MediatorResponse = new Unrelated(),
                });

                data.Add(new()
                {
                    Request = requestResponse.Request,
                    Response = requestResponse.Response,
                    MediatorRequest = new Unrelated(),
                    MediatorResponse = new Any(),
                });

                if (requestResponse.Request is MockContract && requestResponse.Request.GetType() != typeof(MockContract))
                {
                    data.Add(new()
                    {
                        Request = requestResponse.Request,
                        Response = requestResponse.Response,
                        MediatorRequest = new MockContract(),
                        MediatorResponse = new Unrelated(),
                    });
                }

                if (requestResponse.Response is MockContract && requestResponse.Response.GetType() != typeof(MockContract))
                {
                    data.Add(new()
                    {
                        Request = requestResponse.Request,
                        Response = requestResponse.Response,
                        MediatorRequest = new Unrelated(),
                        MediatorResponse = new MockContract(),
                    });
                }
            }

            return data.Where(d => predicate?.Invoke(d) ?? true).ToList();
        }

        public class Unrelated
        {

        }
    }
}
