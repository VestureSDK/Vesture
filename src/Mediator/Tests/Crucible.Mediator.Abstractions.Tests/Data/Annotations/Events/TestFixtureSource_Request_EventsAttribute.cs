namespace Crucible.Mediator.Abstractions.Tests.Data.Annotations.Events
{
    public class TestFixtureSource_Request_EventsAttribute : TestFixtureSourceAttribute
    {
        public TestFixtureSource_Request_EventsAttribute()
            : base(typeof(TestFixtureSource_Request_EventsAttribute), nameof(TestData)) { }

        public static IEnumerable<TestFixtureData> TestData => MediatorTestData
            .Get_Request_Event()
            .Select(x => new TestFixtureData(x.Request));
    }
}
