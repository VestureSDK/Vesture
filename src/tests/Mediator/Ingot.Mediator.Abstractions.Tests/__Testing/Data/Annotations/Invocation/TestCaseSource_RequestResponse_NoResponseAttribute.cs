namespace Ingot.Mediator.Abstractions.Tests.Data.Annotations.Requests
{
    public class TestCaseSource_RequestResponse_NoResponseAttribute : TestCaseSourceAttribute
    {
        public TestCaseSource_RequestResponse_NoResponseAttribute(params object[] methodParams)
            : base(
                typeof(TestCaseSource_RequestResponse_NoResponseAttribute),
                nameof(TestData),
                methodParams?.Length > 0 ? [methodParams] : [Array.Empty<object>()]
            ) { }

        public static object[] TestData(object[] methodParams) =>
            MediatorTestData
                .Get_RequestResponse_NoResponse()
                .Select(x =>
                    new object[] { x.Request, x.Response }
                        .Concat(methodParams)
                        .ToArray()
                )
                .ToArray();
    }
}
