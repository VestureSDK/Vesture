namespace Ingot.Mediator.Abstractions.Tests.Data.Annotations.Requests
{
    public class TestCaseSource_RequestResponse_RequestAttribute : TestCaseSourceAttribute
    {
        public TestCaseSource_RequestResponse_RequestAttribute(params object[] methodParams)
            : base(
                typeof(TestCaseSource_RequestResponse_RequestAttribute),
                nameof(TestData),
                methodParams?.Length > 0 ? [methodParams] : [Array.Empty<object>()]
            ) { }

        public static object[] TestData(object[] methodParams) =>
            MediatorTestData
                .Get_RequestResponse_Request()
                .Select(x =>
                    new object[] { x.Request, x.Response }
                        .Concat(methodParams)
                        .ToArray()
                )
                .ToArray();
    }
}
