namespace Crucible.Mediator.Abstractions.Tests.Data.Annotations.Commands
{
    public class TestCaseSource_RequestResponse_CommandAttribute : TestCaseSourceAttribute
    {
        public TestCaseSource_RequestResponse_CommandAttribute(params object[] methodParams)
            : base(typeof(TestCaseSource_RequestResponse_CommandAttribute), nameof(TestData), methodParams?.Length > 0 ? [methodParams] : [Array.Empty<object>()]) { }

        public static object[] TestData(object[] methodParams) => MediatorTestData
            .Get_RequestResponse_Command()
            .Select(x => new object[] { x.Request, x.Response }.AppendExtraParams(methodParams))
            .ToArray();
    }
}
