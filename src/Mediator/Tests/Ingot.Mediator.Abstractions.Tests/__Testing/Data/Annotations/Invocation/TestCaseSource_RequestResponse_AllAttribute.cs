namespace Ingot.Mediator.Abstractions.Tests.Data.Annotations.Requests
{
    public class TestCaseSource_RequestResponse_AllAttribute : TestCaseSourceAttribute
    {
        public TestCaseSource_RequestResponse_AllAttribute(params object[] methodParams)
            : base(typeof(TestCaseSource_RequestResponse_AllAttribute), nameof(TestData), methodParams?.Length > 0 ? [methodParams] : [Array.Empty<object>()]) { }

        public static object[] TestData(object[] methodParams) => MediatorTestData
            .Get_RequestResponse_All()
            .Select(x => new object[] { x.Request, x.Response }.Concat(methodParams).ToArray())
            .ToArray();
    }
}
