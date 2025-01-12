namespace Crucible.Mediator.Abstractions.Tests.Data.Annotations.Commands
{
    public class TestCaseSource_RequestResponse_CommandsAttribute : TestCaseSourceAttribute
    {
        public TestCaseSource_RequestResponse_CommandsAttribute()
            : base(typeof(TestCaseSource_RequestResponse_CommandsAttribute), nameof(TestData)) { }

        public static IEnumerable<TestCaseData> TestData => MediatorTestData
            .Get_RequestResponse_Command()
            .Select(x => new TestCaseData(x.Request, x.Response));
    }
}
