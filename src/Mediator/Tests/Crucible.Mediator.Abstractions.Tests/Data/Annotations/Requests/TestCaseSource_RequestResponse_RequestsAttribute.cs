namespace Crucible.Mediator.Abstractions.Tests.Data.Annotations.Requests
{
    public class TestCaseSource_RequestResponse_RequestsAttribute : TestCaseSourceAttribute
    {
        public TestCaseSource_RequestResponse_RequestsAttribute()
            : base(typeof(TestCaseSource_RequestResponse_RequestsAttribute), nameof(TestData)) { }

        public static IEnumerable<TestCaseData> TestData => MediatorTestData
            .Get_RequestResponse_Request()
            .Select(x => new TestCaseData(x.Request, x.Response));
    }
}
