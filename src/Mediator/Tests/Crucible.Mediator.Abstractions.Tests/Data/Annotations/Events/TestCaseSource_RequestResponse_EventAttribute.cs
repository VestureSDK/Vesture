namespace Crucible.Mediator.Abstractions.Tests.Data.Annotations.Events
{
    public class TestCaseSource_RequestResponse_EventAttribute : TestCaseSourceAttribute
    {
        public TestCaseSource_RequestResponse_EventAttribute(params object[] methodParams)
            : base(typeof(TestCaseSource_RequestResponse_EventAttribute), nameof(TestData), methodParams?.Length > 0 ? [methodParams] : [Array.Empty<object>()]) { }

        public static object[] TestData(object[] methodParams) => MediatorTestData
            .Get_RequestResponse_Event()
            .Select(x => new object[] { x.Request, x.Response }.AppendExtraParams(methodParams))
            .ToArray();
    }
}
