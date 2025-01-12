namespace Crucible.Mediator.Abstractions.Tests.Data.Annotations.Commands
{
    public class TestFixtureSource_Request_CommandsAttribute : TestFixtureSourceAttribute
    {
        public TestFixtureSource_Request_CommandsAttribute()
            : base(typeof(TestFixtureSource_Request_CommandsAttribute), nameof(TestData)) { }

        public static IEnumerable<TestFixtureData> TestData => MediatorTestData
            .Get_Request_Command()
            .Select(x => new TestFixtureData(x.Request));
    }
}
