namespace Crucible.Mediator.Abstractions.Tests.Data.Annotations.Requests
{
    public class TestCaseSource_RequestResponse_UnmarksAttribute : TestCaseSourceAttribute
    {
        public TestCaseSource_RequestResponse_UnmarksAttribute()
            : base(typeof(TestCaseSource_RequestResponse_UnmarksAttribute), nameof(TestData)) { }

        public static IEnumerable<TestCaseData> TestData => MediatorTestData
            .Get_RequestResponse_Unmarked()
            .Select(x => new TestCaseData(x.Request, x.Response));
    }
}
