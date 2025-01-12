namespace Crucible.Mediator.Abstractions.Tests.Data.Annotations.Requests
{
    public class TestCaseSource_RequestResponse_NoResponsesAttribute : TestCaseSourceAttribute
    {
        public TestCaseSource_RequestResponse_NoResponsesAttribute()
            : base(typeof(TestCaseSource_RequestResponse_NoResponsesAttribute), nameof(TestData)) { }

        public static IEnumerable<TestCaseData> TestData => MediatorTestData
            .Get_RequestResponse_NoResponse()
            .Select(x => new TestCaseData(x.Request, x.Response));
    }
}
