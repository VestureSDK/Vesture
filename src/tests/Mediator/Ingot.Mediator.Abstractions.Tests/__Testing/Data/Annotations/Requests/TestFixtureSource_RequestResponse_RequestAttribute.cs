namespace Ingot.Mediator.Abstractions.Tests.Data.Annotations.Requests
{
    public class TestFixtureSource_RequestResponse_RequestAttribute : TestFixtureSourceAttribute
    {
        public TestFixtureSource_RequestResponse_RequestAttribute()
            : base(typeof(TestFixtureSource_RequestResponse_RequestAttribute), nameof(TestData)) { }

        public static IEnumerable<TestFixtureData> TestData =>
            MediatorTestData
                .Get_RequestResponse_Request()
                .Select(x => new TestFixtureData(x.Request, x.Response));
    }
}
