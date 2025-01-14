using Crucible.Mediator.Abstractions.Tests.Data;
using Crucible.Mediator.Abstractions.Tests.Data.Annotations.Requests;

[assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]

public class A
{
    [Test]
    public void b()
    {
        object[] methodParams = { "a", "b", "c" };

        var g = MediatorTestData
            .Get_RequestResponse_All()
            .Select(x => new object[] { x.Request, x.Response })
            .ToArray();
        var f = TestCaseSource_RequestResponse_AllAttribute.TestData(methodParams);
        var e = MediatorTestData.Get_RequestResponse_All()
            .Select(x => new object[] { x.Request, x.Response }.Concat(methodParams).ToArray())
            .ToArray();
    }
}
