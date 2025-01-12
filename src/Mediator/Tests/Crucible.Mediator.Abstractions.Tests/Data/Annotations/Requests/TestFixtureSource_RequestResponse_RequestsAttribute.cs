namespace Crucible.Mediator.Abstractions.Tests.Data.Annotations.Requests
{
    public class TestFixtureSource_RequestResponse_RequestsAttribute : TestFixtureSourceAttribute
    {
        public TestFixtureSource_RequestResponse_RequestsAttribute()
            : base(typeof(TestFixtureSource_RequestResponse_RequestsAttribute), nameof(TestData)) { }

        public static IEnumerable<TestFixtureData> TestData => MediatorTestData
            .Get_RequestResponse_Request()
            .Select(x => new TestFixtureData(x.Request, x.Response));
    }
}
