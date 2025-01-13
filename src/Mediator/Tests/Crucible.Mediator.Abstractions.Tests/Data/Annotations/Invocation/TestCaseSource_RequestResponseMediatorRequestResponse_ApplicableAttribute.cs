namespace Crucible.Mediator.Abstractions.Tests.Data.Annotations.Invocation
{
    public class TestCaseSource_RequestResponseMediatorRequestResponse_ApplicableAttribute : TestCaseSourceAttribute
    {
        public TestCaseSource_RequestResponseMediatorRequestResponse_ApplicableAttribute(bool isApplicable, params object[] methodParams)
            : base(typeof(TestCaseSource_RequestResponseMediatorRequestResponse_ApplicableAttribute), isApplicable ? nameof(TestDataApplicable) : nameof(TestDataNoApplicable), methodParams?.Length > 0 ? [methodParams] : [Array.Empty<object>()])
        { }

        public static object[] TestDataApplicable(object[] methodParams) => MediatorTestData
            .Get_RequestResponseMediatorRequestResponse_IsApplicable_All()
            .Select(x => new object[] { x.Request, x.Response, x.MediatorRequest, x.MediatorResponse }.AppendExtraParams(methodParams))
            .ToArray();

        public static object[] TestDataNoApplicable(object[] methodParams) => MediatorTestData
            .Get_RequestResponseMediatorRequestResponse_IsNotApplicable_All()
            .Select(x => new object[] { x.Request, x.Response, x.MediatorRequest, x.MediatorResponse }.AppendExtraParams(methodParams))
            .ToArray();
    }
}
