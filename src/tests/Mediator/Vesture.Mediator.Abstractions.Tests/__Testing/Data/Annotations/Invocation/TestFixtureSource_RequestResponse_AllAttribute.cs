namespace Vesture.Mediator.Abstractions.Tests.Data.Annotations.Commands
{
    public class TestFixtureSource_RequestResponse_AllAttribute : TestFixtureSourceAttribute
    {
        public TestFixtureSource_RequestResponse_AllAttribute()
            : base(typeof(TestFixtureSource_RequestResponse_AllAttribute), nameof(TestData)) { }

        public static IEnumerable<TestFixtureData> TestData =>
            MediatorTestData
                .Get_RequestResponse_All()
                .Select(x => new TestFixtureData(x.Request, x.Response));
    }
}
