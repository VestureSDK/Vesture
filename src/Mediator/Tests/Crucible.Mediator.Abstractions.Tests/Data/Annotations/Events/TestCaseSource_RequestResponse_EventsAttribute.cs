namespace Crucible.Mediator.Abstractions.Tests.Data.Annotations.Events
{
    public class TestCaseSource_RequestResponse_EventsAttribute : TestCaseSourceAttribute
    {
        public TestCaseSource_RequestResponse_EventsAttribute()
            : base(typeof(TestCaseSource_RequestResponse_EventsAttribute), nameof(TestData)) { }

        public static IEnumerable<TestCaseData> TestData => MediatorTestData
            .Get_RequestResponse_Event()
            .Select(x => new TestCaseData(x.Request, x.Response));
    }
}
