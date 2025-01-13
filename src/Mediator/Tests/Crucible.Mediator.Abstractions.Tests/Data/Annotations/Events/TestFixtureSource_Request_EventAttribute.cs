namespace Crucible.Mediator.Abstractions.Tests.Data.Annotations.Events
{
    public class TestFixtureSource_Request_EventAttribute : TestFixtureSourceAttribute
    {
        public TestFixtureSource_Request_EventAttribute()
            : base(typeof(TestFixtureSource_Request_EventAttribute), nameof(TestData)) { }

        public static IEnumerable<TestFixtureData> TestData => MediatorTestData
            .Get_Request_Event()
            .Select(x => new TestFixtureData(x.Request));
    }
}
