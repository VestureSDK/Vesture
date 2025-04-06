namespace Vesture.Mediator.Abstractions.Tests.Data.Annotations.Commands
{
    public class TestFixtureSource_Request_CommandAttribute : TestFixtureSourceAttribute
    {
        public TestFixtureSource_Request_CommandAttribute()
            : base(typeof(TestFixtureSource_Request_CommandAttribute), nameof(TestData)) { }

        public static IEnumerable<TestFixtureData> TestData =>
            MediatorTestData.Get_Request_Command().Select(x => new TestFixtureData(x.Request));
    }
}
